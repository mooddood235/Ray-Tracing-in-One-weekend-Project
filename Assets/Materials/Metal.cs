using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : Material
{
    public Vector3 albedo;
    public float fuzz;

    public override bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        Vector3 reflected = Vector3.Reflect(rayIn.dir, rec.normal).normalized;
        scattered = new Ray(rec.pos, reflected + fuzz * Vector3Extensions.RandomInUnitSphere());
        attenuation = albedo;
        return Vector3.Dot(scattered.dir, rec.normal) > 0;
    }
    public override Vector3 GetAlbedo(){
        return albedo;
    }
    public Metal(Vector3 albedo, float fuzz){
        this.albedo = albedo;
        this.fuzz = fuzz;
    }
}
