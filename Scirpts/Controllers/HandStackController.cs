using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HandStackController : MonoBehaviour
{
    [SerializeField] private CharacterBase character;
    public PileBase pileBase;
    public bool isStacked = false;
    private void Awake()
    {
        pileBase = GetComponentInChildren<PileBase>();
    }
    private void Start()
    {
        pileBase.HandStateCheck -= HandStateCheck;
        pileBase.HandStateCheck += HandStateCheck;
    }
    public void ReceiveBread(ItemBase go)
    {
        go.GetComponent<Collider>().enabled = false;
        go.GetComponent<Rigidbody>().isKinematic = true;
        Receive(go);
    }
    public void ReceiveBurger(ItemBase go)
    {
        Receive(go);
    }
    public void ReceiveMilk(ItemBase go)
    {
        Receive(go);
    }

    private void Receive(ItemBase go)
    {
        if (pileBase.StackCount == 0)
        {
            pileBase.AddToPile(go, ()=>
            {
                go.transform.rotation = transform.rotation;
            });
        }
        else
        {
            if (pileBase.stackObjects.Peek().itemData.itemType == go.itemData.itemType)
            {
                pileBase.AddToPile(go, ()=>
                {
                    go.transform.rotation = transform.rotation;
                });
            }
        }
    }
    public ItemBase PopObject()
    {
        ItemBase item = pileBase.stackObjects.Pop();
        HandStateCheck();
        return item;
    }
    public void HandStateCheck()
    {
        if (pileBase.StackCount > 0)
        {
            isStacked = true;
        }
        else
        {
            isStacked = false;
        }
        character.SetAnimation();
    }
}
