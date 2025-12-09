using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class StationBase : MonoBehaviour, IInteractable
{
    public Define.ProductType productType = Define.ProductType.None;
    public PileBase pileBase;
    protected Animator animator;
    
    public InteractionAreaUI interactionAreaUI;
    public WorkerController Worker { get; set; }
    
    public bool isUnlocked = false;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    protected virtual void Start()
    {
        interactionAreaUI.interactAction -= Interaction;
        interactionAreaUI.interactAction += Interaction;
        if (GetType() != typeof(EatTable) && GetType() != typeof(MoneyPileController))
        {
            WorkAIManager.Instance.RegisterInteractionArea(GetType(), interactionAreaUI);
        }
    }
    protected abstract void Interaction(WorkerController worker);
    public virtual bool CanProvideWorkPoint(out InteractionAreaUI interactionArea)
    {
        if (pileBase != null && pileBase.StackCount > 0)
        {
            interactionArea = interactionAreaUI;
            return true;
        }
        interactionArea = null;
        return false;
    }
}