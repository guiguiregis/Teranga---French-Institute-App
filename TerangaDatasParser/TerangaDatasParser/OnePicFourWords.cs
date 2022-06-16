using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerangaDatasParser
{
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
}
