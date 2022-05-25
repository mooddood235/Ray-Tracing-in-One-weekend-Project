using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Clamped(this Vector3 v){
        return new Vector3(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y), Mathf.Clamp01(v.z));
    }
    public static Vector3 RandomComps(float min, float max){
        return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
    }
    public static Vector3 RandomInUnitSphere(){
        while (true){
            Vector3 v = RandomComps(-1f, 1f);
            if (v.sqrMagnitude >= 1) continue;
            return v;
        }
    }
    public static Vector3 SqrtdComps(this Vector3 v){
        return new Vector3(Mathf.Sqrt(v.x), Mathf.Sqrt(v.y), Mathf.Sqrt(v.z));
    }

    public static bool NearZero(this Vector3 v){
        double s = 1e-8;
        return Mathf.Abs(v.x) < s && Mathf.Abs(v.y) < s && Mathf.Abs(v.z) < s;
    }

    public static Vector3 Multiply(Vector3 v1, Vector3 v2){
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }
    public static Vector3 Refract(Vector3 uv, Vector3 n, float etaiOveretat){
        float cosTheta = Mathf.Min(Vector3.Dot(-uv, n), 1f);
        Vector3 rOutPerp = etaiOveretat * (uv + cosTheta*n);
        Vector3 rOutParallel = -Mathf.Sqrt(Mathf.Abs(1f - rOutPerp.sqrMagnitude)) * n;
        return rOutPerp + rOutParallel;
    }
}
