using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessor : MonoBehaviour
{
    public bool useGrayScale;
    public bool useDifferenceOfGaussians;
    public bool useGaussianBlur;
    public bool usePixelPaletteSwap;
    public bool useInvert;

    private List<IImageProcessor> usedProcessors;

    private void Start()
    {
        usedProcessors = new List<IImageProcessor>();

        if (useGrayScale)
        {
            Grayscale grayscale = new Grayscale();
            usedProcessors.Add(grayscale);
        }
        if (useDifferenceOfGaussians)
        {
            DifferenceOfGaussians differenceOfGaussians = new DifferenceOfGaussians(3, .1f, 20);
            usedProcessors.Add(differenceOfGaussians);
        }
        if (useGaussianBlur)
        {
            GaussianBlur gaussianBlur = new GaussianBlur();
            usedProcessors.Add(gaussianBlur);
        }
        if (usePixelPaletteSwap)
        {
            Color[] palette = new Color[] { Color.black, Color.gray, Color.white, Color.gray, Color.white };
            PixelPaletteSwap pixelPaletteSwap = new PixelPaletteSwap(palette);
            usedProcessors.Add(pixelPaletteSwap);
        }
        if (useInvert)
        {
            Invert invert = new Invert();
            usedProcessors.Add(invert);
        }
    }

    // Temp
    private void Update()
    {
        if (useDifferenceOfGaussians)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                ((DifferenceOfGaussians)usedProcessors[1]).SetPixelRadius(((DifferenceOfGaussians)usedProcessors[1]).pixelRadius + 1);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                ((DifferenceOfGaussians)usedProcessors[1]).SetPixelRadius(((DifferenceOfGaussians)usedProcessors[1]).pixelRadius - 1);
            Debug.Log(((DifferenceOfGaussians)usedProcessors[1]).pixelRadius);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        //differenceOfGaussians.Process(src, dest);
        //gaussianBlur.Process(temp, dest);
        //temp.Release();

        RenderTexture[] tempTextures = new RenderTexture[usedProcessors.Count + 1];
        tempTextures[0] = src;

        for (int i = 1; i < tempTextures.Length; i++)
        {
            tempTextures[i] = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        }

        for (int i = 0; i < usedProcessors.Count; i++)
        {
            usedProcessors[i].Process(tempTextures[i], tempTextures[i + 1]);
        }

        Graphics.Blit(tempTextures[tempTextures.Length - 1], dest);

        for (int i = 0; i < tempTextures.Length; i++)
        {
            RenderTexture.ReleaseTemporary(tempTextures[i]);
        }
    }
}
