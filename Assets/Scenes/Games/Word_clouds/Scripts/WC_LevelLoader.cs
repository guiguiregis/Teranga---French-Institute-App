using UnityEngine;

public class WC_LevelLoader : MonoBehaviour
{

    public Word_clouds_quiz LoadLevel(string path)
    {

        string wordCloudsPath = path;

        TextAsset json = Resources.Load(wordCloudsPath) as TextAsset;

        Word_clouds_quiz loadedData = new Word_clouds_quiz();

        if(json != null)
        {
          loadedData = JsonUtility.FromJson<Word_clouds_quiz>(json.text);

        }
        else
        {
            Debug.Log("CANNOT FIND THE FILE " + wordCloudsPath);

        }

        return loadedData;


    }

}
