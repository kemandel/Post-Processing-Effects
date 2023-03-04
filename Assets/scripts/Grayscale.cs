using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grayscale : IImageProcessor
{
    const string SHADER_NAME = "Unlit/Grayscale";

    private Material postProcessingMaterial;

    public Grayscale()
    {
        InitializeMaterial();
    }

    public void Process(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, postProcessingMaterial);
    }

    public void InitializeMaterial()
    {
        postProcessingMaterial = new Material(Shader.Find(SHADER_NAME));
    }
}
