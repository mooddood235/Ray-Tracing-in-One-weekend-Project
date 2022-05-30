using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffuseLight : Material
{
    private Vector3 emission;
    public override bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        attenuation = Vector3.zero;
        scattered = new Ray();
        return false;
    }
    public override Vector3 Emitted(){
        return emission;
    }
    public DiffuseLight(Vector3 emission){
        this.emission = emission;
    }
}
