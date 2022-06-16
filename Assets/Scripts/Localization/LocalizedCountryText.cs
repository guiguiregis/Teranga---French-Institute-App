using UnityEngine;
using UnityEngine.UI;

public class LocalizedCountryText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text textComponent = GetComponent<Text>();
        textComponent.text = LocalizationManager.GetLocalizedValue("COUNTRY_" + textComponent.text.Replace(" ", "_").ToUpper());
    }
}
