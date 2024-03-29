using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sphere : IHittable
{
    public Vector3 center;
    public float radius;
    public Material mat;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        Vector3 oc = ray.origin - center;
        float a = ray.dir.sqrMagnitude;
        float halfB = Vector3.Dot(oc, ray.dir);
        float c = oc.sqrMagnitude - radius * radius;
        float discriminant = halfB * halfB - a * c;
        
        if (discriminant < 0) return false;

        float sqrtd = Mathf.Sqrt(discriminant);

        float root = (-halfB - sqrtd) / a;

        // Find the nearest root that lies in the acceptable range.
        if (root < tMin || tMax < root) {
            root = (-halfB + sqrtd) / a;
            if (root < tMin || tMax < root)
                return false;
        }

        rec.t = root;
        rec.pos = ray.At(root);
        Vector3 outwardNormal = (rec.pos - center) / radius;
        rec.SetFaceNormal(ray, outwardNormal);
        rec.mat = mat;
        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        Vector3 offset = new Vector3(radius, radius, radius);
        outputBox = new AABB(center - offset, center + offset);
        return true;
    }
    public Sphere(Vector3 center, float radius, Material mat){
        this.center = center;
        this.radius = radius;
        this.mat = mat;
    }
}
