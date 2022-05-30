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
    [SerializeField] private RenderMode renderMode;
    [SerializeField] private uint samples;
    [SerializeField] private uint maxDepth;
    private RenderThreadArray renderThreadArray;
    [Space]
    [SerializeField] private uint threadCount;
    [SerializeField] private string path;

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
        List<IHittable> objects = GenerateCornellBox();
        BVHNode scene = new BVHNode(objects, 0, objects.Count);
        renderThreadArray = new RenderThreadArray(
        threadCount, pixels, texWidth, texHeight, pCamera, samples, maxDepth, scene, renderMode);
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

    List<IHittable> GenerateRandomScene(){
        List<IHittable> scene = new List<IHittable>();

        Lambertian groundMat = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
        scene.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, groundMat));

        for (int a = -11; a < 11; a++){
            for (int b = -11; b < 11; b++){
                float chooseMat = Random.Range(0f, 1f);
                Vector3 center = new Vector3(a + 0.9f * Random.Range(0f, 1f), 0.2f, b + 0.9f * Random.Range(0f, 1f));

                if ((center - new Vector3(4f, 0.2f, 0)).magnitude > 0.9f){
                    Material mat;

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
    List<IHittable> GenerateRandomSceneWithLight(){
        List<IHittable> objects = new List<IHittable>();
        Lambertian groundMat = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
        //Lambertian centerPieceMat = new Lambertian(new Vector3(0.4f, 0.2f, 0.7f));
        //Metal centerPieceMat = new Metal(new Vector3(0.5f, 0.2f, 0.7f), 0);
        Dielectric centerPieceMat = new Dielectric(1.4f, (uint)Random.Range(1, 5000));
        objects.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, groundMat));
        objects.Add(new Sphere(new Vector3(0f, 2f, 0f), 2f, centerPieceMat));

        DiffuseLight lightMat = new DiffuseLight(new Vector3(4f, 4f, 4f));
        objects.Add(new XYRect(3, 5, 1, 3, -2, lightMat));
        objects.Add(new Sphere(new Vector3(0f, 8f, 0f), 2f, lightMat));
        return objects;
    }
    List<IHittable> GenerateCornellBox(){
        List<IHittable> objects = new List<IHittable>();

        Lambertian red   = new Lambertian(new Vector3(0.65f, 0.05f, 0.05f));
        Lambertian white = new Lambertian(new Vector3(0.73f, 0.73f, 0.73f));
        Lambertian green = new Lambertian(new Vector3(0.12f, 0.45f, 0.15f));
        DiffuseLight light = new DiffuseLight(new Vector3(7f, 7f, 7f));

        objects.Add(new YZRect(0, 555, 0, 555, 555, green));
        objects.Add(new YZRect(0, 555, 0, 555, 0, red));
        objects.Add(new XZRect(113, 443, 127, 432, 554, light));
        objects.Add(new XZRect(0, 555, 0, 555, 0, white));
        objects.Add(new XZRect(0, 555, 0, 555, 555, white));
        objects.Add(new XYRect(0, 555, 0, 555, 555, white));

        IHittable box1 = new Box(new Vector3(0, 0, 0), new Vector3(165, 330, 165), white);
        box1 = new RotateY(box1, 15);
        box1 = new Translate(box1, new Vector3(265,0,295));

        IHittable box2 = new Box(new Vector3(0,0,0), new Vector3(165,165,165), white);
        box2 = new RotateY(box2, -18);
        box2 = new Translate(box2, new Vector3(130,0,65));

        objects.Add(new ConstantMedium(box1, 0.01f, new Isotropic(Vector3.zero), (uint)Random.Range(1, 5000)));
        objects.Add(new ConstantMedium(box2, 0.01f, new Isotropic(Vector3.one), (uint)Random.Range(1, 5000)));
        return objects;
    }
    private void SaveToPNG(){
        if (path == "") return;
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D((int)texWidth, (int)texHeight);
        tex.ReadPixels(new Rect(0, 0, texWidth, texHeight), 0, 0);
        RenderTexture.active = null;

        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(renderTexture, dest);
    }
    private void OnApplicationQuit() {
        SaveToPNG();
        renderThreadArray.Stop(); 
    }
    public enum RenderMode{
    Default,
    Normals,
    Albedo
    }   
}


