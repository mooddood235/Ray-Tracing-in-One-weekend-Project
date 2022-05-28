using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HittableList : IHittable
{
    private List<IHittable> objects;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        bool hitSomething = false;
        float closestT = tMax;

        foreach (IHittable _object in objects){
            if (_object.Hit(ray, tMin, closestT, rec)){
                hitSomething = true;
                closestT = rec.t;
            }
        }
        return hitSomething;
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = new AABB(Vector3.zero, Vector3.zero);
        if (objects.Count == 0) return false;

        AABB tempBox;
        bool firstBox = true;

        foreach (IHittable _object in objects){
            if (!_object.BoundingBox(out tempBox)) return false;
            outputBox = firstBox ? tempBox : AABB.SurroundingBox(outputBox, tempBox);
            firstBox = false;
        }
        return true;
    }
    public void Add(IHittable _object){
        objects.Add(_object);
    }
    public void Clear(){
        objects.Clear();
    }

    public HittableList(int dummy){
        objects = new List<IHittable>();
    }
    public HittableList(List<IHittable> objects){
        this.objects = new List<IHittable>();
        for (int i = 0; i < objects.Count; i++){
            this.objects.Add(objects[i]);
        }
    }
}
