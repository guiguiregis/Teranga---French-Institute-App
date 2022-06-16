using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
    public class keyWordWords
    {
        public string[] words;
        public bool[] isRelated;
    }
    [System.Serializable]
    public class WordCloudLevelData
    {
        public WordCloudData[] level_1;
        public WordCloudData[] level_2;
    }

    [System.Serializable]
    public class WordCloudData
    {
        public string key;
        public keyWordWords value;
    }

