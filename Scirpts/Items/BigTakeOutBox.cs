using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BigTakeOutBox : ItemBase
{
    public bool isCloseBag = false;
    [SerializeField] private GameObject cap;
    private Vector3 targetPos = new Vector3(-73f, -90f, 0f);
    public void CloseBag(Action onClose = null)
    {
        cap.transform.DOLocalRotate(targetPos, 0.2f).OnComplete(()=> onClose?.Invoke());
    }
}
