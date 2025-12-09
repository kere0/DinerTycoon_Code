using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkMachine : StationBase
{
    const int MAX_CREATE_MILK_COUNT = 8;

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;
        isUnlocked = true;
        productType = Define.ProductType.Milk;
        WorkAIManager.Instance.RegisterStation(this);
    }
    protected void Init()
    {
        StartCoroutine(CreateMilkCoroutine());
    }
    protected override void Interaction(WorkerController worker)
    {
        if (pileBase.StackCount != 0 && worker.maxItemCapacity > worker.handStackController.pileBase.StackCount)
        {
            ItemBase milk = pileBase.PopObject();
            worker.handStackController.ReceiveMilk(milk);
        }
    }
    private bool CanCreateBread()
    {
        bool canCreateBurger = pileBase.StackCount < MAX_CREATE_MILK_COUNT;
        float targetSpeed = canCreateBurger ? 1f : 0f;
        if (animator.speed != targetSpeed)
        {
            animator.speed = targetSpeed;
        }
        return canCreateBurger;
    }
    IEnumerator CreateMilkCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            yield return new WaitUntil(CanCreateBread);
            pileBase.SpawnObject();
            yield return wait;
        }
    }
}
