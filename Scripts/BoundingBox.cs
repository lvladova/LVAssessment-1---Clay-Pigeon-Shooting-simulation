using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox {
    //A bounding box is always axis aligned
    private static Vec3 xAxis = new Vec3(1f, 0f, 0f);
    private static Vec3 yAxis = new Vec3(0f, 1f, 0f);
    private static Vec3 zAxis = new Vec3(0f, 0f, 1f);

    //As this box may be asked numerous times for it's min and max extremes we will calculate these
    //once and store them in the following six variables to avoid the need to calculate them again.
    private float xMin = 0f;
    private float xMax = 0f;
    private float yMin = 0f;
    private float yMax = 0f;
    private float zMin = 0f;
    private float zMax = 0f;

    //Properties for the position of the box
    private Vec3 v3Position = new Vec3(-10f, 0f, 0f);
    public Vec3 Position {
        set { v3Position = value; CalcExtremes(); }
		get { return v3Position; }
	}

    private Vec3 v3Extents = new Vec3(0f, 0f, 0f);
    public Vec3 Extents {
		set { v3Extents = value; CalcExtremes(); }
		get { return v3Extents; }
	}
    //calculate the extremes of the box in the x/y/z axis remeber the box's pivot is at it's centere
    //extents are half the bounds of the bounding box;
    private void CalcExtremes() {
        xMin = v3Position.x - v3Extents.x;
        xMax = v3Position.x + v3Extents.x;
        yMin = v3Position.y - v3Extents.y;
        yMax = v3Position.y + v3Extents.y;
        zMin = v3Position.z - v3Extents.z;
        zMax = v3Position.z + v3Extents.z;
    }

    public bool ContainsObect(Vec3 a_posi, Vec3 a_bounnds) {
        //bounding box origin is at center of box
        Vec3 half_a_bounnds = a_bounnds * 0.5f;
        if (a_posi.x - half_a_bounnds.x < xMax && a_posi.x + half_a_bounnds.x > xMin &&
             a_posi.y - half_a_bounnds.y < yMax && a_posi.y + half_a_bounnds.y > yMin &&
             a_posi.z - half_a_bounnds.z < zMax && a_posi.z + half_a_bounnds.z > zMin) {
            return true;
        }
        return false;
    }

    //A function to visualise the box within the Unity Editor
    public void Draw() {
        //to draw the box we need the draw 12 lines in total
        //Draw vetical lines of the box
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMin, yMin, zMin), Color.green);
        Debug.DrawLine(new Vector3(xMin, yMax, zMax), new Vector3(xMin, yMin, zMax), Color.green);
        Debug.DrawLine(new Vector3(xMax, yMax, zMin), new Vector3(xMax, yMin, zMin), Color.green);
        Debug.DrawLine(new Vector3(xMax, yMax, zMax), new Vector3(xMax, yMin, zMax), Color.green);
        //Draw the top lines of the box
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMin, yMax, zMax), Color.green);
        Debug.DrawLine(new Vector3(xMax, yMax, zMin), new Vector3(xMax, yMax, zMax), Color.green);
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMax, yMax, zMin), Color.green);
        Debug.DrawLine(new Vector3(xMin, yMax, zMax), new Vector3(xMax, yMax, zMax), Color.green);
        //draw bottom lines of box
        Debug.DrawLine(new Vector3(xMin, yMin, zMin), new Vector3(xMin, yMin, zMax), Color.green);
        Debug.DrawLine(new Vector3(xMax, yMin, zMin), new Vector3(xMax, yMin, zMax), Color.green);
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMax, yMin, zMin), Color.green);
        Debug.DrawLine(new Vector3(xMin, yMin, zMax), new Vector3(xMax, yMin, zMax), Color.green);
    }

    public BoundingBox( Vec3 a_origin, Vec3 a_extents) {
        v3Position = a_origin;
        //set extents using the Extents Property so that CalcExtents is called
        Extents = a_extents;
	}
}