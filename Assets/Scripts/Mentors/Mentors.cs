using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Mentor
{
    public string mentor_name;
    public string image_name;
    public string module_name;
}

[System.Serializable]
public class Mentors_struct
{
    public List<Mentor> mentors;
}
