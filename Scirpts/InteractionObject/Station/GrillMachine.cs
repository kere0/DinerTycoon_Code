using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillMachine : StationBase
{
    const int MAX_CREATE_BURGER_COUNT = 6;
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;
        isUnlocked = true;
        productType = Define.ProductType.Burger;
        WorkAIManager.Instance.RegisterStation(this);
    }
    protected void Init()
    {
        StartCoroutine(CreateBurgerCoroutine());
    }
    protected override void Interaction(WorkerController worker)
    {
        if (pileBase.StackCount != 0 && worker.maxItemCapacity > worker.handStackController.pileBase.StackCount)
        {
            ItemBase burger = pileBase.PopObject();
            worker.handStackController.ReceiveBurger(burger);
        }
    }
    private bool CanCreateBurger()
    {
        bool canCreateBurger = pileBase.StackCount < MAX_CREATE_BURGER_COUNT;
        float targetSpeed = canCreateBurger ? 1f : 0f;
        if (animator.speed != targetSpeed)
        {
            animator.speed = targetSpeed;
        }
        Debug.Log(canCreateBurger);
        return canCreateBurger;
    }
    IEnumerator CreateBurgerCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            yield return new WaitUntil(CanCreateBurger);
            pileBase.SpawnObject();
            yield return wait;
        }
    }
}
