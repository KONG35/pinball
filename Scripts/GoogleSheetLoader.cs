using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetLoader : MonoBehaviour
{
    public static GoogleSheetLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private Dictionary<Type, IList> dataLists = new Dictionary<Type, IList>();
    private Dictionary<Type, string> csvURLs = new Dictionary<Type, string>
    {
        { typeof(LoadingData), "https://docs.google.com/spreadsheets/d/1bI72RC2S2vbN48WeKRM1KcWTimsc1z7p7uHBODqBiA4/export?format=csv" },

        { typeof(UITextData), "https://docs.google.com/spreadsheets/d/1RjkftiogaK7GntjrjCewm6LGFnENen4OtD5HlONh6zU/export?format=csv" }
    };

    private void Start()
    {
        //StartCoroutine(LoadAllCSVs());
    }
    public IEnumerator LoadAllCSVs()
    {
        foreach (var entry in csvURLs)
        {
            Type dataType = entry.Key;
            string url = entry.Value;

            if (dataType == typeof(LoadingData))
            {
                yield return StartCoroutine(LoadCSV<LoadingData>(dataType, url));
            }
            else if (dataType == typeof(UITextData))
            {
                yield return StartCoroutine(LoadCSV<UITextData>(dataType, url));
            }
        }
    }

    private IEnumerator LoadCSV<T>(Type type, string url) where T : IBaseData, new()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string csvData = request.downloadHandler.text;
                ParseCSV<T>(type, csvData);
            }
            else
            {
                Debug.LogError($"{type.Name} CSV 불러오기 실패: {request.error}");
            }
        }
    }

    private void ParseCSV<T>(Type type, string csv) where T : IBaseData, new()
    {
        string[] lines = csv.Split('\n');
        List<T> list = new List<T>();

        // 첫 줄은 헤더
        for (int i = 1; i < lines.Length; i++) 
        {
            string[] values = lines[i].Split(',');
            if (values.Length == 0) continue;

            T obj = new T();
            obj.Parse(values);
            list.Add(obj);
        }

        dataLists[type] = list;
        Debug.Log($"{type.Name} {list.Count}개 로드 완료!");
    }

    public List<T> GetDataList<T>() where T : IBaseData
    {
        Type type = typeof(T);
        return dataLists.ContainsKey(type) ? (List<T>)dataLists[type] : new List<T>();
    }
}
