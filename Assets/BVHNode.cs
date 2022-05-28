using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BVHNode : IHittable
{
    private IHittable left;
    private IHittable right;
    private AABB box;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        if (!box.Hit(ray, tMin, tMax)) return false;
        
        bool hitLeft = left.Hit(ray, tMin, tMax, rec);
        bool hitRight = right.Hit(ray, tMin, hitLeft ? rec.t : tMax, rec);
        
        if (hitLeft || hitRight) return true;
        return false;
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = box;
        return true;
    }
    
    public BVHNode(List<IHittable> srcObjects, int start, int end){
        List<IHittable> objects = new List<IHittable>(srcObjects.Count);

        for (int i = 0; i < srcObjects.Count; i++){
            objects.Add(srcObjects[i]);
        }

        int axis = Random.Range(0, 2);
        int objectSpan = end - start;
        IComparer<IHittable> comparator;
        if (axis == 0) comparator = new AABB.boxXCompare();
        else if (axis == 1) comparator = new AABB.boxYCompare();
        else comparator = new AABB.boxZCompare();

        if (objectSpan == 1){
            left = right = objects[start];
        }
        else if (objectSpan == 2){
            if (comparator.Compare(objects[start], objects[start + 1]) == 1){
                left = objects[start];
                right = objects[start + 1];
            }
            else{
                left = objects[start + 1];
                right = objects[start];
            }
        }
        else{
            objects.Sort(start, objectSpan, comparator);

            int mid = start + objectSpan / 2;
            left = new BVHNode(objects, start, mid);
            right = new BVHNode(objects, mid, end);
        }
        
        AABB boxLeft, boxRight;

        if (!left.BoundingBox(out boxLeft) || !right.BoundingBox(out boxRight)) return;
        box = AABB.SurroundingBox(boxLeft, boxRight);
    }
}
