using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmpText;  // 텍스트 객체
    private string originalText;      // 원래 텍스트
    private Color originalColor;      // 원래 색상

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        originalText = tmpText.text;
        originalColor = tmpText.color;
    }

    // 마우스를 올렸을 때 호출됨
    public void OnPointerEnter(PointerEventData eventData)
    {
        tmpText.text = "-> " + originalText;

        // 강조 색상 적용 (파란색 계열)
        if (ColorUtility.TryParseHtmlString("#0c54f4", out Color hoverColor))
        {
            tmpText.color = hoverColor;
        }
    }

    // 마우스를 벗어났을 때 호출됨
    public void OnPointerExit(PointerEventData eventData)
    {
        tmpText.text = originalText;
        tmpText.color = originalColor;
    }
}
