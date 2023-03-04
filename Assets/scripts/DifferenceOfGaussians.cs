using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Processors", menuName = "Difference of Gaussians", order = 1)]
public class DifferenceOfGaussians : ImageProcessor
{
    const string SHADER_NAME = "Unlit/DoG";

    [Range(1,9)]
    public int pixelRadius;
    public float standardDev1;
    public float standardDev2;

    private Material postProcessingMaterial;

    public override void Process(RenderTexture src, RenderTexture dest)
    {
        // Apply the shader
        Graphics.Blit(src, dest, postProcessingMaterial);
    }

    public override void InitializeMaterial()
    {
        postProcessingMaterial = new Material(Shader.Find(SHADER_NAME));
        // Create matrices
        float[] matrix1 = new float[(pixelRadius * 2 + 1) * (pixelRadius * 2 + 1)];
        float[] matrix2 = new float[(pixelRadius * 2 + 1) * (pixelRadius * 2 + 1)];

        // Populate the matrices
        float sum1 = 0;
        float sum2 = 0;

        for (int y = -pixelRadius; y <= pixelRadius; y++)
        {
            for (int x = -pixelRadius; x <= pixelRadius; x++)
            {
                sum1 += matrix1[(y + pixelRadius) * (pixelRadius * 2 + 1) + x + pixelRadius] = Gaussian(x, y, standardDev1);
                sum2 += matrix2[(y + pixelRadius) * (pixelRadius * 2 + 1) + x + pixelRadius] = Gaussian(x, y, standardDev2);
            }
        }

        // Normalize Matrices
        for (int i = 0; i < pixelRadius * 2 + 1; i++)
        {
            for (int j = 0; j < pixelRadius * 2 + 1; j++)
            {
                matrix1[i * (pixelRadius * 2 + 1) + j] /= sum1;
                matrix2[i * (pixelRadius * 2 + 1) + j] /= sum2;
            }
        }

        // Pass shader data
        postProcessingMaterial.SetFloatArray("_Matrix1", matrix1);
        postProcessingMaterial.SetFloatArray("_Matrix2", matrix2);
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
