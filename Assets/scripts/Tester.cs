using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class Tester : MonoBehaviour
{
    /// <summary>
    /// Run the tests on start?
    /// </summary>
    public bool testScripts;

    /// <summary>
    /// Runtime per test
    /// </summary>
    public float testRuntime;

    void Start()
    {
        if (testScripts)
        {
            StartCoroutine(TestProcessorsCoroutine());
        }
    }

    IEnumerator TestProcessorsCoroutine()
    {
        yield return new WaitForSeconds(1f);
        // Get data
        PostProcessor postProcessor = FindObjectOfType<PostProcessor>();
        Text text = FindObjectOfType<Text>();
        UnityEngine.Object[] processorObjects = Resources.LoadAll("ImageProcessors");
        ImageProcessor[] processors = new ImageProcessor[processorObjects.Length];
        processorObjects.CopyTo(processors, 0);
        UnityEngine.Object[] imageObjects = Resources.LoadAll("Images");;
        Texture[] images = new Texture[imageObjects.Length];
        imageObjects.CopyTo(images, 0);
        Debug.Log(images.Length);
        processorObjects.CopyTo(processors, 0);
        List<List<int>> data = new List<List<int>>();
        // Write data to file
        string path = Application.persistentDataPath + "/test" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        StreamWriter writer = new StreamWriter(path, true);
        for (int j = 0; j < images.Length; j++)
        {
            FindObjectOfType<RawImage>().texture = images[j];
            writer.WriteLine(images[j].name);
            for (int i = 0; i < processors.Length; i++)
            {
                // Change active processor
                postProcessor.processorStack = new ImageProcessor[] { processors[i] };

                // Get data for this processor
                data.Add(new List<int>());
                float currentRuntime = 0;
                float startRuntime = Time.time;
                while (currentRuntime < testRuntime)
                {
                    int fps = Mathf.RoundToInt(1.0f / Time.deltaTime);
                    data[i].Add(fps);
                    text.text = "Image: " + images[j].name + "\nProcessing Effect: " + processors[i].name + "\nMean FPS: " + GetMean(data[i]);
                    yield return null;
                    currentRuntime = Time.time - startRuntime;
                }
                data[i].Sort();
                writer.WriteLine("\t" + processors[i].name);
                writer.WriteLine("\t\tMin:" + GetMin(data[i]));
                writer.WriteLine("\t\tMax:" + GetMax(data[i]));
                writer.WriteLine("\t\tMean:" + GetMean(data[i]));
                writer.WriteLine("\t\tMedian:" + GetMedian(data[i]));
            }
        }
        writer.Close();
        text.text = "Saved results in " + path;
    }

    int GetMedian(List<int> list)
    {
        return list[Mathf.RoundToInt(list.Count / 2)];
    }

    int GetMean(List<int> list)
    {
        int sum = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sum += list[i];
        }
        return Mathf.RoundToInt(sum / list.Count);
    }

    int GetMax(List<int> list)
    {
        return list[list.Count - 1];
    }

    int GetMin(List<int> list)
    {
        return list[0];
    }
}
