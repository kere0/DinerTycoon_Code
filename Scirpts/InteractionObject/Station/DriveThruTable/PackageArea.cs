using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageArea : StationBase
{
    private PackagingBox packagingBox;
    [SerializeField] private PileBase burgerPile;
    [SerializeField] private PileBase completePackagePile;
    protected override void Start()
    {
        base.Start();
        WorkAIManager.Instance.RegisterStation(this);
    }
    protected override void Interaction(WorkerController worker)
    {
        if (pileBase.StackCount == 0 && burgerPile.StackCount != 0)
        {
            packagingBox = pileBase.SpawnObject().GetComponent<PackagingBox>();
            packagingBox.transform.localPosition = Vector3.zero;
        }
        if (packagingBox == null) return;
        if(pileBase.StackCount != 0 && burgerPile.StackCount != 0 && packagingBox.pileBase.StackCount < PackagingBox.PACKAGINGBOX_BURGER_MAX_COUNT)
        {
            ItemBase burger = burgerPile.PopObject();
            if(burger == null) return;
            packagingBox.pileBase.AddToPile(burger);
        }
        if (packagingBox.pileBase.StackCount == PackagingBox.PACKAGINGBOX_BURGER_MAX_COUNT)
        {
            completePackagePile.AddToPile(pileBase.PopObject());
        }
    }
    public override bool CanProvideWorkPoint(out InteractionAreaUI interactionArea)
    {
        if (burgerPile != null && burgerPile.StackCount > 0)
        {
            interactionArea = interactionAreaUI;
            return true;
        }
        interactionArea = null;
        return false;
    }
}
