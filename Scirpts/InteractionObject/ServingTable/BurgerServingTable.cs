using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BurgerServingTable : ServingTable
{
    private const int SET_BURGER_MAX_COUNT = 50;
    protected override void Start()
    {
        base.Start();
        productType = Define.ProductType.Burger;
        maxCount = SET_BURGER_MAX_COUNT;
    }
}
