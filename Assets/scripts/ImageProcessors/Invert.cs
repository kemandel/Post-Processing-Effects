using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Processors", menuName = "Invert", order = 1)]
public class Invert : ImageProcessor
{
    const string SHADER_NAME = "Unlit/Invert";

    private Material postProcessingMaterial;

    public override void Process(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, postProcessingMaterial);
    }

    public override void InitializeMaterial()
    {
        postProcessingMaterial = new Material(Shader.Find(SHADER_NAME));
    }
}
