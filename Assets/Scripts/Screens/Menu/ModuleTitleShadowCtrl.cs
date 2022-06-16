using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModuleTitleShadowCtrl : MonoBehaviour
{
    private int m_maxTextLengthSize = 10;
    // Start is called before the first frame update
    void Start()
    {
        GameObject lastChild = this.transform.parent.GetChild(this.transform.parent.childCount - 1).gameObject;
        float length = TextWidthApproximation(lastChild.GetComponent<TextMeshProUGUI>().text, lastChild.GetComponent<TextMeshProUGUI>().font, (int)lastChild.GetComponent<TextMeshProUGUI>().fontSize, lastChild.GetComponent<TextMeshProUGUI>().fontStyle);

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(length, this.GetComponent<RectTransform>().sizeDelta.y);
        if(lastChild.GetComponent<TextMeshProUGUI>().text.Length > m_maxTextLengthSize)
        {
            this.transform.position = new Vector3(transform.position.x + 48f, transform.position.y, transform.position.z);
        }
    }


    public float TextWidthApproximation(string text, TMP_FontAsset fontAsset, int fontSize, FontStyles style)
    {
        // Compute scale of the target point size relative to the sampling point size of the font asset.
        float pointSizeScale = (fontSize * 1.2f) / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
        float emScale = fontSize * 10f;

        float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
        float normalSpacingAdjustment = fontAsset.normalSpacingOffset;

        float width = 0;


        for (int i = 0; i < text.Length; i++)
        {
            char unicode = text[i];
            TMP_Character character;
            // Make sure the given unicode exists in the font asset.
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out character))
                width += character.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
        }

        return width;
    }
}
