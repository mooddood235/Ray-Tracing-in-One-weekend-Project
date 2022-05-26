using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Dielectric : IMaterial
{
    // Index of refraction.
    public float ir;
    public bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered){
        attenuation = Vector3.one;
        float refractionRatio = rec.frontFace ? (1f/ir) : ir;

        Vector3 unitDir = rayIn.dir.normalized;
        float cosTheta = Mathf.Min(Vector3.Dot(-unitDir, rec.normal), 1f);
        float sinTheta = Mathf.Sqrt(1f - cosTheta*cosTheta);

        bool cannotRefract = refractionRatio * sinTheta > 1f;
        Vector3 dir;

        if (cannotRefract || Reflectance(cosTheta, refractionRatio) > Random.Range(0f, 1f))
            dir = Vector3.Reflect(unitDir, rec.normal);
        else
            dir = Vector3Extensions.Refract(unitDir, rec.normal, refractionRatio);

        scattered = new Ray(rec.pos, dir);
        return true;
    }
    
    private float Reflectance(float cosine, float ref_idx){
        float r0 = (1f-ref_idx) / (1f+ref_idx);
        r0 = r0 * r0;
        return r0 + (1f-r0)* Mathf.Pow((1f - cosine), 5f);
    }
    public Dielectric(float ir){
        this.ir = ir;
    }
}
