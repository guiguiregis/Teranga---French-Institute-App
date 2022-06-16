using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class JsonFileReader : MonoBehaviour
{
    public static string LoadJsonAsResource(string path)
    {
        string jsonFilePath = path.Replace(".json", "");
        TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);
        return loadedJsonFile.text;
    }

}
