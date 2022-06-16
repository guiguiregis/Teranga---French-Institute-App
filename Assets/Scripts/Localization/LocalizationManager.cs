using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

[System.Serializable]
public class LocalizationData 
{
	public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
	public string key;
	public string value;
}

public class LocalizationManager : MonoBehaviour
{
	private static Dictionary<string, string> localizedText;
	private static bool isReady = false;
	private const string missingTextString = "Localized text not found";
    static CultureInfo s_cultureInfo;

    // Use this for initialization
    void Awake () 
	{
		Init();
	}

	public static void Init()
	{
		if (!GetIsReady())
		{
            string languageCode;
            s_cultureInfo = new CultureInfo("en-US");

            switch (Application.systemLanguage)
            {
                case SystemLanguage.French:
                    languageCode = "fr";
                    s_cultureInfo = new CultureInfo("fr-FR");
                    break;

                default:
                    languageCode = "en";
                    break;
            }
            
            LocalizationManager.LoadLocalizedText(languageCode);
		}
	}

	public static void LoadLocalizedText(string fileName)
	{
		localizedText = new Dictionary<string, string> ();
        string path = "Localization" + Path.DirectorySeparatorChar + fileName; // + ".json";

        TextAsset json = Resources.Load(path) as TextAsset;

        if (json != null)
        {
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (json.text);

			for (int i = 0; i < loadedData.items.Length; i++) 
			{
				localizedText.Add(loadedData.items [i].key, loadedData.items [i].value);   
			}

			Debug.Log ("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find file: " + path);
        }

        isReady = true;
	}

	public static string GetLocalizedValue(string key)
	{
		string result = missingTextString;
		if (localizedText.ContainsKey(key)) 
		{
			result = localizedText[key];
		}

		return result;

	}

	public static bool GetIsReady()
	{
		return isReady;
	}

    static public string FormatInteger(int _originalValue)
    {
        return _originalValue.ToString("N0", s_cultureInfo);
    }
}