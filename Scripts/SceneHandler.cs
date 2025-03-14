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

    //private string[] guideStrArr = { "까치는 대략 6세 정도의 지능을 갖고 있다 합니다.", "까치는 유해 조수로 분류되어 있습니다.", "이 게임의 개발자는 디자인적 재능이 없습니다.", "직박구리는 귀엽습니다." };

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
    /// 비동기로 씬 불러오는 함수
    /// </summary>
    /// <param name="_scName"></param> 
    /// <param name="_mode"></param>
    public void LoadScene_Async(SceneName _scName, LoadSceneMode _mode)
    {
        string name = Enum.IsDefined(typeof(SceneName), _scName) ? _scName.ToString() : null;

        if (string.IsNullOrEmpty(name) || IsSceneExists(name)==false)
        {
            Debug.Log("Error : " + name + " 이라는 씬 이름이 존재하지 않습니다.");
            // !! 로비로 돌아가기
            return;
        }
        
        if (cor != null)
            StopCoroutine(cor);

        cor = StartCoroutine(AsyncLoadSceneCor(_scName, _mode));
    }
    IEnumerator AsyncLoadSceneCor(SceneName _scName, LoadSceneMode _mode)
    {
        // LoadingData 리스트 가져오기
        List<LoadingData> languageDataList = GoogleSheetLoader.Instance.GetDataList<LoadingData>();
        int ranIdx = UnityEngine.Random.Range(0, languageDataList.Count);
        
        progressTxt.text = languageDataList[ranIdx].GetDescribe(LocalizationManager.Instance.currentLanguage);

        progressSlider.value = 0f;
        loadingObj.SetActive(true);
        yield return null;

        float elapsedTime = 0f;
        float displayProgress = 0f; // 로딩바 
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(_scName.ToString(), _mode);
        asyncOper.allowSceneActivation = false; // 씬 자동 전환 방지

        while (!asyncOper.isDone && elapsedTime < WAITTIME)
        {
            elapsedTime += Time.deltaTime; // 프레임당 경과 시간 누적
            float targetProgress = asyncOper.progress < 0.9f ? asyncOper.progress : 1f; // 실제 진행률

            // 부드러운 증가를 위해 Lerp 사용
            displayProgress = Mathf.Lerp(displayProgress, targetProgress, Time.deltaTime * 3f);
            progressSlider.value = displayProgress;

            Debug.Log($"로딩 진행률 (표시): {displayProgress * 100}% / (실제): {asyncOper.progress * 100}%");

            // 씬이 준비되었고, 최소 5초가 경과하면 씬 전환
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
    /// 해당 이름의 씬이 존재하는지 확인하는 함수
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
