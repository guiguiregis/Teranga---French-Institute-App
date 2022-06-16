using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Job_title
{
    public int max_level;
    public List<string> titles_names;
}


[System.Serializable]
public class Job_title_struct
{
    public List<Job_title> titles;
}