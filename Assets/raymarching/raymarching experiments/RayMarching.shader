Shader "Auctux/RayMarching"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM

            #define mod(x,y) (x-y*floor(x/y))
            // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "DistanceFunctions.cginc"


            sampler2D _MainTex;
            uniform fixed4 r_mainColor;
            uniform sampler2D _CameraDepthTexture;
            uniform float4x4 r_cameraFrustum, r_cameraToWorld;
            uniform float4 r_box, r_sphere, r_sphere2;
            uniform float radius1;
            uniform float radius2;

            uniform float r_boxRound, r_boxSphereSmooth, r_sphereIntersectSmooth;
            uniform float3 r_modInterval;
            uniform float3 r_light, r_lightColor;
            uniform float r_lightIntensity;
            uniform float r_maxdistance;
            uniform float2 r_shadowDistance;
            uniform float r_shadowIntensity;
            uniform float r_shadowPenumbra;
            uniform float r_ambientSteps;
            uniform int r_ambientIterations;
            uniform float r_ambientIntensity;
            uniform int r_maxIterations;
            uniform float r_accuracy;

            uniform float4 posArray[25];
            uniform float4 r_sphere4;

            uniform float4 _mandleBrot1;
            uniform float _mandleBrotColor1;
            uniform float _power;


            uniform float r_sphereSmooth;
            uniform float r_degreeRotate;
            uniform float r_rotationDegree;

            uniform int r_reflectionCount;
            uniform float r_reflectionIntensity;
            uniform float r_environmentIntensity;
            uniform samplerCUBE r_reflectionCube;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {   
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                // ray direction
                float3 ray: TEXCOORD1;
                
            };

            v2f vert (appdata v)
            {
                
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;
                o.ray = r_cameraFrustum[(int)index].xyz;
                o.ray /= abs(o.ray.z);
                o.ray = mul(r_cameraToWorld, o.ray);
                return o;
            }

            float3 Rotatey(float3 v, float degree) {
                float rad = 0.01745329 * degree;
                float cosY = cos(rad);
                float sinY = sin(rad);
                return float3(cosY * v.x - sinY * v.z, v.y, sinY * v.x + cosY * v.z);
            }

            float sdEllipsoid( in float3 p, in float3 r )
            {
                float k0 = length(p/r);
                float k1 = length(p/(r*r));
                return k0*(k0-1.0)/k1;
            }
            float smin( float a, float b, float k )
            {
                float h = max(k-abs(a-b),0.0);
                return min(a, b) - h*h*0.25/k;

            }
            float2 stalagmite(float3 pos){
                // ground
                float fh = -0.1 - 0.05*(sin(pos.x*2.0)+sin(pos.z*2.0));

                float d = pos.y - fh;
                // bubbles
                
                float2 res;
                
                float3 vp = float3( fmod(abs(pos.x),3.0),pos.y,fmod(pos.z+1.5,3.0)-1.5);
                float2 id = float2( floor(pos.x/3.0), floor((pos.z+1.5)/3.0) );
                float fid = id.x*11.1 + id.y*31.7;
                float fy = frac(fid*1.312+_Time.y*0.1);
                float y = -1.0+4.0*fy;
                float3  rad = float3(0.7,1.0+0.5*sin(fid),0.7);
                rad -= 0.1*(sin(pos.x*3.0)+sin(pos.y*4.0)+sin(pos.z*5.0));    
                float siz = 4.0*fy*(1.0-fy);
                float d2 = sdEllipsoid( vp-float3(2.0,y,0.0), siz*rad );
    
                d2 *= 0.6;
                d2 = min(d2,2.0);
                d = smin( d, d2, 0.32 );

                if( d<res.x ) res = float2(d,1.0);
                
    
                return res;

            }
            float boxAndSphere(float3 p){

                        float Box1 = sdBox(p - r_box.xyz, r_box.www);
                float sphere = sdSphere(p - r_sphere.xyz, radius1);
                //float cone =sdCone( p+float3(3,3,0), float2 (1,1),3 );
                float sphere2 = sdSphere(p - r_sphere2.xyz, radius2);
//                float d = opUS(cone, Box1,10);
                return opUS(sphere, sphere2, 5);
            }
            float r_distanceField(float3 p) {
         
              
             //
             return stalagmite(p).x;
              // return boxAndSphere(p);

    
             // float4 _mandleBrotColor1;
             //float MandleBrot1 = mandleBulb(p-_mandleBrot1.xyz,_mandleBrotColor1.xyzw);
                 //float fractal1 = DE(p-_mandleBrot1.xyz,_power);
       
              //   return MandleBrot1;
            }

            float3 getNormal(float3 p) {
                const float2 offset = float2(0.001, 0.0);
                float3 n = float3(
                    r_distanceField(p + offset.xyy) - r_distanceField(p - offset.xyy),
                    r_distanceField(p + offset.yxy) - r_distanceField(p - offset.yxy),
                    r_distanceField(p + offset.yyx) - r_distanceField(p - offset.yyx)
                    );
                return normalize(n);
                    
            }
            float hardShadow(float3 ray_o, float3 ray_d, float mint, float maxt) {
                for (float t = mint; t < maxt;)
                {
                    float h = r_distanceField(ray_o + ray_d * t);
                    if (h < 0.001) {
                        return 0.0;
                    }
                    t += h;
                }
                return 1.0;
            }
            float softShadow(float3 ray_o, float3 ray_d, float min_travel, float max_travel, float k) {
                float result = 1.0;
                for (float t = min_travel; t < max_travel;)
                {
                    float h = r_distanceField(ray_o + ray_d * t);
                    if (h < 0.001) {
                        return 0.0;
                    }
                    result = min(result, k * h / t);
                    t += h;
                }
                return result;
            }
            float AmbientOcclusion(float3 p, float3 n) {
                float step = r_ambientSteps;
                float ao = 0.0;
                float dist;
                for (int i=1; i <= r_ambientIterations; i++) {
                    dist = step * i;
                    ao += max ( 0.0, (dist - r_distanceField(p + n * dist)) / dist);
                }
                return (1.0 - ao * r_ambientIntensity);
            }
            float3 Shading(float3 p, float3 n) {
                // light
                float3 result;
                float3 color = r_mainColor.rgb;
                float3 light = (r_lightColor * dot(-r_light, n) *0.5 + 0.5) * r_lightIntensity;
                //shadows
                float shadow = softShadow(p, -r_light, r_shadowDistance.x, r_shadowDistance.y, r_shadowPenumbra) * 0.5 + 0.5;
                shadow = max(0.0, pow(shadow, r_shadowIntensity));
                float ao = AmbientOcclusion(p, n);
                result = color * light * shadow * ao;
                return result;
            }
            bool raymarching( float3 r_origin, float3 r_direction, float this_depth, float maxDistance, float maxIterations, inout float3 pos) {
                
                bool hit;
                float ray_dist = 0;

                for (int i = 0; i < maxIterations; i++)
                {
                    if ( ray_dist > maxDistance || ray_dist >= this_depth ) {
                        hit = false;
                        break;
                    }
                     pos = r_origin + r_direction * ray_dist;
                    //collision check
                    float r_dist = r_distanceField(pos);
                    if (r_dist < r_accuracy) {

                        hit = true;
                        break;
                    }
                    ray_dist += r_dist;

                }
                return hit;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float r_depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
                r_depth *= length(i.ray);
                fixed3 col = tex2D(_MainTex, i.uv);
                float3 ray_direction = normalize(i.ray.xyz);
                float3 ray_origin = _WorldSpaceCameraPos;
                fixed4 result;
                float3 hit_position; 
                bool hit = raymarching(ray_origin, ray_direction, r_depth, r_maxdistance, r_maxIterations, hit_position);
                if (hit) {
                    float3 n = getNormal(hit_position);
                    float3 shade = Shading(hit_position, n);
                    result = fixed4(shade, 1);
                    result += fixed4(texCUBE(r_reflectionCube, n).rgb * r_environmentIntensity * r_reflectionIntensity, 0);
                    if (r_reflectionCount > 0) {
                        ray_direction = normalize(reflect(ray_direction, n));
                        ray_origin = hit_position + (ray_direction * 0.01);
                        hit = raymarching(ray_origin, ray_direction, r_maxdistance, r_maxdistance*0.5, r_maxIterations /2, hit_position);
                        if (hit) {
                            float3 n = getNormal(hit_position);
                            float3 shade = Shading(hit_position, n);
                            result += fixed4(shade * r_reflectionIntensity, 0);
                            if (r_reflectionCount > 1) {
                                ray_direction = normalize(reflect(ray_direction, n));
                                ray_origin = hit_position + (ray_direction * 0.01);
                                hit = raymarching(ray_origin, ray_direction, r_maxdistance, r_maxdistance *0.25, r_maxIterations /4, hit_position);
                                if (hit) {
                                    float3 n = getNormal(hit_position);
                                    float3 shade = Shading(hit_position, n);
                                    result += fixed4(shade * r_reflectionIntensity * 0.5, 0);
                                }
                            }
                        }
                    }
                }
                else {
                    result = fixed4(0, 0, 0, 0);
                }
                
                

                return fixed4(col * (1.0 - result.w) + result.xyz * result.w , 1.0);
                
            }
            ENDCG
        }
    }
}
