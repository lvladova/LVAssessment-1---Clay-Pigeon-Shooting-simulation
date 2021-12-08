using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptionControllerTurret : MonoBehaviour {
    public float launchVelocity = 10f; // the launch velocity of the projectile
    public float Gravity = -9.8f; // the gravity that effects the projectile
    public float fVisionRadius = 20f;
    private List<ProjectileVariant> projectilesVariants;
    private List<ProjectileBullet> projectileBullets;

    public ProcetileVariantLauncher launcher;
    public GameObject launchPoint;
    public GameObject ProjectileBullet;
    
    public Vec3 v3InitialVelocity = new Vec3(); // launch velocity as a vector
    public Vec3 v3Acceleration; // vector quantity for acceleration
    
    public float airTime = 0f;      // how long the projectile will be in the air
    public float horizontalDisplacment = 0f; // how far in the horizontal panel the projectile will travel


    // Start is called before the first frame update
    void Start() {
        projectileBullets = new List<ProjectileBullet>();
        projectilesVariants = new List<ProjectileVariant>();
        CalculateProjectileBullet();
    }
    float FindTimeToIntercept(float launcherVelocity, Vec3 projectilePos, Vec3 projectileVariantVel) {
        //law of cos form c^2 = a^2 + b^2 - 2ab*cos(phi)
        //reareange to look like a quadratic form
        //x = (lv - pv)t^2 + (2*ab*cos(phi)) t- a^2);
        //for quadratic x = ax^2 + bx + x
        //a = (lv-pv)^2
        //b = (2ab* A.B)
        //c = a^2

        //getting direction to projectile for AS.B dot product(we want the direction from the pigeon towards the gun)
        Vec3 directionShootertoProjectileVariant = new Vec3(transform.position) - projectilePos;
        //the distance to the pigion = targer 
        float distancetoProjectileVariantSquared = directionShootertoProjectileVariant.MagnitudeSquared();
        //as the projectile only exist in the Y axis we can remorce this part of the velocity as it only creats complexicity
        //and we can avoid it by ignoring it and making it a problem existing only in the X/Z plane
        Vec3 horizontalProjectileVariantVelocity = new Vec3(projectileVariantVel.x, 0f, projectileVariantVel.z);

        //for the quadratic eq
        float c = -(distancetoProjectileVariantSquared);
        //abcos(phi), [A] = a, [B] = b
        float b = 2 * Vec3.DotProduct(directionShootertoProjectileVariant, horizontalProjectileVariantVelocity);
        float a = horizontalProjectileVariantVelocity.Dot(horizontalProjectileVariantVelocity) - launcherVelocity * launcherVelocity;

        float timeToIntercept = UseQuadraticFormula(a, b, c);
        return timeToIntercept;
    }
    float UseQuadraticFormula(float a, float b, float c) {
        //if a is nearly 0 then the formula is not true
        if (0.0001f > Mathf.Abs(a)) {
            return 0f;
		}

        float bb = b * b;
        float ac = a * c;
        float b4ac = bb - 4f * ac;
        if (b4ac < 0f) {
            return -1f;
		}
        b4ac = Mathf.Sqrt(b4ac);
        float t1 = (-b + b4ac) / (2f * a);
        float t2 = (-b - b4ac) / (2f * a);
        float t = Mathf.Max(t1, t2); // only returning the hishest value as one of these might be negative
        return t;
	}
    private void CalculateProjectileBullet() {
        //work out velocity as a vector quantity
        Vec3 lp = new Vec3(launchPoint.transform.position);
        Vec3 lf_fwd = new Vec3(-launchPoint.transform.right);
        lf_fwd.Normalize();
        float launchHeight = lp.y;
        v3InitialVelocity = lf_fwd * launchVelocity;
        //gravity as avec3
        v3Acceleration = new Vec3(0f, Gravity, 0f);
        //calc total time in the air
        airTime = UseQuadraticFormula(v3Acceleration.y, v3InitialVelocity.y * 2f, launchHeight * 2f);
        //calc total distance travelled in the z
        horizontalDisplacment = airTime * v3InitialVelocity.z;

    }
    
    private void FireProjectileBullet(Vec3 direction, float a_lifeSpan) {
        transform.rotation = Quaternion.LookRotation(direction.ToVector3());

        CalculateProjectileBullet();

        //inst the lauch point position, with the current rotation
        GameObject bullet = Instantiate(ProjectileBullet, launchPoint.transform.position, launchPoint.transform.rotation);
        bullet.GetComponent<ProjectileBullet>().Velocity = v3InitialVelocity;
        bullet.GetComponent<ProjectileBullet>().Acceleration = v3Acceleration;
        bullet.GetComponent<ProjectileBullet>().Lifespan = a_lifeSpan;

        //plays an audio of shooting
        GetComponent<AudioSource>().Play();
        ShootingScore.scoreValue += 1;
    }
    // Update is called once per frame
    void Update() {
        CalculateProjectileBullet();
        projectilesVariants.Clear();
        projectileBullets.Clear();
        launcher.DrawPath();
        

            //use Unity's FindObjectWithTag function to find all the targets in the scene
            GameObject[] allActiveTargets = GameObject.FindGameObjectsWithTag("ProjectileVariant");
            Vec3 position = new Vec3(transform.position); // position of the turret
            float radSqrd = fVisionRadius * fVisionRadius;

            foreach (GameObject pigeonTarget in allActiveTargets) {
                //getting the distance to this target
                Vec3 vecToTarget = new Vec3(pigeonTarget.transform.position) - position;
                float distanceToTarget = vecToTarget.MagnitudeSquared(); //use maf squard to avoid calc for speed

                if (distanceToTarget < radSqrd) {
                    ProjectileVariant pigeon = pigeonTarget.GetComponent<ProjectileVariant>();
                    projectilesVariants.Add(pigeon);
                }
            }

            //cacl time to each target intercept within range
            List<(int id, float time)> interceptTime = new List<(int id, float time)>();
            for (int i = 0; i < projectilesVariants.Count; i++) {
                interceptTime.Add((i, FindTimeToIntercept(launcher.launchVelocity, projectilesVariants[i].Position, projectilesVariants[i].Velocity)));


            }
            //get the index in the tube with the shortest time to intercept
            int index = -1;
            float fiTime = float.MaxValue;

            foreach (var intercept in interceptTime) {
                if (intercept.time > 0f && intercept.time < fiTime) {
                    fiTime = intercept.time;
                    index = intercept.id;
                }
            }

            if (index != -1) {
                Debug.Log("Closet interception time is: " + fiTime + ", For item at index: " + index);
                //calc the position of the projectileVariant - pigeon - at this time interval
                ProjectileVariant p = projectilesVariants[index];
                //getting the future positin using s = ut+ 1/2at^2
                Vec3 predictedPos = p.Position + (p.Velocity * fiTime + p.Acceleration * 0.5f * fiTime * fiTime);

                Vec3 dirToPos = predictedPos - position;
                float dirToTarget = dirToPos.Normalize();
            //pressing space key to start the shooting
            if (Input.GetKey(KeyCode.Space)) {
                FireProjectileBullet(dirToPos, 2);
            }
        }
    }
    // for the shooting of the pigion
}
    
