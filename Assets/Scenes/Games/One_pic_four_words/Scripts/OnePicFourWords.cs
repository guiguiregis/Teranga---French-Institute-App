using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//class for the one pic four words level

    [System.Serializable]
    public class OnePicFourWordsLevel
    {
        public OnePicFourWordsData[] level_1;
    }

//class for the one pic word data organisation

    [System.Serializable]
    public class OnePicFourWordsData
    {
        public string key;
        public keyWordValue value;
    }


    //class for the key word value organisation
    [System.Serializable]
    public class keyWordValue
    {
        public string imageUrl;
        public string[] words;
        public string keyword;
        public string gameDifficulty;

    }

