using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderThreadArray
{
    private uint threadCount;
    RenderThread[] renderThreads;
    private Vector3[] pixels;
    private uint texWidth;
    private uint texHeight;
    private PCamera pCamera;
    private uint samples;
    private uint maxDepth;
    private HittableList scene;

    public void Render(){
        foreach (RenderThread renderThread in renderThreads){
            renderThread.Render();
        }
    }
    public void Stop(){
        foreach (RenderThread renderThread in renderThreads){
            renderThread.Stop();
        }
    }

    private void CreateRenderThreads(){
        renderThreads = new RenderThread[threadCount];
        uint segment = texWidth / threadCount;
        for (uint i = 0; i < threadCount; i++){
            renderThreads[i] = new RenderThread(
            pixels, i * segment, (i + 1) * segment - 1, 0, texHeight - 1, texWidth, texHeight, pCamera, samples, maxDepth, scene);
        }
    }
    public RenderThreadArray(uint threadCount, Vector3[] pixels, uint texWidth, uint texHeight, PCamera pCamera, uint samples, uint maxDepth, HittableList scene){
        this.threadCount = threadCount;
        this.pixels = pixels;
        this.texWidth = texWidth;
        this.texHeight = texHeight;
        this.pCamera = pCamera;
        this.samples = samples;
        this.maxDepth = maxDepth;
        this.scene = scene;
        CreateRenderThreads();
    }
}
