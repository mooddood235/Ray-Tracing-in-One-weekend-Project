using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMaterial
{
    bool Scatter(Ray rayIn, HitRecord rec, out Vector3 attenuation, out Ray scattered);
}
