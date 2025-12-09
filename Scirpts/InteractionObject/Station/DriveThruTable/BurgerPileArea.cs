using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerPileArea : StationBase
{
    protected override void Start()
    {
        base.Start();
        WorkAIManager.Instance.RegisterStation(this);
    }
    protected override void Interaction(WorkerController worker)
    {
        HandStackController handStack = worker.handStackController;
        if (handStack.pileBase.StackCount != 0)
        {
            if (handStack.pileBase.stackObjects.Peek().itemData.itemType != Define.ProductType.Burger)
                return;
        }
        if (handStack.pileBase.StackCount != 0)
        {
            ItemBase item = handStack.PopObject();
            pileBase.AddToPile(item);
            item.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
