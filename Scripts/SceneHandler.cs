using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum SceneName
{
    Init=0,
    Ingame
}
public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }

    private Coroutine cor;

    [SerializeField]
    private GameObject loadingObj;
    [SerializeField]
    private Slider progressSlider;
    [SerializeField]
    private TextMeshProUGUI progressTxt;

    //private string[] guideStrArr = { "��ġ�� �뷫 6�� ������ ������ ���� �ִ� �մϴ�.", "��ġ�� ���� ������ �з��Ǿ� �ֽ��ϴ�.", "�� ������ �����ڴ� �������� ����� �����ϴ�.", "���ڱ����� �Ϳ����ϴ�." };

    const int WAITTIME = 15;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cor = null;
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    /// <summary>
    /// �񵿱�� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="_scName"></param> 
    /// <param name="_mode"></param>
    public void LoadScene_Async(SceneName _scName, LoadSceneMode _mode)
    {
        string name = Enum.IsDefined(typeof(SceneName), _scName) ? _scName.ToString() : null;

        if (string.IsNullOrEmpty(name) || IsSceneExists(name)==false)
        {
            Debug.Log("Error : " + name + " �̶�� �� �̸��� �������� �ʽ��ϴ�.");
            // !! �κ�� ���ư���
            return;
        }
        
        if (cor != null)
            StopCoroutine(cor);

        cor = StartCoroutine(AsyncLoadSceneCor(_scName, _mode));
    }
    IEnumerator AsyncLoadSceneCor(SceneName _scName, LoadSceneMode _mode)
    {
        // LoadingData ����Ʈ ��������
        List<LoadingData> languageDataList = GoogleSheetLoader.Instance.GetDataList<LoadingData>();
        int ranIdx = UnityEngine.Random.Range(0, languageDataList.Count);
        
        progressTxt.text = languageDataList[ranIdx].GetDescribe(LocalizationManager.Instance.currentLanguage);

        progressSlider.value = 0f;
        loadingObj.SetActive(true);
        yield return null;

        float elapsedTime = 0f;
        float displayProgress = 0f; // �ε��� 
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(_scName.ToString(), _mode);
        asyncOper.allowSceneActivation = false; // �� �ڵ� ��ȯ ����

        while (!asyncOper.isDone && elapsedTime < WAITTIME)
        {
            elapsedTime += Time.deltaTime; // �����Ӵ� ��� �ð� ����
            float targetProgress = asyncOper.progress < 0.9f ? asyncOper.progress : 1f; // ���� �����

            // �ε巯�� ������ ���� Lerp ���
            displayProgress = Mathf.Lerp(displayProgress, targetProgress, Time.deltaTime * 3f);
            progressSlider.value = displayProgress;

            Debug.Log($"�ε� ����� (ǥ��): {displayProgress * 100}% / (����): {asyncOper.progress * 100}%");

            // ���� �غ�Ǿ���, �ּ� 5�ʰ� ����ϸ� �� ��ȯ
            if (asyncOper.progress >= 0.9f && elapsedTime >= 5f)
            {
                asyncOper.allowSceneActivation = true;
            }
            yield return null;
        }
        loadingObj.SetActive(false);

        switch (_scName)
        {
            case SceneName.Ingame:
                IngameManager.Instance.ChangeState<GameReadyState>();
                break;
            case SceneName.Init:
                IngameManager.Instance.ChangeState<NoneState>();
                break;
        }
    }
    /// <summary>
    /// �ش� �̸��� ���� �����ϴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="_scName"></param> 
    /// <returns></returns>
    private bool IsSceneExists(string _scName)
    {
        for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scPath = SceneUtility.GetScenePathByBuildIndex(i);
            string scFileName = System.IO.Path.GetFileNameWithoutExtension(scPath);
            if (scFileName == _scName)
            {
                return true;
            }
        }
        return false;
    }
}
