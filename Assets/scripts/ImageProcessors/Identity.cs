using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Processors", menuName = "Identity", order = 1)]
public class Identity : ImageProcessor
{
    const string SHADER_NAME = "Unlit/Identity";

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
