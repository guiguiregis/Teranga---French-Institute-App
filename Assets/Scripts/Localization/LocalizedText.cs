using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text textComponent = GetComponent<Text>();
        textComponent.text = LocalizationManager.GetLocalizedValue(textComponent.text);
    }
}
