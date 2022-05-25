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
    [Space]
    [SerializeField] private Vector3 lookat;
    [SerializeField] private Vector3 upVector;
    [SerializeField] private float vFov;
    [SerializeField] private float focalLength;
    [Space]
    [SerializeField] private uint samples;
    [SerializeField] private uint maxDepth;

    private Vector3[] pixels;


    private void Awake() {
        texHeight = (uint)Mathf.RoundToInt(texWidth / aspectRatio);
       
        pCamera = new PCamera(transform.position, lookat, upVector, vFov, aspectRatio, focalLength);

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
        HittableList scene = GenerateRandomScene();

        for (uint x = 0; x < texWidth; x++){
            for (uint y = 0; y < texHeight; y++){
                Vector3 pixelColor = Vector3.zero;
                for (uint s = 0; s < samples; s++){
                    float u = (x + Random.Range(0f, 0.999f)) / (texWidth - 1);
                    float v = (y + Random.Range(0f, 0.999f)) / (texHeight - 1);

                    Ray ray = pCamera.GetRay(u, v);
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

    HittableList GenerateRandomScene(){
        HittableList scene = new HittableList();

        Lambertian groundMat = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
        scene.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, groundMat));

        for (int a = -11; a < 11; a++){
            for (int b = -11; b < 11; b++){
                float chooseMat = Random.Range(0f, 1f);
                Vector3 center = new Vector3(a + 0.9f * Random.Range(0f, 1f), 0.2f, b + 0.9f * Random.Range(0f, 1f));

                if ((center - new Vector3(4f, 0.2f, 0)).magnitude > 0.9f){
                    IMaterial mat;

                    if (chooseMat < 0.8f){
                        Vector3 albedo = Vector3Extensions.Multiply(Vector3Extensions.RandomComps(0f, 1f), Vector3Extensions.RandomComps(0f, 1f));
                        mat = new Lambertian(albedo);
                    }
                    else if (chooseMat < 0.95f){
                        Vector3 albedo = Vector3Extensions.RandomComps(0.5f, 1f);
                        float fuzz = Random.Range(0f, 0.5f);
                        mat = new Metal(albedo, fuzz);
                    }
                    else{
                        mat = new Dielectric(1.5f);
                    }
                    scene.Add(new Sphere(center, 0.2f, mat));
                }
            }
        }
        Dielectric mat1 = new Dielectric(1.5f);
        scene.Add(new Sphere(new Vector3(0, 1f, 0), 1f, mat1));

        Lambertian mat2 = new Lambertian(new Vector3(0.5f, 0.2f, 0.1f));
        scene.Add(new Sphere(new Vector3(-4f, 1f, 0), 1f, mat2));

        Metal mat3 = new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f);
        scene.Add(new Sphere(new Vector3(4f, 1f, 0f), 1f, mat3));

        return scene;
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(renderTexture, dest);
    }
}
