using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HittableList : IHittable
{
    private List<IHittable> objects;

    public bool Hit(Ray ray, float tMin, float tMax, out HitRecord rec){
        rec = new HitRecord();
        HitRecord tempRec;
        bool hitSomething = false;
        float closestT = tMax;

        foreach (IHittable _object in objects){
            if (_object.Hit(ray, tMin, closestT, out tempRec)){
                hitSomething = true;
                closestT = tempRec.t;
                rec = tempRec;
            }
        }
        return hitSomething;
    }

    public void Add(IHittable _object){
        objects.Add(_object);
    }
    public void Clear(){
        objects.Clear();
    }

    public HittableList(object dummy){
        objects = new List<IHittable>();
    }
}
