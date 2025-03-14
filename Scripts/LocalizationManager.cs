using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LanguageType
{
    English=0,
    Korean
}
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public List<UITextData> uiTextDataList = new List<UITextData>();
    public LanguageType currentLanguage = LanguageType.English;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(LoadLocalizationData());
    }

    private IEnumerator LoadLocalizationData()
    {
        // GoogleSheetLoader가 모든 데이터를 로드할 때까지 대기
        yield return StartCoroutine(GoogleSheetLoader.Instance.LoadAllCSVs());

        // LanguageData 리스트 가져오기
        uiTextDataList.Clear();
        uiTextDataList = GoogleSheetLoader.Instance.GetDataList<UITextData>();

        Debug.Log("언어 데이터 로드 완료!");
        RefreshAllUI();
    }

    public void ChangeLanguage(LanguageType newLanguage)
    {
        currentLanguage = newLanguage;
        RefreshAllUI();
    }

    private void RefreshAllUI()
    {
        foreach (var localizable in FindObjectsOfType<LocalizableText>())
        {
            localizable.UpdateText();
        }
    }
}
