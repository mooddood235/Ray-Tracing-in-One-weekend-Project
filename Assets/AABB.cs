using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AABB
{
    public Vector3 minimum;
    public Vector3 maximum;

    public bool Hit(Ray ray, float tMin, float tMax){
        for (int a = 0; a < 3; a++) {
            float invD = 1.0f / ray.dir[a];
            float t0 = (minimum[a] - ray.origin[a]) * invD;
            float t1 = (maximum[a] - ray.origin[a]) * invD;
            if (invD < 0.0f)
                (t0, t1) = (t1, t0);
            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;
            if (tMax <= tMin)
                return false;
        }
        return true;
    }

    public static AABB SurroundingBox(AABB box0, AABB box1){
        Vector3 small = new Vector3(Mathf.Min(box0.minimum.x, box1.minimum.x),
                 Mathf.Min(box0.minimum.y, box1.minimum.y),
                 Mathf.Min(box0.minimum.z, box1.minimum.z));

        Vector3 big = new Vector3(Mathf.Max(box0.maximum.x, box1.maximum.x),
                Mathf.Max(box0.maximum.y, box1.maximum.y),
                Mathf.Max(box0.maximum.z, box1.maximum.z));

        return new AABB(small,big);
    }
    public static bool BoxCompare(IHittable a, IHittable b, int axis){
        AABB boxA;
        AABB boxB;

        if (!a.BoundingBox(out boxA) || !b.BoundingBox(out boxB)){
            Debug.LogError("No bounding box in BVHNode constructor.");
            return false;
        }
        return boxA.minimum[axis] < boxB.minimum[axis];
    }
    public AABB(Vector3 minimum, Vector3 maximum){
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public class boxXCompare : IComparer<IHittable>{
        public int Compare(IHittable a, IHittable b)
        {
            bool result = BoxCompare(a, b, 0);
            if (result) return 1;
            return 0;
        }
    }
    public class boxYCompare : IComparer<IHittable>{
        public int Compare(IHittable a, IHittable b)
        {
            bool result = BoxCompare(a, b, 1);
            if (result) return 1;
            return 0;
        }
    }
    public class boxZCompare : IComparer<IHittable>{
        public int Compare(IHittable a, IHittable b)
        {
            bool result = BoxCompare(a, b, 2);
            if (result) return 1;
            return 0;
        }
    }
}
