using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray
{
    public Vector3 origin;
    public Vector3 dir;

    public Vector3 At(float t){
        return origin + t * dir;
    }

    public Ray(Vector3 origin, Vector3 dir){
        this.origin = origin;
        this.dir = dir;
    }
}
