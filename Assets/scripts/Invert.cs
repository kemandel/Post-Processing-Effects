using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invert : IImageProcessor
{
    const string SHADER_NAME = "Unlit/Invert";

    private Material postProcessingMaterial;

    public Invert()
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
