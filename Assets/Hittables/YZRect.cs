using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct YZRect : IHittable
{
    private float y0;
    private float y1;
    private float z0;
    private float z1;
    private float k;
    private Material mat;
    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        float t = (k - ray.origin.x) / ray.dir.x;
        if (t < tMin || t > tMax) return false;

        float y = ray.origin.y + t * ray.dir.y;
        float z = ray.origin.z + t * ray.dir.z;

        if (y < y0 || y > y1 || z < z0 || z > z1) return false;

        rec.t = t;
        Vector3 outwardNormal = new Vector3(1f, 0f, 0f);
        rec.SetFaceNormal(ray, outwardNormal);
        rec.mat = mat;
        rec.pos = ray.At(t);
        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = new AABB(new Vector3(k - 0.0001f, y0, z0), new Vector3(k + 0.0001f, y1, z1));
        return true;
    }

    public YZRect(float y0, float y1, float z0, float z1, float k, Material mat){
        this.y0 = y0;
        this.y1 = y1;
        this.z0 = z0;
        this.z1 = z1;
        this.k = k;
        this.mat = mat;
    }
}
