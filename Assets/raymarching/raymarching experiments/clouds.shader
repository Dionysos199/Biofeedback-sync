Shader "Unlit/clouds"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            #define NOISE_METHOD 1

// 0: no LOD
// 1: yes LOD
#define USE_LOD 1

// 0: sunset look
// 1: bright look
#define LOOK 1

float3x3 setCamera( in float3 ro, in float3 ta, float cr )
{
	float3 cw = normalize(ta-ro);
	float3 cp = float3(sin(cr), cos(cr),0.0);
	float3 cu = normalize( cross(cw,cp) );
	float3 cv = normalize( cross(cu,cw) );
    return float3x3( cu, cv, cw );
}

float noise( in float3 x )
{
    float3 p = floor(x);
    float3 f = frac(x);
	f = f*f*(3.0-2.0*f);

#if NOISE_METHOD==0
    x = p + f;
    return tex2Dlod(iChannel2,(x+0.5)/32.0,0.0).x*2.0-1.0;
#endif
#if NOISE_METHOD==1
	float2 uv = (p.xy+float2(37.0,239.0)*p.z) + f.xy;
    float2 rg = tex2Dlod(iChannel0,(uv+0.5)/256.0,0.0).yx;
	return lerp( rg.x, rg.y, f.z )*2.0-1.0;
#endif    
#if NOISE_METHOD==2
    ifloat3 q = ifloat3(p);
	ifloat2 uv = q.xy + ifloat2(37,239)*q.z;
	float2 rg = lerp(lerp(texelFetch(iChannel0,(uv           )&255,0),
				      texelFetch(iChannel0,(uv+ifloat2(1,0))&255,0),f.x),
				  lerp(texelFetch(iChannel0,(uv+ifloat2(0,1))&255,0),
				      texelFetch(iChannel0,(uv+ifloat2(1,1))&255,0),f.x),f.y).yx;
	return lerp( rg.x, rg.y, f.z )*2.0-1.0;
#endif    
}

#if LOOK==0
float map( in float3 p, int oct )
{
	float3 q = p - float3(0.0,0.1,1.0)*iTime;
    float g = 0.5+0.5*noise( q*0.3 );
    
	float f;
    f  = 0.50000*noise( q ); q = q*2.02;
    #if USE_LOD==1
    if( oct>=2 ) 
    #endif
    f += 0.25000*noise( q ); q = q*2.23;
    #if USE_LOD==1
    if( oct>=3 )
    #endif
    f += 0.12500*noise( q ); q = q*2.41;
    #if USE_LOD==1
    if( oct>=4 )
    #endif
    f += 0.06250*noise( q ); q = q*2.62;
    #if USE_LOD==1
    if( oct>=5 )
    #endif
    f += 0.03125*noise( q ); 
    
    f = lerp( f*0.1-0.75, f, g*g ) + 0.1;
    return 1.5*f - 0.5 - p.y;
}

const int kDiv = 1; // make bigger for higher quality
const float3 sundir = normalize( float3(-1.0,0.0,-1.0) );

float4 raymarch( in float3 ro, in float3 rd, in float3 bgcol, in ifloat2 px )
{
    // bounding planes	
    const float yb = -3.0;
    const float yt =  0.6;
    float tb = (yb-ro.y)/rd.y;
    float tt = (yt-ro.y)/rd.t;

    // find tigthest possible raymarching segment
    float tmin, tmax;
    if( ro.y>yt )
    {
        // above top plane
        if( tt<0.0 ) return float4(0.0); // early exit
        tmin = tt;
        tmax = tb;
    }
    else
    {
        // inside clouds slabs
        tmin = 0.0;
        tmax = 60.0;
        if( tt>0.0 ) tmax = min( tmax, tt );
        if( tb>0.0 ) tmax = min( tmax, tb );
    }
    
    // dithered near distance
    float t = tmin + 0.1*texelFetch( iChannel1, px&1023, 0 ).x;
    
    // raymarch loop
	float4 sum = float4(0.0);
    for( int i=0; i<190*kDiv; i++ )
    {
       // step size
       float dt = max(0.05,0.02*t/float(kDiv));

       // lod
       #if USE_LOD==0
       const int oct = 5;
       #else
       int oct = 5 - int( log2(1.0+t*0.5) );
       #endif
       
       // sample cloud
       float3 pos = ro + t*rd;
       float den = map( pos,oct );
       if( den>0.01 ) // if inside
       {
           // do lighting
           float dif = clamp((den - map(pos+0.3*sundir,oct))/0.3, 0.0, 1.0 );
           float3  lin = float3(0.65,0.65,0.75)*1.1 + 0.8*float3(1.0,0.6,0.3)*dif;
           float4  col = float4( lerp( float3(1.0,0.95,0.8), float3(0.25,0.3,0.35), den ), den );
           col.xyz *= lin;
           // fog
           col.xyz = lerp(col.xyz,bgcol, 1.0-exp2(-0.075*t));
           // composite front to back
           col.w    = min(col.w*8.0*dt,1.0);
           col.rgb *= col.a;
           sum += col*(1.0-sum.a);
       }
       // advance ray
       t += dt;
       // until far clip or full opacity
       if( t>tmax || sum.a>0.99 ) break;
    }

    return clamp( sum, 0.0, 1.0 );
}

