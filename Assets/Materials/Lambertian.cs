using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Lambertian : IMaterial
{
    public Vector3 albedo;
    public bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        Vector3 scatterDir = rec.normal + Vector3Extensions.RandomInUnitSphere().normalized;

        if (scatterDir.NearZero()){
            scatterDir = rec.normal;
        }

        scattered = new Ray(rec.pos, scatterDir);
        attenuation = albedo;
        return true;
    }

    public Lambertian(Vector3 albedo){
        this.albedo = albedo;
    }
}
