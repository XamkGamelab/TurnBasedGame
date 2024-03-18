using System;
using UnityEngine;
using UniRx;

public class CharacterBase : MonoBehaviour
{
    public GameObject CharacterModel;    
    public float Speed = 1f;
    public bool PerformTimedActions = true;
    public bool IgnoreInitTimeStep = true;

    protected Animator anim => GetComponentInChildren<Animator>();
    protected int lastActionStep = 0;
    public float InternalTime { get; protected set; } = 0;
    protected virtual void Awake()
    {
        GameSettings.GlobalTime.Subscribe(gt => ProgressInternalTime());
    }
    protected virtual void ProgressInternalTime()
    {
        if (IgnoreInitTimeStep && GameSettings.GlobalTime.Value == 1)
            return;

        InternalTime += Speed;

        if ((int)InternalTime - lastActionStep >= 1) 
        {
            if (PerformTimedActions)            
                //If character is relatively faster, it may perform several action when global time ticks on
                for (int i = 0; i < (int)InternalTime - lastActionStep; i++)                
                    PerformActions();            

            lastActionStep = (int)InternalTime;
        }
    }
    protected virtual void PerformActions() {}
}