using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletePileArea : StationBase
{
    protected override void Start()
    {
        base.Start();
        WorkAIManager.Instance.RegisterStation(this);
    }
    protected override void Interaction(WorkerController worker)
    {
        if (pileBase.StackCount != 0 && worker.maxItemCapacity > worker.handStackController.pileBase.StackCount)
        {
            ItemBase item = pileBase.PopObject();
            if (item == null) return;
            worker.handStackController.pileBase.AddToPile(item);
            item.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
