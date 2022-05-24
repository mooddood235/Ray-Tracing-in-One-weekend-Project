using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Camera))]
public class PathTracer : MonoBehaviour
{
    private RenderTexture renderTexture;
    [SerializeField] private ComputeShader applyPixelsCompute;
    [Space]
    private PCamera pCamera;
    [SerializeField] private float aspectRatio;
    [SerializeField] private uint texWidth;
    private uint texHeight;
    [SerializeField] private float viewPortHeight;
    private float viewPortWidth;
    [SerializeField] private float focalLength;
    [Space]
    [SerializeField] private uint samples;

    private Vector3[] pixels;


    private void Awake() {
        texHeight = (uint)Mathf.RoundToInt(texWidth / aspectRatio);
        viewPortWidth = aspectRatio * viewPortHeight;
        
        Vector3 horizontal = new Vector3(viewPortWidth, 0f, 0f);
        Vector3 vertical = new Vector3(0f, viewPortHeight, 0f);
        Vector3 lowerLeftCorner = transform.position - horizontal / 2f - vertical / 2f - new Vector3(0f, 0f, focalLength);

        pCamera = new PCamera(transform.position, aspectRatio, texWidth, texHeight, viewPortWidth, viewPortHeight, focalLength, horizontal, vertical, lowerLeftCorner);

        pixels = new Vector3[texWidth * texHeight]; 
        
        renderTexture = new RenderTexture((int)texWidth, (int)texHeight, 0);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.enableRandomWrite = true;
    }

    private void Start()
    {
        HittableList scene = new HittableList();
        scene.Add(new Sphere(new Vector3(0f, 0f, -1), 0.5f));
        scene.Add(new Sphere(new Vector3(0, -100.5f, -1), 100));

        for (uint x = 0; x < texWidth; x++){
            for (uint y = 0; y < texHeight; y++){
                Vector3 pixelColor = Vector3.zero;
                for (uint s = 0; s < samples; s++){
                    float u = (x + Random.Range(0f, 0.999f)) / (texWidth - 1);
                    float v = (y + Random.Range(0f, 0.999f)) / (texHeight - 1);

                    Ray ray = new Ray(pCamera.pos, pCamera.lowerLeftCorner + u * pCamera.horizontal + v * pCamera.vertical - pCamera.pos);
                    pixelColor += GetPixelColor(ray, scene);
                }
                WriteColor(x, y,  pixelColor);
            }
        }

        DispatchApplyPixelsCompute();
    }

    private void DispatchApplyPixelsCompute(){
        ComputeBuffer pixelsCB = new ComputeBuffer(pixels.Length, 3 * sizeof(float));
        pixelsCB.SetData(pixels);
        applyPixelsCompute.SetBuffer(0, "pixels", pixelsCB);

        applyPixelsCompute.SetTexture(0, "tex", renderTexture);

        applyPixelsCompute.SetInt("texWidth", (int)texWidth);

        applyPixelsCompute.Dispatch(0, (int)texWidth, (int)texHeight, 1);
        pixelsCB.Dispose();
    }

    private void WriteColor(uint x, uint y, Vector3 color){
        color = (color / (float)samples).ClampedComponents();
        pixels[y * texWidth + x] = color;
    }

    private Vector3 GetPixelColor(Ray ray, IHittable scene){
        HitRecord rec;
        if (scene.Hit(ray, 0, Mathf.Infinity, out rec)){
            return 0.5f * (rec.normal + Vector3.one);
        }
        return SampleSkyBox(ray);
    }

    private Vector3 SampleSkyBox(Ray ray){
        Vector3 unitDir = ray.dir.normalized;
        float t = 0.5f*(unitDir.y + 1f);
        return (1f - t) * new Vector3(1f, 1f, 1f) + t * new Vector3(0.5f, 0.7f, 1f);
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(renderTexture, dest);
    }
}
