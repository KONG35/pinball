using TMPro;
using UnityEngine;

public class LocalizableText : MonoBehaviour
{
    [SerializeField] private int textKey; // CSV���� ������ Key ��
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
