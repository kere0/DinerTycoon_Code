using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DriveThruCustomerController : CharacterBase
{
    [SerializeField] private BubbleUI bubbleUI;
    public OrderInfo orderInfo = new OrderInfo();
    private const float speed  = 7f;
    public bool isReady = false;
    public int orderNumber = 0;
    [SerializeField] private ParticleSystem smokeParticle;
    private bool isMovingParticle = false;
    public override Define.CharacterState CurrentState
    {
        get { return currentState; }
        set { currentState = value;}
    }

    void Start() 
    {
        characterType = Define.CharacterType.Customer;
    }
    void Update()
    {
        MoveCheck();
    }
    private void OnDisable()
    {
        // 풀에 들어갈 때 손에있는거 Push
        while (handStackController.pileBase.StackCount > 0)
        {
            ItemBase stackObject = handStackController.pileBase.PopObject();
            stackObject.transform.DOKill();
            Managers.Pool.ObjPush(stackObject.gameObject);
        }
        orderInfo.Clear();
        isReady = false;
        orderNumber = 0;
    }
    public void MoveWaypoint(Vector3 targetPosition, Action arrived = null)
    {
        transform.DOMove(targetPosition, speed).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(()=>arrived?.Invoke());
        SetRotation(targetPosition);
    }
    public void SetRotation(Vector3 targetDir)
    {
        transform.DOLookAt(targetDir, 0.2f, AxisConstraint.Y);
    }
    private void MoveCheck()
    {
        if (DOTween.IsTweening(transform) == true && isMovingParticle == false)
        {
            CurrentState = Define.CharacterState.Move;
            smokeParticle.Play();
            isMovingParticle = true;
        }
        else if(DOTween.IsTweening(transform) == false && isMovingParticle == true)
        {
            CurrentState = Define.CharacterState.Idle;
            smokeParticle.Stop();
            isMovingParticle = false;
        }
    }
    public void ShowBubbleUI(Define.BubbleState bubbleState, int count = 0,
        Define.ProductType productType = Define.ProductType.None)
    {
        orderInfo.orderCount =  count;
        bubbleUI.ViewBubbleUI(bubbleState, orderInfo.orderCount, productType);
    }
}
