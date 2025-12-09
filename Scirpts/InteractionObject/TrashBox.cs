using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrashBox : StationBase
{
    [SerializeField] private Transform targetPoint;
    protected override void Start()
    {
        base.Start();
        isUnlocked = true;
        productType = Define.ProductType.Trash;
        WorkAIManager.Instance.RegisterStation(this);
    }
    protected override void Interaction(WorkerController worker)
    {
        HandStackController handStack = worker.handStackController;
        if (handStack.pileBase.StackCount != 0)
        {
            if (handStack.pileBase.stackObjects.Peek().itemData.itemType != productType)
                return;
        }
        if (handStack.pileBase.StackCount != 0)
        {
            ItemBase item = handStack.PopObject();
            SoundManager.Instance.PlaySFX(Define.SFXType.Object);
            item.transform.DOJump(targetPoint.position,3,1,  0.2f).OnComplete(()=> Managers.Pool.ObjPush(item.gameObject));
        }
    }
}
