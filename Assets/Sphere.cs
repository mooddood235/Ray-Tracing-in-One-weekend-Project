using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : IHittable
{
    public Vector3 center;
    public float radius;

    public bool Hit(Ray ray, float tMin, float tMax, out HitRecord rec){
        rec = new HitRecord();

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
        return true;
    }

    public Sphere(Vector3 center, float radius){
        this.center = center;
        this.radius = radius;
    }
}
