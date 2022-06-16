using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerangaDatasParser
{
    [System.Serializable]
    public class QuizDataStruct
    {
        public Quiz[] quiz_sentence;
    }

    [System.Serializable]
    public class Quiz
    {
        public string sentence;
        public string gameDifficulty;
        public Anwser[] anwser;
    }

    [System.Serializable]
    public class Anwser
    {
        public string text_content;
        public bool is_correct;
    }
}
