using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SentencesDataStruct 
{
    public Sentence[] sentences;
}

[System.Serializable]
public class Sentence
{

    public string sentence;
    public string[] words;
    public string author;
    public string gameDifficulty;
    public List<Difficulty> difficulty;
}

[System.Serializable]
public class Difficulty
{
    public int missing_words;
    public int trap;
}

/// <summary>
/// FOR EACH SENTENCE IN THE JSON FILE, WE SHOULD PUT THEM INTO THE LIST 
/// AND EACH HAVE HIS LIST OF WORDS CONTAINING THE SENTENCE AND THE LIST OF MISSING WORDS
/// </summary>
[System.Serializable]
public class MissingWordsGameDataStruct
{
    public List<GameDataSentence> sentencesData =  new List<GameDataSentence>();
}


/// <summary>
/// FOR EACH SENTENCE, WE SHOULD SEPARATE EACH WORD INTO THE LIST  SENTENCE WORDS
/// </summary>
[System.Serializable]
public class GameDataSentence
{
    public string[] sentenceWords;
    public List<string> words =  new List<string>();
    public string authorName;
    public List<Difficulty> difficulty;

    public string ReturnWholeSentence()
    {
        string sentence = "";
        foreach(string s in sentenceWords)
        {
            sentence += s;
        }
        return sentence;
    }
}
