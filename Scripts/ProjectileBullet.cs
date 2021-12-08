using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBullet : MonoBehaviour {
    private Vec3 v3CurrentVelocity = new Vec3(); // launch velocity as a vector
    private Vec3 v3Acceleration = new Vec3(); // vector quantity for acceleration
    private float fLifespan = 0f; // lifespan of the gameobject
    [SerializeField]
    GameObject ExplosionEffects;
    public Vec3 Velocity {
		get { return v3CurrentVelocity; }
        set { v3CurrentVelocity = value; }
	}

    public Vec3 Acceleration {
		get { return v3Acceleration; }
		set { v3Acceleration = value; }
	}

    public float Lifespan {
		set { fLifespan = value; }
        get { return fLifespan; }   
	}

    public Vec3 Position {
        set { transform.position = value.ToVector3(); }
        get { return new Vec3(transform.position); }  
	}
   
    private void FixedUpdate() {
        float microTimeStep = Time.deltaTime * 1.0f;

        Vec3 currentPos = Position;
         v3CurrentVelocity += v3Acceleration * microTimeStep;
        Vec3 displacment = v3CurrentVelocity * microTimeStep;
        currentPos += displacment;
        Position = currentPos;
        fLifespan -= microTimeStep;
        if ( fLifespan < 0f) {
            Destroy(gameObject);
		}
    }

    //particle effect explossion from unity asset store
    //is activated when the bullet aka the projectile gets destroied
	private void OnDestroy() {
       GameObject vfxplosion =  GameObject.Instantiate(ExplosionEffects, transform.position, transform.rotation, null);
	}
}
