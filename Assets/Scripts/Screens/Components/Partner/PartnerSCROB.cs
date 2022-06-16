using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
// path of editor menu from we can create the instance of this scriptable objects
[CreateAssetMenu(fileName = "Partner", menuName = "ScriptableObjects/Partner", order = 1)]
public class PartnerSCROB : ScriptableObject
{
    public new string name;
    public string status;
    public Sprite avatar;

    public enum Gender
    {
        MALE,
        FEMALE
    };

    public PartnerSCROB.Gender gender;
    //public bool unlocked = false;

}