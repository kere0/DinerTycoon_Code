using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum CharacterType
    {
        Player,
        Customer,
        Employee
    }
    public enum CharacterState
    {
        None,
        Idle,
        Move,
        Sit
    }
    public enum ProductType
    {
        None,
        Bread,
        Burger,
        Milk,
        PackagingBox,
        Trash
    }
    public enum BubbleState
    {
        None,
        Order,
        Pay,
        DineIn,
        Emotion
    }
    public enum OrderType
    {
        None,
        TakeOut,
        DineIn
    }
    public enum SFXType
    {
        Object,
        Money,
        CreateMoney,
        UnLock
    }
    public enum TableState
    {
        None,
        Empty,
        Occupied,
        Dirty
    }
}
