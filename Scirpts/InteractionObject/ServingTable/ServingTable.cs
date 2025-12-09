using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ServingTable : StationBase
{
    protected int maxCount = 0;
    public int ProductCount => pileBase.StackCount;

    protected override void Interaction(WorkerController worker)
    {
        HandStackController handStack = worker.handStackController;
        if (handStack.pileBase.StackCount != 0)
        {
            if (handStack.pileBase.stackObjects.Peek().itemData.itemType != productType)
                return;
        }
        if (handStack.pileBase.StackCount != 0 && pileBase.stackObjects.Count < maxCount)
        {
            ItemBase item = handStack.PopObject();
            pileBase.AddToPile(item);
        }
    }
    public IEnumerator HandleOrder(Define.OrderType orderType, PileBase pileBase, CustomerController customer, Action<bool> OnComplete = null)
    {
        switch (orderType)
        {
            case Define.OrderType.TakeOut :
                Debug.Log("Take Out");
                yield return HandlePackage(pileBase, customer, OnComplete);
                break;
            case Define.OrderType.DineIn :
                yield return HandleServing(customer, OnComplete);
                break;
        }
    }
    private IEnumerator HandlePackage(PileBase pile, CustomerController customer, Action<bool> OnComplete = null)
    {
        if (ProductCount == 0)
        {
            OnComplete?.Invoke(false);
            yield break;
        }
        BigTakeOutBox bigTakeOutBox;
        if (pile.StackCount == 0)
        { 
            bigTakeOutBox = pile.SpawnObject().GetComponent<BigTakeOutBox>();
        }
        else
        {
            bigTakeOutBox = pile.stackObjects.Peek().GetComponent<BigTakeOutBox>();
        }
        while (customer.orderInfo.orderCount > 0)
        {
            bool isServed = false;
            if (pileBase.stackObjects.Count != 0)
            {
                ItemBase itemBase = pileBase.PopObject();
                SoundManager.Instance.PlaySFX(Define.SFXType.Object);
                itemBase.transform.DOJump(bigTakeOutBox.transform.position, 3, 1, 0.2f).OnComplete(() =>
                {
                    customer.orderInfo.orderCount -= 1;
                    isServed = true;
                    Managers.Pool.ObjPush(itemBase.gameObject);
                    if (customer.orderInfo.productType == Define.ProductType.Burger)
                    {
                        SetBubbleUI(customer, Define.ProductType.Burger);
                    }
                    else if (customer.orderInfo.productType == Define.ProductType.Milk)
                    {
                        SetBubbleUI(customer, Define.ProductType.Milk);
                    }
                });
            }
            else
            {
                OnComplete?.Invoke(false);
                yield break;
            }
            yield return new WaitUntil(() => isServed);
            yield return new WaitForSeconds(0.1f);
        }
        if (customer.orderInfo.orderCount == 0)
        {
            BigTakeOutBox takeOutBox = pile.PopObject().GetComponent<BigTakeOutBox>();
            takeOutBox.CloseBag(()=> customer.handStackController.pileBase.AddToPile(takeOutBox, () => OnComplete?.Invoke(true)));
        }
    }
    private IEnumerator HandleServing(CustomerController customer, Action<bool> OnComplete = null)
    {
        if (ProductCount == 0)
        {
            OnComplete?.Invoke(false);
            yield break;
        }
        while (customer.orderInfo.orderCount > 0)
        {
            bool isServed = false;
            if (pileBase.stackObjects.Count != 0)
            {
                customer.handStackController.pileBase.AddToPile(pileBase.PopObject(), () =>
                {
                    customer.orderInfo.orderCount -= 1;
                    isServed = true;
                    customer.ShowBubbleUI(Define.BubbleState.Order, customer.orderInfo.orderCount, Define.ProductType.Burger);
                    customer.handStackController.HandStateCheck();
                    if (customer.orderInfo.productType == Define.ProductType.Burger)
                    {
                        SetBubbleUI(customer, Define.ProductType.Burger);
                    }
                });
            }
            else
            {
                OnComplete?.Invoke(false);
                yield break;
            }
            yield return new WaitUntil(() => isServed);
            yield return new WaitForSeconds(0.1f);
        }
        if (customer.orderInfo.orderCount == 0)
        {
            OnComplete?.Invoke(true);
        }
    }

    private void SetBubbleUI(CustomerController customer, Define.ProductType productType)
    {
        if (customer.orderInfo.orderCount == 0)
        {
            customer.ShowBubbleUI(Define.BubbleState.None);
        }
        else
        {
            customer.ShowBubbleUI(Define.BubbleState.Order, customer.orderInfo.orderCount, productType);
        }
    }
}
