using System.Collections.Generic;
using UnityEngine;

public class One_pic_four_words_dataloader : MonoBehaviour
{

    public Dictionary<string, keyWordValue> level1_onePicFourWordsData = new Dictionary<string, keyWordValue>();
    public Dictionary<string, keyWordValue> level2_onePicFourWordsData = new Dictionary<string, keyWordValue>();
    string minigamePath = "Games\\One_pic_four_words\\";
    string jsonFileName = "\\OnePicFourWordsDatas";
    string defaultGameDataPath = "Games\\One_pic_four_words\\Leadership\\Level_3\\OnePicFourWordsDatas";

    private void Awake()
    {
        string gamePath = "";
        if(StageManager.instance != null)
        {
            int levelIndex = StageManager.instance.GetCurrentLevelIndex() + 1; // StageManager.instance.GetInt("current_level_index") + 1;//;
            string levelName = "\\Level_" + levelIndex;
            gamePath = minigamePath + StageManager.instance.GetString("current_module_name") + levelName + jsonFileName;
            Debug.Log(gamePath);
        }
        else
        {
            gamePath = defaultGameDataPath;
        }

        LoadOnePicFourWordsGameDatas(gamePath);
        InitializeMinigameData();
    }
    public void LoadOnePicFourWordsGameDatas(string path)
    {
        TextAsset json = Resources.Load(path) as TextAsset;
        if (json != null)
        {
            OnePicFourWordsLevel loadedData = JsonUtility.FromJson<OnePicFourWordsLevel>(json.text);

            for (int i = 0; i < loadedData.level_1.Length; i++)
            {
                level1_onePicFourWordsData.Add(loadedData.level_1[i].key, loadedData.level_1[i].value);
            }
        }
        else
        {
            Debug.Log("CANNOT FIND THE FILE " + path);
        }
    }

    public void InitializeMinigameData()
    {
        
            foreach (keyWordValue v in level1_onePicFourWordsData.Values)
            {
                OnePicFourWordsGameLevel m = new OnePicFourWordsGameLevel();

                m.m_levelImage = Resources.Load<Sprite>(v.imageUrl);
                m.keyword = v.keyword;
                m.gameDifficulty = v.gameDifficulty;
                for (int j = 0; j < v.words.Length; j++)
                {
                    m.m_fourWords.Add(v.words[j]);
                }
                GenerateImage.Instance.m_onePicFourWordsGameLevels.Add(m);
            }
    }
}
