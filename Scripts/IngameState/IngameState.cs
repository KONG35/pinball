using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class IngameState
{
    protected IngameManager ingameManager;

    public IngameState(IngameManager manager)
    {
        this.ingameManager = manager;
    }

    public virtual void Init()
    {
        UIManager.Instance.ingameUI.ShowUI(ingameManager.currentIngameState);
    }
    public abstract void Loop();
    public abstract void Exit();
}