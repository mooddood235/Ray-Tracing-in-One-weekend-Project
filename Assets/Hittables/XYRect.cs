using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct XYRect : IHittable
{
    private float x0;
    private float x1;
    private float y0;
    private float y1;
    private float k;
    private Material mat;
    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        float t = (k - ray.origin.z) / ray.dir.z;
        if (t < tMin || t > tMax) return false;

        float x = ray.origin.x + t * ray.dir.x;
        float y = ray.origin.y + t * ray.dir.y;

        if (x < x0 || x > x1 || y < y0 || y > y1) return false;

        rec.t = t;
        Vector3 outwardNormal = new Vector3(0f, 0f, 1f);
        rec.SetFaceNormal(ray, outwardNormal);
        rec.mat = mat;
        rec.pos = ray.At(t);
        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = new AABB(new Vector3(x0, y0, k - 0.0001f), new Vector3(x1, y1, k + 0.0001f));
        return true;
    }

    public XYRect(float x0, float x1, float y0, float y1, float k, Material mat){
        this.x0 = x0;
        this.x1 = x1;
        this.y0 = y0;
        this.y1 = y1;
        this.k = k;
        this.mat = mat;
    }
}
