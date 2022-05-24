using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCamera
{
    public Vector3 pos;

    public float aspectRatio;
    public uint texWidth;
    public uint texHeight;
    public float viewPortHeight;
    public float viewPortWidth;
    public float focalLength;
    public Vector3 horizontal;
    public Vector3 vertical;
    public Vector3 lowerLeftCorner;

    public PCamera(Vector3 pos, float aspectRatio, uint texWidth, uint texHeight, float viewPortWidth, float viewPortHeight, float focalLength,
                   Vector3 horizontal, Vector3 vertical, Vector3 lowerLeftCorner){
        
        this.pos = pos;
        this.aspectRatio = aspectRatio;
        this.texWidth = texWidth;
        this.texHeight = texHeight;
        this.viewPortWidth = viewPortWidth;
        this.viewPortHeight = viewPortHeight;
        this.focalLength = focalLength;
        this.horizontal = horizontal;
        this.vertical = vertical;
        this.lowerLeftCorner = lowerLeftCorner;
    }
}
