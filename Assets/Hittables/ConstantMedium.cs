using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ConstantMedium : IHittable
{
    private IHittable boundary;
    private Material phaseFunction;
    private float negInvDensity;

    private Unity.Mathematics.Random rand;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        HitRecord rec1 = new HitRecord();
        HitRecord rec2 = new HitRecord();

        if (!boundary.Hit(ray, Mathf.NegativeInfinity, Mathf.Infinity, rec1)) return false;
        if (!boundary.Hit(ray, rec1.t + 0.0001f, Mathf.Infinity, rec2)) return false;

        if (rec1.t < tMin) rec1.t = tMin;
        if (rec2.t > tMax) rec2.t = tMax;

        if (rec1.t >= rec2.t) return false;

        if (rec1.t < 0f) rec1.t = 0f;

        float rayLength = ray.dir.magnitude;
        float distanceInsideBoundary = (rec2.t - rec1.t) * rayLength;
        float hitDistance = negInvDensity * Mathf.Log(rand.NextFloat(0f, 1f));

        if (hitDistance > distanceInsideBoundary) return false;

        rec.t = rec1.t + hitDistance / rayLength;
        rec.pos = ray.At(rec.t);
        rec.normal = Vector3.right; // arbitrary
        rec.frontFace = true; // arbitrary
        rec.mat = phaseFunction;
        
        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        return boundary.BoundingBox(out outputBox);
    }

    public ConstantMedium(IHittable boundary, float density, Material phaseFunction, uint randSeed){
        this.boundary = boundary;
        this.negInvDensity = -1f / density;
        this.phaseFunction = phaseFunction;
        rand = new Unity.Mathematics.Random(randSeed);
    }
}
