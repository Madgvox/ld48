using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Polar2 {
    public float d;
    public float r;

	public static Polar2 zero {
        get {
            return new Polar2( 0, 0 );
		}
    }

	public Polar2 ( float r, float d ) {
        this.r = r;
        this.d = d;
	}

    public static implicit operator Vector2 ( Polar2 p ) {
        Vector2 v;

        v.x = p.r * Mathf.Cos( p.d );
        v.y = p.r * Mathf.Sin( p.d );

        return v;
    }

    public static implicit operator Vector3 ( Polar2 p ) {
        Vector3 v;

        v.x = p.r * Mathf.Cos( p.d );
        v.y = p.r * Mathf.Sin( p.d );
        v.z = 0;

        return v;
    }

    public static float Distance ( Polar2 a, Polar2 b ) {
        return Mathf.Sqrt( a.r * a.r + b.r * b.r - ( 2 * a.r * b.r * Mathf.Cos( a.d - b.d ) ) );
	}

    public static float SqrDistance ( Polar2 a, Polar2 b ) {
        return a.r * a.r + b.r * b.r - ( 2 * a.r * b.r * Mathf.Cos( a.d - b.d ) );
	}

    public static explicit operator Polar2 ( Vector2 v ) {
        Polar2 p;

        p.d = Mathf.Atan( v.y / v.x );
        p.r = v.magnitude;

        return p;
    }

    public static explicit operator Polar2 ( Vector3 v ) {
        Polar2 p;

        p.d = Mathf.Atan( v.y / v.x );
        p.r = v.magnitude;

        return p;
    }
}

public class SpiralController : MonoBehaviour {

    public float outerRadius;

    public float turns;

    public float stepSize;

    public float startAngle;

    public float a = 10f;

    public float b = 5f;

    const float MIN_STEP = 0.03f;

    const float E = 2.718281f;

    public float pitch = 10;

	public AnimationCurve curve;

    // Start is called before the first frame update
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {

    }

	private void OnDrawGizmos () {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.red;

		var start = new Polar2( outerRadius, 0 );

		var last = (Vector3)start;
		var current = start;

		var turnRad = turns * Mathf.PI * 2;

		if( stepSize < MIN_STEP ) stepSize = MIN_STEP;

		var numSteps = turnRad / stepSize;


		if( numSteps >= 1 ) {
			for( var i = 0; i < numSteps; i++ ) {
				var p = i / numSteps;
				current.r = Mathf.Lerp( outerRadius, 0, curve.Evaluate( p ) );
				current.d += stepSize;
				var c = (Vector3)current;
				Gizmos.DrawLine( last, c );
				last = c;
			}
		}

		//for( var i = 0; i < 200; i++ ) {
		//    var p = i / 200f;
		//    current.r = Mathf.Lerp( start.r, 0, Mathf.Log( p ) );
		//    current.d += stepSize;
		//    Gizmos.DrawLine( last, current );
		//    last = current;
		//}

		//var pitchRad = pitch * Mathf.Deg2Rad;

		//for( var i = 0; i < 200; i++ ) {
		//          var r = a * Mathf.Pow( E, pitchRad * ( 1 / Mathf.Tan( b ) ) );

		//          var p = new Polar2();
		//}

		//      float minTheta = Mathf.Log( 0.1f / a ) / b;

		//      float delta = 5 * Mathf.Deg2Rad;

		//      Polar2 last = Polar2.zero;

		//      if( a <= 0 ) a = 0.1f;
		//      if( b <= 0 ) b = 0.1f;

		//      for( var theta = minTheta; ; theta += delta ) {
		//          float r = a * Mathf.Exp( b * theta );

		//          var p = new Polar2( r, theta + startAngle );

		//          Gizmos.DrawLine( last, p );

		//          last = p;

		//          if( r > outerRadius ) break;    
		//}

		//for( var i = 0; i < 200; i++ ) {
		//          var p = new Polar2( , );
		//	current.r = Mathf.Lerp( start.r, 0, Mathf.Log( p ) );
		//	current.d += stepSize;
		//	Gizmos.DrawLine( last, current );
		//	last = current;
		//}
	}

	// Return points that define a spiral.
	//private List<PointF> GetSpiralPoints (
	//    PointF center, float A, float B,
	//    float angle_offset, float max_r ) {
	//    // Get the points.
	//    List<PointF> points = new List<PointF>();
	//    const float dtheta = (float)(5 * Math.PI / 180);    // Five degrees.
	//    float min_theta = (float)(Math.Log(0.1 / A) / B);
	//    for( float theta = min_theta; ; theta += dtheta ) {
	//        // Calculate r.
	//        float r = (float)(A * Math.Exp(B * theta));

	//        // Convert to Cartesian coordinates.
	//        float x, y;
	//        PolarToCartesian( r, theta + angle_offset, out x, out y );

	//        // Center.
	//        x += center.X;
	//        y += center.Y;

	//        // Create the point.
	//        points.Add( new PointF( (float)x, (float)y ) );

	//        // If we have gone far enough, stop.
	//        if( r > max_r ) break;
	//    }
	//    return points;
	//}
}
