using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Translate : IHittable
{
    private IHittable _object;
    private Vector3 displacement;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        ray.origin -= displacement;
        if(!_object.Hit(ray, tMin, tMax, rec)) return false;

        rec.pos += displacement;
        rec.SetFaceNormal(ray, rec.normal);
        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        if (!_object.BoundingBox(out outputBox)) return false;

        outputBox = new AABB(outputBox.minimum + displacement, outputBox.maximum + displacement);
        return true;
    }


    public Translate(IHittable _object, Vector3 displacement){
        this._object = _object;
        this.displacement = displacement;
    }
}
