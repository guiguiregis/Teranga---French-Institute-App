using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word_search_dataloader : MonoBehaviour
{
    string minigamePath = "Games\\Word_search";
    private void Awake()
    {
        string path = minigamePath + StageManager.instance.game_structure.modules[0].levels[0].games[0].data;
        WordSearch.Instance.wordpool = LoadOnePicFourWordsGameDatas(path);
    }

    public TextAsset LoadOnePicFourWordsGameDatas(string path)
    {
        TextAsset json = Resources.Load(path) as TextAsset;
        return json;
    }
}