float4 render( in float3 ro, in float3 rd, in ifloat2 px )
{
	float sun = clamp( dot(sundir,rd), 0.0, 1.0 );

    // background sky
    float3 col = float3(0.76,0.75,0.86);
    col -= 0.6*float3(0.90,0.75,0.95)*rd.y;
	col += 0.2*float3(1.00,0.60,0.10)*pow( sun, 8.0 );

    // clouds    
    float4 res = raymarch( ro, rd, col, px );
    col = col*(1.0-res.w) + res.xyz;
    
    // sun glare    
	col += 0.2*float3(1.0,0.4,0.2)*pow( sun, 3.0 );

    // tonemap
    col = smoothstep(0.15,1.1,col);
 
    return float4( col, 1.0 );
}

#else


float map5( in float3 p )
{    
    float3 q = p - float3(0.0,0.1,1.0)*iTime;    
    float f;
    f  = 0.50000*noise( q ); q = q*2.02;    
    f += 0.25000*noise( q ); q = q*2.03;    
    f += 0.12500*noise( q ); q = q*2.01;    
    f += 0.06250*noise( q ); q = q*2.02;    
    f += 0.03125*noise( q );    
    return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
}
float map4( in float3 p )
{    
    float3 q = p - float3(0.0,0.1,1.0)*iTime;    
    float f;
    f  = 0.50000*noise( q ); q = q*2.02;    
    f += 0.25000*noise( q ); q = q*2.03;    
    f += 0.12500*noise( q ); q = q*2.01;   
    f += 0.06250*noise( q );    
    return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
}
float map3( in float3 p )
{
    float3 q = p - float3(0.0,0.1,1.0)*iTime;    
    float f;
    f  = 0.50000*noise( q ); q = q*2.02;    
    f += 0.25000*noise( q ); q = q*2.03;    f += 0.12500*noise( q );    
    return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
}
float map2( in float3 p )
{    
    float3 q = p - float3(0.0,0.1,1.0)*iTime;    
    float f;
    f  = 0.50000*noise( q ); 
    q = q*2.02;    f += 0.25000*noise( q );;    
    return clamp( 1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0 );
}

const float3 sundir = float3(-0.7071,0.0,-0.7071);

#define MARCH(STEPS,MAPLOD) for(int i=0; i<STEPS; i++) { float3 pos = ro + t*rd; if( pos.y<-3.0 || pos.y>2.0 || sum.a>0.99 ) break; float den = MAPLOD( pos ); if( den>0.01 ) { float dif = clamp((den - MAPLOD(pos+0.3*sundir))/0.6, 0.0, 1.0 ); float3  lin = float3(1.0,0.6,0.3)*dif+float3(0.91,0.98,1.05); float4  col = float4( lerp( float3(1.0,0.95,0.8), float3(0.25,0.3,0.35), den ), den ); col.xyz *= lin; col.xyz = lerp( col.xyz, bgcol, 1.0-exp(-0.003*t*t) ); col.w *= 0.4; col.rgb *= col.a; sum += col*(1.0-sum.a); } t += max(0.06,0.05*t); }

float4 raymarch( in float3 ro, in float3 rd, in float3 bgcol, in ifloat2 px )
{    
    float4 sum = float4(0.0);    
    float t = 0.05*texelFetch( iChannel1, px&255, 0 ).x;    
    MARCH(40,map5);    
    MARCH(40,map4);    
    MARCH(30,map3);    
    MARCH(30,map2);    
    return clamp( sum, 0.0, 1.0 );
}

float4 render( in float3 ro, in float3 rd, in ifloat2 px )
{
    // background sky         
    float sun = clamp( dot(sundir,rd), 0.0, 1.0 );    
    float3 col = float3(0.6,0.71,0.75) - rd.y*0.2*float3(1.0,0.5,1.0) + 0.15*0.5;    
    col += 0.2*float3(1.0,.6,0.1)*pow( sun, 8.0 );    
    // clouds        
    float4 res = raymarch( ro, rd, col, px );    
    col = col*(1.0-res.w) + res.xyz;        
    // sun glare        
    col += float3(0.2,0.08,0.04)*pow( sun, 3.0 );    
    return float4( col, 1.0 );
}

#endif

void mainImage( out float4 fragColor, in float2 fragCoord )
{
    float2 p = (2.0*fragCoord-iResolution.xy)/iResolution.y;
    float2 m =                iMouse.xy      /iResolution.xy;

    // camera
    float3 ro = 4.0*normalize(float3(sin(3.0*m.x), 0.8*m.y, cos(3.0*m.x))) - float3(0.0,0.1,0.0);
	float3 ta = float3(0.0, -1.0, 0.0);
    float3x3 ca = setCamera( ro, ta, 0.07*cos(0.25*iTime) );
    // ray
    float3 rd = ca * normalize( float3(p.xy,1.5));
    
    fragColor = render( ro, rd, ifloat2(fragCoord-0.5) );
}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
