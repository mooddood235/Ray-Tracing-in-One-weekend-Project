using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateY : IHittable
{
    private IHittable _object;
    private float sinTheta;
    private float cosTheta;
    private bool hasBox;
    private AABB bBox;

    public bool Hit(Ray ray, float tMin, float tMax, HitRecord rec){
        Vector3 origin = ray.origin;
        Vector3 dir = ray.dir;

        origin[0] = cosTheta*ray.origin[0] - sinTheta*ray.origin[2];
        origin[2] = sinTheta*ray.origin[0] + cosTheta*ray.origin[2];

        dir[0] = cosTheta*ray.dir[0] - sinTheta*ray.dir[2];
        dir[2] = sinTheta*ray.dir[0] + cosTheta*ray.dir[2];

        Ray rotatedRay = new Ray(origin, dir);

        if (!_object.Hit(rotatedRay, tMin, tMax, rec)) return false;

        Vector3 pos = rec.pos;
        Vector3 normal = rec.normal;

        pos[0] =  cosTheta*rec.pos[0] + sinTheta*rec.pos[2];
        pos[2] = -sinTheta*rec.pos[0] + cosTheta*rec.pos[2];

        normal[0] =  cosTheta*rec.normal[0] + sinTheta*rec.normal[2];
        normal[2] = -sinTheta*rec.normal[0] + cosTheta*rec.normal[2];

        rec.pos = pos;
        rec.SetFaceNormal(rotatedRay, normal);

        return true;
    }
    public bool BoundingBox(out AABB outputBox){
        outputBox = bBox;
        return hasBox;
    }


    public RotateY(IHittable _object, float angle){
        this._object = _object;
        
        float radians = angle * Mathf.PI / 180f;
        sinTheta = Mathf.Sin(radians);
        cosTheta = Mathf.Cos(radians);
        hasBox = _object.BoundingBox(out bBox);

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                for (int k = 0; k < 2; k++) {
                    float x = i*bBox.maximum.x + (1-i)*bBox.minimum.x;
                    float y = j*bBox.maximum.y + (1-j)*bBox.minimum.y;
                    float z = k*bBox.maximum.z + (1-k)*bBox.minimum.z;

                    float newx =  cosTheta*x + sinTheta*z;
                    float newz = -sinTheta*x + cosTheta*z;

                    Vector3 tester = new Vector3(newx, y, newz);

                    for (int c = 0; c < 3; c++) {
                        min[c] = Mathf.Min(min[c], tester[c]);
                        max[c] = Mathf.Max(max[c], tester[c]);
                    }
                }
            }
        }
        bBox = new AABB(min, max);
    }
}
