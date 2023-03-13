using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ImageProcessor : ScriptableObject
{
    private void OnEnable() 
    {
        InitializeMaterial();
    }

    private void OnValidate() {
        InitializeMaterial();
    }

    /// <summary>
    /// Processes a Render Texture with a shader
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public abstract void Process(RenderTexture src, RenderTexture dest);

    /// <summary>
    /// Initializes a class-level Material object according to the shader properties
    /// </summary>
    public abstract void InitializeMaterial();
}
