using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmpText;
    private string originalText;
    private Color originalColor;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        originalText = tmpText.text;
        originalColor = tmpText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tmpText.text = "-> " + originalText;
        if (ColorUtility.TryParseHtmlString("#0c54f4", out Color hoverColor))
        {
            tmpText.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmpText.text = originalText;
        tmpText.color = originalColor;
    }
}
