using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandIKController : MonoBehaviour
{
    [Header("IK Constraints")]
    [SerializeField] private TwoBoneIKConstraint leftIK;
    [SerializeField] private TwoBoneIKConstraint rightIK;
    
    [Header("IK Targets")]
    [SerializeField] protected Transform leftHandTarget;
    [SerializeField] protected Transform rightHandTarget;
    
    [SerializeField] protected Transform pileBaseLeftPoint;
    [SerializeField] protected Transform pileBaseRightPoint;
    public void EnableIK()
    {
        SetTarget();
        DOTween.To(()=> leftIK.weight, x => leftIK.weight = x, 1f, 0.3f);
        DOTween.To(()=> rightIK.weight, x => rightIK.weight = x, 1f, 0.3f);
    }
    public void DisableIK()
    {
        DOTween.To(()=> leftIK.weight, x => leftIK.weight = x, 0f, 0.3f);
        DOTween.To(()=> rightIK.weight, x => rightIK.weight = x, 0f, 0.3f);
    }
    private void SetTarget()
    {
        leftHandTarget.position = pileBaseLeftPoint.transform.position;
        rightHandTarget.position = pileBaseRightPoint.transform.position;
    }

    public void EnableEatIK()
    {
        SetEatIK();
        DOTween.To(()=> leftIK.weight, x => leftIK.weight = x, 1f, 0.3f);
        DOTween.To(()=> rightIK.weight, x => rightIK.weight = x, 1f, 0.3f);
    }
    private void SetEatIK()
    {
        leftHandTarget.localPosition = new Vector3(-0.254f, 0.77f, 0.086f);
        leftHandTarget.localRotation = Quaternion.Euler(90f, 220f, 200f);
        rightHandTarget.localPosition = new Vector3(0.224f, 0.77f, 0.111f);
        rightHandTarget.localRotation =Quaternion.Euler(87.9f, 179.954f, 186.1f);
    }
}
