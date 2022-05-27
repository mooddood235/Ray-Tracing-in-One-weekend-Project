using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PCamera
{
    public Vector3 pos;
    public Vector3 lookat;
    public Vector3 upVector;
    public float vFov;
    public float aspectRatio;
    public float aperture;
    public float focusDist;
    private float lensRadius;
    public float viewPortHeight;
    public float viewPortWidth;
    public float focalLength;
    public Vector3 horizontal;
    public Vector3 vertical;
    public Vector3 lowerLeftCorner;
    private Vector3 x;
    private Vector3 y;

    public Ray GetRay(float u, float v){
        Vector3 rd = lensRadius * Vector3Extensions.RandomInUnitCircle();
        Vector3 offset = x * rd.x + y * rd.y;
        return new Ray(pos + offset, lowerLeftCorner + u * horizontal + v * vertical - pos - offset);
    }

    public PCamera(Vector3 pos, Vector3 lookat, Vector3 upVector, float vFov, float aspectRatio, float aperture, float focusDist, float focalLength){
        
        this.pos = pos;
        this.vFov = vFov;
        this.lookat = lookat;
        this.upVector = upVector;

        float theta = vFov * Mathf.PI / 180f; // Degrees to radians.
        float h = Mathf.Tan(theta / 2f);
        this.viewPortHeight = 2f * h;
        this.viewPortWidth = aspectRatio * viewPortHeight;
        this.aperture = aperture;
        this.focusDist = focusDist;
        lensRadius = aperture / 2f;

        Vector3 z = (pos - lookat).normalized;
        this.x = Vector3.Cross(upVector, z).normalized;
        this.y = Vector3.Cross(z, x);

        this.aspectRatio = aspectRatio;
        this.focalLength = focalLength;
        this.horizontal = focusDist * viewPortWidth * x;
        this.vertical = focusDist * viewPortHeight * y;
        this.lowerLeftCorner = pos - horizontal / 2 - vertical / 2 - focusDist * z;      
    }
}
