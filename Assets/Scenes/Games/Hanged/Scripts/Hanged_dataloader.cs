using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hanged_dataloader : MonoBehaviour
{
    string minigamePath = "Games\\Hanged\\";
    public Hanged_struct m_hangedLevelsData;

    private void Awake()
    {
        
        string path = minigamePath + PlayerPrefs.GetString("current_module_name")+"\\Levels\\HangedLevelsData";
        int gameDifficulty = 1;
        if (StageManager.instance != null)
            gameDifficulty = StageManager.instance.GetCurrentLevelIndex() + 1;
        LoadHangedMiniGameLevel(path, gameDifficulty);
    }

    public void LoadHangedMiniGameLevel(string path, int levelIndex)
    {
        string myLoadedGameLevel = JsonFileReader.LoadJsonAsResource(path);
        // Debug.Log(myLoadedGameLevel);
        m_hangedLevelsData = JsonUtility.FromJson<Hanged_struct>(myLoadedGameLevel);
        InitialiseLevelsData(levelIndex, m_hangedLevelsData);
    }

    public void InitialiseLevelsData(int levelIndex, Hanged_struct _hangedLevelsData )
    {
        string currDifficulty = StageManager.instance.GetGameCurrentDifficulty();
        Hanged_difficulty hangedData = new Hanged_difficulty();
        hangedData = GetHangedDifficulty(levelIndex, _hangedLevelsData.m_difficulties); //verify which difficulty has the current level before loading it
        
        switch (levelIndex)
        {
            case 1:
                GenerateRandomWord.Instance.m_words = hangedData.levelOne;
                break;
            case 2:
                GenerateRandomWord.Instance.m_words = hangedData.levelTwo;
                break;
            case 3:
                GenerateRandomWord.Instance.m_words = hangedData.levelThree;
                break;
            case 4:
                GenerateRandomWord.Instance.m_words = hangedData.levelFour;
                break;
            case 5:
                GenerateRandomWord.Instance.m_words = hangedData.levelFive;
                break;
            case 6:
                GenerateRandomWord.Instance.m_words = hangedData.levelSix;
                break;
            default:
                break;
        }
    }




    public Hanged_difficulty GetHangedDifficulty(int index, List<Hanged_difficulty> _difficulties)
    {

        switch (index)
        {
            case 1:
                return _difficulties.Where(d => d.levelOne != null && d.levelOne.Count > 0).FirstOrDefault();
                break;
            case 2:
                return _difficulties.Where(d => d.levelTwo != null && d.levelTwo.Count > 0).FirstOrDefault();
                break;
            case 3:
                return _difficulties.Where(d => d.levelThree != null && d.levelThree.Count > 0).FirstOrDefault();
                break;
            case 4:
                return _difficulties.Where(d => d.levelFour != null && d.levelFour.Count > 0).FirstOrDefault();
                break;
            case 5:
                return _difficulties.Where(d => d.levelFive != null && d.levelFive.Count > 0).FirstOrDefault();
                break;
            case 6:
                return _difficulties.Where(d => d.levelSix != null && d.levelSix.Count > 0).FirstOrDefault();
                break;
            default:
                return null;
                break;
        }
    }

}
