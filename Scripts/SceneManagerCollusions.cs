using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerCollusions : MonoBehaviour {
    bool booPaused = false;

    //for the Esc key quit of the game
    //reference used from: https://answers.unity.com/questions/899037/applicationquit-not-working-1.html
    public void QuitTheGame() {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    // Update is called once per frame
    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) { //presing Esc to quit the game
            QuitTheGame();
		}
 
        if (Input.GetKeyDown(KeyCode.P)) { // for pausing the game
			if (booPaused) {
                Time.timeScale = 1;
			} else {
                Time.timeScale = 0;
			}
            booPaused = !booPaused;
		}

        //Create a new  Octree Root node that encompases the entire scene
        OctreeNode rootNode = new OctreeNode(new Vec3(-50f, 100f, -100f), new Vec3(100f, 100f, 100f));
        //Find all projectileBullets that are launched into the scene
         GameObject[] allActiveProjectileVariants = GameObject.FindGameObjectsWithTag("ProjectileVariant");
         GameObject[] allActiveProjectiles = GameObject.FindGameObjectsWithTag("ProjectileBullet");
        foreach (GameObject ProjectileVariant in allActiveProjectileVariants) {
            //add the projectileVariants to the root node
            rootNode.AddObject(ProjectileVariant);
        }

       foreach (GameObject ProjectileBullet in allActiveProjectiles) {
            //add the projectileBullets to the root node
            rootNode.AddObject(ProjectileBullet);
        }

        // draw the octree
        rootNode.Draw();
        rootNode.PerformCollisonTest();
    }
}
