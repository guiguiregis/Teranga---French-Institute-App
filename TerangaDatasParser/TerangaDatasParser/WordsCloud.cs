using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerangaDatasParser
{

    [System.Serializable]
    public class Word_clouds_quiz
    {
        public Word_clouds_quiz_item[] quizes;
    }

    [System.Serializable]
    public class Word_clouds_quiz_item
    {
        public string key_word;
        public string gameDifficulty;
        public string[] words;
        public string[] related;
    }
}
