using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessor : MonoBehaviour
{
    [SerializeField]
    public ImageProcessor[] processorStack;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        //differenceOfGaussians.Process(src, dest);
        //gaussianBlur.Process(temp, dest);
        //temp.Release();


        RenderTexture[] tempTextures = new RenderTexture[processorStack.Length + 1];
        tempTextures[0] = src;

        for (int i = 1; i < tempTextures.Length; i++)
        {
            tempTextures[i] = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        }

        for (int i = 0; i < processorStack.Length; i++)
        {
            processorStack[i].Process(tempTextures[i], tempTextures[i + 1]);
        }

        Graphics.Blit(tempTextures[tempTextures.Length - 1], dest);

        for (int i = 0; i < tempTextures.Length; i++)
        {
            RenderTexture.ReleaseTemporary(tempTextures[i]);
        }
    }
}
