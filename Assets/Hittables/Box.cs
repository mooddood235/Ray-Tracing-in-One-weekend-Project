using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Box : IHittable
{
    private Vector3 min;
    private Vector3 max;
    private HittableList sides;
    private Material mat;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        return sides.Hit(ray, tMin, tMax, rec);
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = new AABB(min, max);
        return true;
    }

    public Box(Vector3 min, Vector3 max, Material mat){
        this.min = min;
        this.max = max;
        this.mat = mat;
        this.sides = new HittableList(0);

        sides.Add(new XYRect(min.x, max.x, min.y, max.y, max.z, mat));
        sides.Add(new XYRect(min.x, max.x, min.y, max.y, min.z, mat));

        sides.Add(new XZRect(min.x, max.x, min.z, max.z, max.y, mat));
        sides.Add(new XZRect(min.x, max.x, min.z, max.z, min.y, mat));

        sides.Add(new YZRect(min.y, max.y, min.z, max.z, max.x, mat));
        sides.Add(new YZRect(min.y, max.y, min.z, max.z, min.x, mat));
    }
}
