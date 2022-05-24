using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : IMaterial
{
    public Vector3 albedo;

    public bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        Vector3 reflected = Vector3.Reflect(rayIn.dir, rec.normal).normalized;
        scattered = new Ray(rec.pos, reflected);
        attenuation = albedo;
        return Vector3.Dot(scattered.dir, rec.normal) > 0;
    }

    public Metal(Vector3 albedo){
        this.albedo = albedo;
    }
}
