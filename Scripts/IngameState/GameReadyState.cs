using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GameReadyState : IngameState
{
    public GameReadyState(IngameManager manager) : base(manager) { }
    private IngameUIManager ingameUimanager;
    
    public override void Init()
    {
        base.Init();
        if (ingameManager.objectManager==null)
            ingameManager.objectManager = GameObject.FindObjectOfType<IngameObjectManager>();
        
        ingameManager.ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();

        ingameManager.ball.transform.position = ingameManager.objectManager.spawnTrArr[ingameManager.stageIdx].position;
    }

    public override void Loop()
    {
    }
    public override void Exit()
    {
    }

}
