using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


