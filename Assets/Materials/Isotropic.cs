using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isotropic : Material
{
    private Vector3 albedo;

    public override bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        scattered = new Ray(rec.pos, Vector3Extensions.RandomInUnitSphere());
        attenuation = albedo;
        return true;
    }
    public Isotropic(Vector3 albedo){
        this.albedo = albedo;
    }
}
