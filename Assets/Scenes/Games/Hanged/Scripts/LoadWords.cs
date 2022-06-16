using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadWords : MonoBehaviour
{
    private void Awake()
    {
        LoadWordsData();
    }

    public void LoadWordsData()
    {

        string path = "HangedData" + Path.DirectorySeparatorChar + "Words.JSON"; 
        TextAsset json = Resources.Load(path) as TextAsset;


        if (json != null)
        {
            List<WordsList> loadedData = JsonUtility.FromJson<List<WordsList>>(json.text);
            Debug.Log(loadedData);
        }
        else
        {
            Debug.LogError("Cannot find file: " + path);
        }
    }
}
