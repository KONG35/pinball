using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Flags]
public enum InGameStateFlag
{
    None = 0,        
    GameReady = 1 << 0,   
    GameStart = 1 << 1,   
    ShootReady = 1 << 2,
    Shooting = 1 << 3,
    ShootEnd = 1 << 4,
    GameOver = 1 << 5,
    GameEnd = 1 << 6,
    GameClear = 1 << 7,
}
[Serializable]
public class UIElement
{
    public GameObject uiObject;
    public InGameStateFlag visibleStates;
}

public class IngameUIManager : MonoBehaviour
{
    [SerializeField] 
    private List<UIElement> uiElements;

    [SerializeField]
    private Button playTabBtn;

    [SerializeField]
    private TextMeshProUGUI roundTxt;

    private IngameManager inGmanager;
    private void Awake()
    {
        playTabBtn.onClick.AddListener(PressPlayTabBtn);
    }
    private void Start()
    {
        if(inGmanager==null)
            inGmanager = IngameManager.Instance;
    }
    public void ShowUI(InGameStateFlag currentState)
    {
        foreach (var element in uiElements)
        {
            // 현재 상태가 UI가 표시될 수 있는 상태에 포함되는지 확인
            bool shouldBeVisible = (element.visibleStates & currentState) != 0;
            element.uiObject.SetActive(shouldBeVisible);
        }
        switch (currentState)
        {
            case InGameStateFlag.GameReady:
                InitGameReady();
                break;
        }
    }

    private void PressPlayTabBtn()
    {
        IngameManager.Instance.ChangeState<GameStartState>();
    }
    
    private void InitGameReady()
    {
        roundTxt.text = "Round " + (inGmanager.stageIdx + 1).ToString();
    }
    private void InitGameStart()
    {
    }
    private void InitShootReady()
    {
    }
    private void InitShootEnd()
    {
    }
}
