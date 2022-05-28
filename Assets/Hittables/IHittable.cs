using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    bool Hit(Ray ray, float tMin, float tMax, HitRecord rec);
    bool BoundingBox(out AABB outputBox);
}
