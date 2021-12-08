using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcetileVariantLauncher : MonoBehaviour {
    public float launchVelocity = 10f; // the launch velocity of the projectile
    public float Gravity = -9.8f; // the gravity that effects the projectile

    public GameObject ProjectileVariant;
    public GameObject launchPoint;
    public Vec3 v3InitialVelocity = new Vec3(); // launch velocity as a vector
    public Vec3 v3Acceleration; // vector quantity for acceleration
    
    public float airTime = 0f;      // how long the projectile will be in the air
    public float horizontalDisplacment = 0f; // how far in the horizontal panel the projectile will travel

    private List<Vec3> pathPoints; // list of point along the path of the vector for drawing line of travel
    private int simulationSteps = 30; // number of points on the path of the projectile to 

    private bool hasShot = false;

    // Start is called before the first frame update
    void Start()
    {
        //initialise path vector for drawing
        pathPoints = new List<Vec3>();
        CalculateProjectileVariant();
        CalculatePath();
    }
    private void CalculateProjectileVariant() {
        //work out velocity as a vector quantity
        Vec3 lp = new Vec3(launchPoint.transform.position);
        Vec3 lf_fwd = new Vec3(-launchPoint.transform.right);
        lf_fwd.Normalize();
        float launchHeight = lp.y;
        v3InitialVelocity = lf_fwd * launchVelocity;
        //gravity as avec3
        v3Acceleration = new Vec3(0f, Gravity, 0f);
        //calc total time in the air
        airTime = QuadraticFormula(v3Acceleration.y, v3InitialVelocity.y * 2f, launchHeight * 2f);
        //calc total distance travelled in the z
        horizontalDisplacment = airTime * v3InitialVelocity.z;

    }

    //making a function to use the quadratic formula 
    float QuadraticFormula(float a, float b, float c) {
        if ( 0.0001f > Mathf.Abs(a)) {
            return 0f;
		}
        float bb = b * b;
        float ac = a * c;
        float b4ac = Mathf.Sqrt(bb - 4f * ac);
        float t1 = (-b + b4ac) / (2f * a);
        float t2 = (-b - b4ac) / (2f * a);
        float t = Mathf.Max(t1, t2);
        return t;
	}
    private void CalculatePath() {
        Vec3 launchPos = new Vec3(launchPoint.transform.position);
        pathPoints.Add(launchPos);

        for( int i = 0; i < simulationSteps; i++) {
            float simTime = (i / (float)simulationSteps) * airTime;
            //suvat formula for displacement s = ut + 1/2at^2
            Vec3 displacment = (v3InitialVelocity * simTime) + ( 0.5f * v3Acceleration * (simTime * simTime));
            Vec3 drwaPoint = launchPos + displacment;
            pathPoints.Add(drwaPoint);

		}
	}
    //drawing the line of shooting out the pigeon
   public void DrawPath() {
        for ( int i = 0; i < pathPoints.Count-1; i++) {
            Debug.DrawLine(pathPoints[i].ToVector3(), pathPoints[i + 1].ToVector3(), Color.red);
		}
	}
    //random shooting of the projectileVariant
    //https://docs.unity3d.com/400/Documentation/ScriptReference/index.Coroutines_26_Yield.html
    public IEnumerator FireProjectileVariant(float a_lifeSpan) {
        float randit = Random.Range(0.5f, 0.5f);
        yield return new WaitForSeconds(randit);
        GameObject p = Instantiate(ProjectileVariant, launchPoint.transform.position, launchPoint.transform.rotation);
        p.GetComponent<ProjectileVariant>().Velocity = v3InitialVelocity;
        p.GetComponent<ProjectileVariant>().Acceleration = v3Acceleration;
        p.GetComponent<ProjectileVariant>().Lifespan = a_lifeSpan;
        hasShot = false;
    }
        // Update is called once per frame
   void Update() {
        pathPoints.Clear();
        CalculateProjectileVariant();
        CalculatePath();
        DrawPath();
        //calculating the launch speed of the pigeon aka ProjectileVariant s = ut + 1/2at^2 
        //considering airspeed = launchspeed because there are no forces expet gravity in the game
        float laucnhspeed = (horizontalDisplacment - (0.5f * v3Acceleration.y * airTime * airTime)) / airTime;
        int speed = (int)laucnhspeed; // converting the number to int value
        if ( speed < 0) {
            Speed.speed = 0;
		}
        else
        Speed.speed = speed; // for the UI

        //random shooting of a projectile
        if ( hasShot == false) {
            hasShot = true;
            StartCoroutine(FireProjectileVariant(airTime));
		}
    }
}
