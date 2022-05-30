using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Material
{
    public abstract bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered);
    public virtual Vector3 Emitted(){
        return Vector3.zero;
    }
    public virtual Vector3 GetAlbedo(){
        return Vector3.zero;
    }
}
