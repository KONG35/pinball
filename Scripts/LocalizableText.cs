using TMPro;
using UnityEngine;

public class LocalizableText : MonoBehaviour
{
    [SerializeField] private int textKey; // CSV에서 설정한 Key 값
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateText()
    {
        if (textComponent != null)
        {
            textComponent.text = LocalizationManager.Instance.uiTextDataList[textKey].GetText(LocalizationManager.Instance.currentLanguage);
        }
    }
}
