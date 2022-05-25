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
    [SerializeField] private uint maxDepth;

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
    private void Start() {
        StartCoroutine(Render());
    }
    private void Update()
    {
        DispatchApplyPixelsCompute();
    }

    private IEnumerator Render(){
        HittableList scene = new HittableList();

        Lambertian matGround = new Lambertian(new Vector3(0.8f, 0.8f, 0f));
        Lambertian matCenter = new Lambertian(new Vector3(0.1f, 0.2f, 0.5f));
        Dielectric matLeft = new Dielectric(1.5f);
        Metal matRight = new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0f);

        scene.Add(new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f, matCenter));
        scene.Add(new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f, matGround));
        scene.Add(new Sphere(new Vector3(-1.0f, 0.0f, -1.0f), 0.4f, matLeft));
        scene.Add(new Sphere(new Vector3(1.0f, 0.0f, -1.0f), 0.5f, matRight));

        for (uint x = 0; x < texWidth; x++){
            for (uint y = 0; y < texHeight; y++){
                Vector3 pixelColor = Vector3.zero;
                for (uint s = 0; s < samples; s++){
                    float u = (x + Random.Range(0f, 0.999f)) / (texWidth - 1);
                    float v = (y + Random.Range(0f, 0.999f)) / (texHeight - 1);

                    Ray ray = new Ray(pCamera.pos, pCamera.lowerLeftCorner + u * pCamera.horizontal + v * pCamera.vertical - pCamera.pos);
                    pixelColor += GetPixelColor(ray, scene, maxDepth);
                }
                WriteColor(x, y,  pixelColor);
            }
            yield return null;
        }
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
        color = (color / (float)samples).SqrtdComps().Clamped();
        pixels[y * texWidth + x] = color;
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
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(renderTexture, dest);
    }
}
