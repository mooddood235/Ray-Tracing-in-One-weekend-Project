using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lambertian : Material
{
    public Vector3 albedo;
    public override bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        Vector3 scatterDir = rec.normal + Vector3Extensions.RandomInUnitSphere().normalized;

        if (scatterDir.NearZero()){
            scatterDir = rec.normal;
        }

        scattered = new Ray(rec.pos, scatterDir);
        attenuation = albedo;
        return true;
    }
    public override Vector3 GetAlbedo(){
        return albedo;
    }
    public Lambertian(Vector3 albedo){
        this.albedo = albedo;
    }
}
