using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Hanged_struct
{

    public List<Hanged_difficulty> m_difficulties;
}

public class Hanged_difficulty
{
    public string difficulty;
    public List<string> levelOne;
    public List<string> levelTwo;
    public List<string> levelThree;
    public List<string> levelFour;
    public List<string> levelFive;
    public List<string> levelSix;
}