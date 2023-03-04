using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPaletteSwap : IImageProcessor
{
    const string SHADER_NAME = "Hidden/PixelSwap";

    public Color[] palette { get; private set; }

    private Material postProcessingMaterial;

    public PixelPaletteSwap()
    {
        // By defualt use a palette of just black and white
        palette = new Color[] {new Color(255,255,255,0), new Color(0,0,0,1)};

        InitializeMaterial();
    }

    public PixelPaletteSwap(Color[] _palette)
    {
        palette = _palette;

        InitializeMaterial();
    }

    /// <summary>
    /// Adds a color to the palette
    /// </summary>
    /// <param name="color"></param>
    public void AddToPalette(Color color)
    {
        List<Color> tempPalette = new List<Color>(palette);
        tempPalette.Add(color);
        palette = tempPalette.ToArray();

        InitializeMaterial();
    }

    /// <summary>
    /// Changes the palette to the new palette
    /// </summary>
    /// <param name="newPalette"></param>
    public void NewPalette(Color[] newPalette)
    {
        palette = newPalette;
        
        InitializeMaterial();
    }

    public void Process(RenderTexture src, RenderTexture dest)
    {
        // Apply the shader
        Graphics.Blit(src, dest, postProcessingMaterial);
    }

    public void InitializeMaterial()
    {
        postProcessingMaterial = new Material(Shader.Find(SHADER_NAME));
        List<Vector4> initPalette = new List<Vector4>(new Vector4[255]);
        postProcessingMaterial.SetVectorArray("_Colors", initPalette);

        List<Vector4> unsortedPalette = new List<Vector4>();
        List<Vector4> sortedPalette = new List<Vector4>();

        // Convert the color array to vectors
        for (int i = 0; i < palette.Length; i++)
        {
            unsortedPalette.Add(palette[i]);
        }

        // For each color in the palette..
        for (int i = palette.Length - 1; i >= 0; i--)
        {
            // Compare each color's luminance
            Color color_1 = unsortedPalette[i];
            for (int k = 0; k < i; k++)
            {
                Color color_2 = unsortedPalette[k];
                if ((0.299 * color_1.r + 0.587 * color_1.g + 0.114 * color_1.b) > (0.299 * color_2.r + 0.587 * color_2.g + 0.114 * color_2.b))
                {
                    color_1 = color_2;
                }
            }

            // Add the brightest color to sorted
            sortedPalette.Add(color_1);
            // Remove that color from unsorted
            unsortedPalette.Remove(color_1);
        }

        // Send the sorted array to the shader
        postProcessingMaterial.SetFloat("_Colors_Amount", palette.Length);
        postProcessingMaterial.SetVectorArray("_Colors", sortedPalette);
    }
}
