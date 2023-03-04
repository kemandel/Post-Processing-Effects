using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Processors", menuName = "Grayscale", order = 1)]
public class Grayscale : ImageProcessor
{
    const string SHADER_NAME = "Unlit/Grayscale";

    private Material postProcessingMaterial;

    private void OnEnable()
    {
        InitializeMaterial();
    }

    public override void Process(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, postProcessingMaterial);
    }

    public override void InitializeMaterial()
    {
        postProcessingMaterial = new Material(Shader.Find(SHADER_NAME));
    }
}
