using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vec3 {
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    public Vec3() {
        x = 0f; y = 0f; z = 0f;
    }

    public Vec3(float a_x, float a_y, float a_z) {
        x = a_x; y = a_y; z = a_z;
    }

    public Vec3(Vector3 a_v3) {
        x = a_v3.x; y = a_v3.y; z = a_v3.z;
	}

    public Vector3 ToVector3() {
        return new Vector3(x, y, z);
    }

    public static Vec3 operator +(Vec3 a, Vec3 b) {
        return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vec3 operator -(Vec3 a, Vec3 b) {
        return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vec3 operator *(Vec3 a, Vec3 b) {
        return new Vec3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vec3 operator *(Vec3 a, float s) {
        return new Vec3(a.x * s, a.y * s, a.z * s);
    }

    public static Vec3 operator *(float s, Vec3 a) {
        return new Vec3(a.x * s, a.y * s, a.z * s);
    }

    public static Vec3 operator -(Vec3 a) {
        return new Vec3(-a.x, -a.y, -a.z);
    }

    public float Magnitude() {
        return Mathf.Sqrt(x * x + y * y + z * z);
    }

    public float MagnitudeSquared() {
        return x * x + y * y + z * z;
    }

    public float Dot(Vec3 a_b) {
        return x * a_b.x + y * a_b.y + z * a_b.z;
    }

    public static float DotProduct(Vec3 a, Vec3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public float Normalize() {
        float mag = Magnitude();
        float fInvMag = (mag != 0f) ? 1f / mag : 1.00e-12f;
        x *= fInvMag;
        y *= fInvMag;
        z *= fInvMag;
        return mag;
    }

    public Vec3 Prep() {
        return new Vec3(-y, x, z);
    }

    public static Vec3 CrossProduct(Vec3 a, Vec3 b) {
        return new Vec3(a.y * b.z - a.z * b.z,
                         a.z * b.z - a.x * b.z,
                         a.x * b.y - a.y * b.x);
    }

    public void RotateZ(float angle) {
        float fX = x;
        x = fX + Mathf.Cos(angle) - y * Mathf.Sin(angle);
        y = fX * Mathf.Sin(angle) + y * Mathf.Cos(angle);
    }

    public void RotateY(float angle) {
        float fX = x;
        x = fX + Mathf.Cos(angle) - z * Mathf.Sin(angle);
        y = fX * Mathf.Sin(angle) + z * Mathf.Cos(angle);
    }

    public void RotateX(float angle) {
        float fY = y;
        x = fY + Mathf.Cos(angle) - y * Mathf.Sin(angle);
        y = fY * Mathf.Sin(angle) + y * Mathf.Cos(angle);
    }
}
