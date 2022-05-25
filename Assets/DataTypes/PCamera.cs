using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCamera
{
    public Vector3 pos;
    public Vector3 lookat;
    public Vector3 upVector;
    public float vFov;
    public float aspectRatio;
    public uint texWidth;
    public uint texHeight;
    public float viewPortHeight;
    public float viewPortWidth;
    public float focalLength;
    public Vector3 horizontal;
    public Vector3 vertical;
    public Vector3 lowerLeftCorner;

    public Ray GetRay(float u, float v){
        return new Ray(pos, lowerLeftCorner + u * horizontal + v * vertical - pos);
    }

    public PCamera(Vector3 pos, Vector3 lookat, Vector3 upVector, float vFov, float aspectRatio, float focalLength){
        
        this.pos = pos;
        this.vFov = vFov;

        float theta = vFov * Mathf.PI / 180f; // Degrees to radians.
        float h = Mathf.Tan(theta / 2f);
        this.viewPortHeight = 2f * h;
        this.viewPortWidth = aspectRatio * viewPortHeight;

        Vector3 z = (pos - lookat).normalized;
        Vector3 x = Vector3.Cross(upVector, z).normalized;
        Vector3 y = Vector3.Cross(z, x);

        this.aspectRatio = aspectRatio;
        this.focalLength = focalLength;
        this.horizontal = viewPortWidth * x;
        this.vertical = viewPortHeight * y;
        this.lowerLeftCorner = pos - horizontal / 2 - vertical / 2 - z;
    }
}
