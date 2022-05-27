using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class RenderThread
{

    private Vector3[] pixels;
    private uint x0;
    private uint x1;
    private uint y0;
    private uint y1;
    private uint texWidth;
    private uint texHeight;
    private PCamera pCamera;
    private uint samples;
    private uint maxDepth;
    private HittableList scene;
    private Unity.Mathematics.Random rand;
    private bool stop;


    public void Render(){
        Thread thread = new Thread(ThreadRender);
        thread.Start();
    }
    private void ThreadRender(){
        for (uint x = x0; x <= x1; x++){
            for (uint y = y0; y <= y1; y++){
                if (stop) return;
                Vector3 pixelColor = Vector3.zero;
                for (uint s = 0; s < samples; s++){
                    float u = (x + rand.NextFloat(0f, 0.999f)) / (texWidth - 1);
                    float v = (y + rand.NextFloat(0f, 0.999f)) / (texHeight - 1);

                    Ray ray = pCamera.GetRay(u, v);
                    pixelColor += GetPixelColor(ray, scene, maxDepth);
                }
                WriteColor(x, y,  pixelColor);
            }
        }
    }

    private Vector3 GetPixelColor(Ray ray, IHittable scene, uint depth){

        if (depth <= 0) return new Vector3(0f, 0f, 0f);

        HitRecord rec;
        if (scene.Hit(ray, 0.001f, Mathf.Infinity, out rec)){
            Ray scattered;
            Vector3 attenuation;

            if (rec.mat.Scatter(ray, rec, out attenuation, out scattered)){
                return Vector3Extensions.Multiply(attenuation, GetPixelColor(scattered, scene, depth - 1));
            }
            return Vector3.zero;
        }
        return SampleSkyBox(ray);
    }
    private Vector3 SampleSkyBox(Ray ray){
        Vector3 unitDir = ray.dir.normalized;
        float t = 0.5f*(unitDir.y + 1f);
        return (1f - t) * new Vector3(1f, 1f, 1f) + t * new Vector3(0.5f, 0.7f, 1f);
    }
    private void WriteColor(uint x, uint y, Vector3 color){
        color = (color / (float)samples).SqrtdComps().Clamped();
        pixels[y * texWidth + x] = color;
    }
    public void Stop(){
        stop = true;
    }

    public RenderThread(Vector3[] pixels, uint x0, uint x1, uint y0, uint y1, uint texWidth, uint texHeight, PCamera pCamera, uint samples, uint maxDepth, HittableList scene){
        this.pixels = pixels;
        this.x0 = x0;
        this.x1 = x1;
        this.y0 = y0;
        this.y1 = y1;
        this.texWidth = texWidth;
        this.texHeight = texHeight;
        this.pCamera = pCamera;
        this.samples = samples;
        this.maxDepth = maxDepth;
        this.scene = scene;
        this.stop = false;
        this.rand = new Unity.Mathematics.Random((uint)Random.Range(0, 5000));
    }
}
