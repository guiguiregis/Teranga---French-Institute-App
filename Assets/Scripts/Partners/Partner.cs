using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Partner 
{

    public string partner_name;
    public string image_name;
    public string gender;
    public string title;
}

[System.Serializable]
public class Partners_struct
{
    public List<Partner> partners;
}