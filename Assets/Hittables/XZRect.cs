using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct XZRect : IHittable
{
    private float x0;
    private float x1;
    private float z0;
    private float z1;
    private float k;
    private Material mat;
    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        float t = (k - ray.origin.y) / ray.dir.y;
        if (t < tMin || t > tMax) return false;

        float x = ray.origin.x + t * ray.dir.x;
        float z = ray.origin.z + t * ray.dir.z;

        if (x < x0 || x > x1 || z < z0 || z > z1) return false;

        rec.t = t;
        Vector3 outwardNormal = new Vector3(0f, 1f, 0f);
        rec.SetFaceNormal(ray, outwardNormal);
        rec.mat = mat;
        rec.pos = ray.At(t);
        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = new AABB(new Vector3(x0, k - 0.0001f, z0), new Vector3(x1, k + 0.0001f, z1));
        return true;
    }

    public XZRect(float x0, float x1, float z0, float z1, float k, Material mat){
        this.x0 = x0;
        this.x1 = x1;
        this.z0 = z0;
        this.z1 = z1;
        this.k = k;
        this.mat = mat;
    }
}