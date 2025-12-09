using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class MilkServingTable : ServingTable
{
    private const int SET_MILK_MAX_COUNT = 54;
    protected override void Start()
    {
        base.Start();
        productType = Define.ProductType.Milk;
        maxCount = SET_MILK_MAX_COUNT;
    }
}
