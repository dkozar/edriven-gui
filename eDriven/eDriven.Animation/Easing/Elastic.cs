using System;

namespace eDriven.Animation.Easing
{
    public class Elastic
    {
        public static float EaseIn (float t, float b, float c, float d, float a, float p) {
            float s;
            if (t==0) return b;
            if ((t/=d)==1) return b+c;  
            if (0 == p) p=d*.3f;
		    if (0 == a || a < Math.Abs(c)) { a=c; s=p/4; }
		    else { s = (float) (p/(2*Math.PI) * Math.Asin (c/a)); }
		    return (float) (-(a*Math.Pow(2,10*(t-=1)) * Math.Sin( (t*d-s)*(2*Math.PI)/p )) + b);
	    }

	    public static float EaseOut (float t, float b, float c, float d, float a, float p) {
		    float s;
            if (t==0) return b;  if ((t/=d)==1) return b+c;  if (0 == p) p=d*.3f;
		    if (0 == a || a < Math.Abs(c)) { a=c; s=p/4; }
		    else s = (float) (p/(2*Math.PI) * Math.Asin(c/a));
		    return (float) (a*Math.Pow(2,-10*t) * Math.Sin( (t*d-s)*(2*Math.PI)/p ) + c + b);
	    }

	    public static float EaseInOut (float t, float b, float c, float d, float a, float p) {
		    float s;
            if (t==0) return b;  if ((t/=d/2)==2) return b+c;  if (0 == p) p=d*(.3f*1.5f);
		    if (0 == a || a < Math.Abs(c)) { a=c; s=p/4; }
		    else s = (float) (p/(2*Math.PI) * Math.Asin (c/a));
		    if (t < 1) return (float) (-.5*(a*Math.Pow(2,10*(t-=1)) * Math.Sin( (t*d-s)*(2*Math.PI)/p )) + b);
		    return (float) (a*Math.Pow(2,-10*(t-=1)) * Math.Sin( (t*d-s)*(2*Math.PI)/p )*.5 + c + b);
	    }
    }
}