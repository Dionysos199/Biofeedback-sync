// Sphere
// s: radius
float sdSphere(float3 p, float s)
{
	return length(p) - s;
}
float sdPlane(float3 p, float4 n) {
	return dot(p, n.xyz) + n.w;
}
// Box
// b: size of box in x/y/z
float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) +
		length(max(d, 0.0));
}



float sdRoundBox(in float3 p, in float3 b, in float r) {
	float3 q = abs(p) - b;
	return min(max(q.x, max(q.y, q.z)), 0.0) + length(max(q, 0.0)) - r;
}
float sdCone( float3 p, float2 c, float h )
{
  float q = length(p.xz);
  return max(dot(c.xy,float2(q,p.y)),-h-p.y);
}


float opUS(float d1, float d2, float k) {
	float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) - k * h * (1.0 - h);
}
float opSS(float d1, float d2, float k) {
	float h = clamp(0.5 - 0.5 * (d2 + d1) / k, 0.0, 1.0);
	return lerp(d2, -d1, h) + k * h * (1.0 - h);
}
float opIS(float d1, float d2, float k) {
	float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) + k * h * (1.0 - h);
}
// BOOLEAN OPERATORS //

// Union
float opU(float d1, float d2)
{
	return min(d1, d2);
}

// Subtraction
float opS(float d1, float d2)
{
	return max(-d1, d2);
}
float opSU(float d1, float d2, float k) {
	float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
	return lerp(d2, d1, h) - k * h * (1.0 - h);
}
// Intersection
float opI(float d1, float d2)
{
	return max(d1, d2);
}

// Mod Position Axis
float pMod1 (inout float p, float size)
{
	float halfsize = size * 0.5;
	float c = floor((p+halfsize)/size);
	p = fmod(p+halfsize,size)-halfsize;
	p = fmod(-p+halfsize,size)-halfsize;
	return c;
}

float mandleBulb(in float3 p, out float4 resColor)
{
	float3 w = p;
	float m = dot(w, w);

	float4 trap = float4(abs(w), m);
	float dz = 1.0;

	for (int i = 0; i < 1024; i++)
	{

		// trigonometric version (MUCH faster than polynomial)

		// dz = 8*z^7*dz
		dz = 8.0 * pow(m, 3.5) * dz + 1.0;

		// z = z^8+c
		float r = length(w);
		float b = 8.0 * acos(w.y / r);
		float a = 8.0 * atan2(w.x, w.z);
		w = p + pow(r, 8.0) * float3(sin(b) * sin(a), cos(b), sin(b) * cos(a));


		trap = min(trap, float4(abs(w), m));

		m = dot(w, w);
		if (m > 512.0)
			break;
	}

	resColor = float4(m, trap.yzw);

	// distance estimation (through the Hubbard-Douady potential)
	return 0.25 * log(m) * sqrt(m) / dz;
}
float DE(float3 pos, float Power) {
	float3 z = pos;
	float dr = 1.0;
	float r = 0.0;
	for (int i = 0; i < 16; i++) {
		r = length(z);
		if (r > 64) break;

		// convert to polar coordinates
		float theta = acos(z.z / r);
		float phi = atan2(z.y, z.x);
		dr = pow(r, Power - 1.0) * Power * dr + 1.0;

		// scale and rotate the point
		float zr = pow(r, Power);
		theta = theta * Power;
		phi = phi * Power;

		// convert back to cartesian coordinates
		z = zr * float3(sin(theta) * cos(phi), sin(phi) * sin(theta), cos(theta));
		z += pos;
	}
	return 0.5 * log(r) * r / dr;
}
float sdEllipsoid(in float3 p, in float3 r)
{
	float k0 = length(p / r);
	float k1 = length(p / (r * r));
	return k0 * (k0 - 1.0) / k1;
}
float smin(float a, float b, float k)
{
	float h = max(k - abs(a - b), 0.0);
	return min(a, b) - h * h * 0.25 / k;

}

float2 stalagmite(float3 pos) {
	// ground
	float fh = -0.1 - 0.05 * (sin(pos.x * 2.0) + sin(pos.z * 2.0));

	float d = pos.y - fh;
	// bubbles

	float2 res;

	float3 vp = float3(fmod(abs(pos.x), 3.0), pos.y, fmod(pos.z + 1.5, 3.0) - 1.5);
	float2 id = float2(floor(pos.x / 3.0), floor((pos.z + 1.5) / 3.0));
	float fid = id.x * 11.1 + id.y * 31.7;
	float fy = frac(fid * 1.312 + _Time.z * 0.02);
	float y = -1.0 + 4.0 * fy;
	float3  rad = float3(0.7, 1.0 + 0.5 * sin(fid), 0.7);
	rad -= 0.1 * (sin(pos.x * 3.0) + sin(pos.y * 4.0) + sin(pos.z * 5.0));
	float siz = 4.0 * fy * (1.0 - fy);
	float d2 = sdEllipsoid(vp - float3(2.0, y, 0.0), siz * rad);

	d2 *= 0.6;
	d2 = min(d2, 2.0);
	d = smin(d, d2, 0.32);

	if (d < res.x) res = float2(d, 1.0);


	return res;

}

float2x2 rot (float a)
{ float c=cos(a),s=sin(a); return float2x2(c,-s,s,c); }

float gyroid (float3 seed)
{ return dot(sin(seed),cos(seed.yzx)); }

// noise
float fbm (float3 seed)
{
    float result = 0., a = .5;
    for (int i = 0; i < 8; ++i, a/=2.)
    {
        result += abs(gyroid(seed/a))*a;
    }
    return result;
}

// signed distance function
float chargedCloud(float3 p, float glow)
{
    float dist = 100.;
    
    // cloud
    float3 seed = p*.4;
    seed.z += _Time.y*.1;
    float noise = fbm(seed);
    dist = length(p) - .5 - noise*1.;
    
    // lightning
    const float count = 4.;
    float a = 1.;
    float t = _Time.y*.2 + noise*.5;
    float r = .1+.2*sin(_Time.y+p.x);
    float shape = 100.;
    for (float i = 0.; i < count; ++i)
    {
        p.xz =mul(rot(t/a),p.xz);
		
        p.xy =mul(rot(t/a),p.xy);
        p = abs(p)-r*a;
        shape = min(shape, length(p.xz));
        a /= 1.8;
    }
    glow += .002/shape;
    dist = min(dist, shape);
    
    return dist*.8;
}
