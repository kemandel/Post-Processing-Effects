using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Processors", menuName = "Gaussian Blur", order = 1)]
public class GaussianBlur : ImageProcessor
{
    const string SHADER_NAME = "Unlit/GaussianBlur";

    [Range(1,9)]
    public int pixelRadius;
    public float standardDev;

    private Material postProcessingMaterial;

    private void OnEnable() 
    {
        InitializeMaterial();
    }

    private void OnValidate() {
        InitializeMaterial();
    }

    /// <summary>
    /// Updates the Pixel Radius used in Gaussian Calculations
    /// </summary>
    /// <param name="newPixelRadius"></param>
    public void SetPixelRadius(int newPixelRadius)
    {
        pixelRadius = newPixelRadius;
        InitializeMaterial();
    }

    /// <summary>
    /// Updates the Standard Deviation used in Gaussian Calculations
    /// </summary>
    /// <param name="newStandardDev"></param>
    public void SetStandardDeviation(float newStandardDev)
    {
        standardDev = newStandardDev;
        InitializeMaterial();
    }

    public override void Process(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, postProcessingMaterial);
    }

    public override void InitializeMaterial()
    {
        postProcessingMaterial = new Material(Shader.Find(SHADER_NAME));

        // Create matrix
        float[] matrix = new float[(pixelRadius * 2 + 1) * (pixelRadius * 2 + 1)];

        // Populate the matrix
        float sum = 0;
        for (int y = -pixelRadius; y <= pixelRadius; y++)
        {
            for (int x = -pixelRadius; x <= pixelRadius; x++)
            {
                sum += matrix[(y + pixelRadius) * (pixelRadius * 2 + 1) + x + pixelRadius] = Gaussian(x, y, standardDev);
            }
        }

        // Normalize Matrix
        for (int i = 0; i < pixelRadius * 2 + 1; i++)
        {
            for (int j = 0; j < pixelRadius * 2 + 1; j++)
            {
                matrix[i * (pixelRadius * 2 + 1) + j] /= sum;
            }
        }

        // Pass shader data
        postProcessingMaterial.SetFloatArray("_Matrix", matrix);
        postProcessingMaterial.SetFloat("_Kernel_Radius", pixelRadius);
    }

    /// <summary>
    /// Calculates the section of the matrix found at (x,y) according to the standard deviation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="stDev"></param>
    /// <returns></returns>
    private float Gaussian(int x, int y, float stDev)
    {
        float exp = -(x * x + y * y) / (2 * stDev * stDev);
        float den = 2 * Mathf.PI * stDev * stDev;
        float result = Mathf.Exp(exp) / den;
        return result;
    }
}
