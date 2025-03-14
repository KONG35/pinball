using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance { get; private set; }
    
    private IngameState currentState;
    private Dictionary<Type, IngameState> stateInstances;

    [SerializeField] 
    private SerializableDictionary<string, IngameState> serializedStateInstances;
    [SerializeField] 
    private string currentStateName;
    public InGameStateFlag currentIngameState { get; private set; }

    public Ball ball;
    public int stageIdx;

    public IngameObjectManager objectManager;
    public bool isGamePaused = false;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        stateInstances = new Dictionary<Type, IngameState>();
        stateInstances[typeof(NoneState)] = new NoneState(this);
        stateInstances[typeof(GameReadyState)] = new GameReadyState(this);
        stateInstances[typeof(GameStartState)] = new GameStartState(this);
        stateInstances[typeof(GameOverState)] = new GameOverState(this);
        stateInstances[typeof(GameEndState)] = new GameEndState(this);
        stateInstances[typeof(GameClearState)] = new GameClearState(this);

        stateInstances[typeof(ShootReadyState)] = new ShootReadyState(this);
        stateInstances[typeof(ShootingState)] = new ShootingState(this);
        stateInstances[typeof(ShootEndState)] = new ShootEndState(this);

        serializedStateInstances.SetDictionary(ConvertStateInstancesToSerializable());

        stageIdx = 0;
    }
    private void Start()
    {
        ChangeState<NoneState>();
    }
    private void Update()
    {
        if (currentState != null)
            currentState.Loop();
    }
    public void ChangeState<T>() where T : IngameState
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = stateInstances[typeof(T)];
        currentStateName = currentState.GetType().Name;
        currentIngameState = GetGameStateFlag<T>();
        currentState.Init();

        // 상태 변경 시 UI 업데이트
        //UIManager.Instance.ingameUI.ShowUI(GetGameStateFlag<T>());
    }

    public InGameStateFlag GetGameStateFlag<T>() where T : IngameState
    {
        if (typeof(T) == typeof(NoneState)) return InGameStateFlag.None;
        if (typeof(T) == typeof(GameReadyState)) return InGameStateFlag.GameReady;
        if (typeof(T) == typeof(GameStartState)) return InGameStateFlag.GameStart;
        if (typeof(T) == typeof(GameOverState)) return InGameStateFlag.GameOver;
        if (typeof(T) == typeof(GameEndState)) return InGameStateFlag.GameEnd;
        if (typeof(T) == typeof(GameClearState)) return InGameStateFlag.GameClear;

        if (typeof(T) == typeof(ShootReadyState)) return InGameStateFlag.ShootReady;
        if (typeof(T) == typeof(ShootingState)) return InGameStateFlag.Shooting;
        if (typeof(T) == typeof(ShootEndState)) return InGameStateFlag.ShootEnd;
        return InGameStateFlag.None;
    }
    private Dictionary<string, IngameState> ConvertStateInstancesToSerializable()
    {
        var tempDict = new Dictionary<string, IngameState>();
        foreach (var pair in stateInstances)
        {
            tempDict.Add(pair.Key.Name, pair.Value);
        }
        return tempDict;
    }
}
