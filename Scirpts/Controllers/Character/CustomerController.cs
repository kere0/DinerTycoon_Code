using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : CharacterBase
{
    [SerializeField] private BubbleUI bubbleUI;
    public OrderInfo orderInfo = new OrderInfo();
    public NavMeshAgent agent;
    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out agent);
        agent.angularSpeed = ROTATE_SPEED;
        characterType = Define.CharacterType.Customer;
        agent.enabled = false;
    }
    public override void Init()
    {
        speed = Managers.SaveManager.saveData.customerSpeed;
        agent.speed = Managers.SaveManager.saveData.employeeSpeed;
        maxItemCapacity = Managers.SaveManager.saveData.customerCapacity;
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
        transform.GetChild(0).position = Vector3.zero;
        orderInfo.Clear();
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
        if (CurrentState == Define.CharacterState.Sit) return;
        if (DOTween.IsTweening(transform))
        {
            CurrentState = Define.CharacterState.Move;
        }
        else
        {
            CurrentState = Define.CharacterState.Idle;
        }
    }
    public void ShowBubbleUI(Define.BubbleState bubbleState, int count = 0,
        Define.ProductType productType = Define.ProductType.None)
    {
        orderInfo.orderCount =  count;
        bubbleUI.ViewBubbleUI(bubbleState, orderInfo.orderCount, productType);
    }
}
