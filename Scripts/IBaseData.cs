using System.Collections.Generic;
using UnityEngine;

// 공통 데이터 인터페이스
public interface IBaseData
{
    void Parse(string[] values); // 각 데이터 클래스마다 구현
}

// 로딩창 설명 데이터
public class LoadingData : IBaseData
{
    public int Key { get; private set; }
    public Dictionary<LanguageType, string> DescribeDict { get; private set; } = new Dictionary<LanguageType, string>();

    public void Parse(string[] values)
    {
        if (values.Length < 3) return;

        Key = int.Parse(values[0]);
        DescribeDict[LanguageType.English] = values[1].Trim();
        DescribeDict[LanguageType.Korean] = values[2].Trim();
    }

    public string GetDescribe(LanguageType lang)
    {
        // 기본값 영어
        return DescribeDict.ContainsKey(lang) ? DescribeDict[lang] : DescribeDict[LanguageType.English];
    }
}

// 언어 데이터
public class UITextData : IBaseData
{
    public int Key { get; private set; }
    public Dictionary<LanguageType, string> TextDict { get; private set; } = new Dictionary<LanguageType, string>();

    public void Parse(string[] values)
    {
        if (values.Length < 3) return;

        Key = int.Parse(values[0]);
        TextDict[LanguageType.English] = values[1].Trim();
        TextDict[LanguageType.Korean] = values[2].Trim();
    }

    public string GetText(LanguageType lang)
    {
        return TextDict.ContainsKey(lang) ? TextDict[lang] : TextDict[LanguageType.English];
    }
}
