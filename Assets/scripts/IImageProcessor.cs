using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImageProcessor
{
    /// <summary>
    /// Processes a Render Texture with a shader
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public void Process(RenderTexture src, RenderTexture dest);

    /// <summary>
    /// Initializes a class-level Material object according to the shader properties
    /// </summary>
    public void InitializeMaterial();
}
