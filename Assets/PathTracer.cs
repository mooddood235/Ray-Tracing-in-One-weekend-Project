using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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
    [SerializeField] private float focusDist;
    [SerializeField] private float aperture;
    [Space]
    [SerializeField] private uint samples;
    [SerializeField] private uint maxDepth;
    private RenderThreadArray renderThreadArray;
    [Space]
    [SerializeField] private uint threadCount;

    private Vector3[] pixels;

    private void Awake() {
        Vector3Extensions.rand = new Unity.Mathematics.Random((uint)Random.Range(1, 5000));

        texHeight = (uint)Mathf.RoundToInt(texWidth / aspectRatio);
       
        pCamera = new PCamera(transform.position, lookat, upVector, vFov, aspectRatio, aperture, focusDist, focalLength);

        pixels = new Vector3[texWidth * texHeight]; 
        
        renderTexture = new RenderTexture((int)texWidth, (int)texHeight, 0);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.enableRandomWrite = true;
    }
    private void Start() {
        Render();
    }
    private void Update()
    {
        DispatchApplyPixelsCompute();
    }

    private void Render(){
        HittableList scene = GenerateRandomScene();
        renderThreadArray = new RenderThreadArray(threadCount, pixels, texWidth, texHeight, pCamera, samples, maxDepth, scene);
        renderThreadArray.Render();
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

    HittableList GenerateRandomScene(){
        HittableList scene = new HittableList(null);

        Lambertian groundMat = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
        scene.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, groundMat));

        for (int a = -11; a < 11; a++){
            for (int b = -11; b < 11; b++){
                float chooseMat = Random.Range(0f, 1f);
                Vector3 center = new Vector3(a + 0.9f * Random.Range(0f, 1f), 0.2f, b + 0.9f * Random.Range(0f, 1f));

                if ((center - new Vector3(4f, 0.2f, 0)).magnitude > 0.9f){
                    IMaterial mat;

                    if (chooseMat < 0.33f){
                        Vector3 albedo = Vector3Extensions.Multiply(Vector3Extensions.RandomComps(0f, 1f), Vector3Extensions.RandomComps(0f, 1f));
                        mat = new Lambertian(albedo);
                    }
                    else if (chooseMat < 0.77f){
                        Vector3 albedo = Vector3Extensions.RandomComps(0.5f, 1f);
                        float fuzz = Random.Range(0f, 0.5f);
                        mat = new Metal(albedo, fuzz);
                    }
                    else{
                        mat = new Dielectric(1.5f, (uint)Random.Range(1, 5000));
                    }
                    scene.Add(new Sphere(center, 0.2f, mat));
                }
            }
        }
        Dielectric mat1 = new Dielectric(1.5f, (uint)Random.Range(1, 5000));
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
    private void OnApplicationQuit() {
        renderThreadArray.Stop();
    }
}
