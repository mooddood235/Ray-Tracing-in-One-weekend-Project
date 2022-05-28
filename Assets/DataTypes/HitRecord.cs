using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRecord
{
    public Vector3 pos;
    public Vector3 normal;
    public float t;
    public bool frontFace;
    public IMaterial mat;

    public void SetFaceNormal(Ray ray, Vector3 outwardNormal){
        frontFace = Vector3.Dot(ray.dir, outwardNormal) < 0;

        if (frontFace){
            normal = outwardNormal;
        }
        else{
            normal = -outwardNormal;
        }
    }
}
