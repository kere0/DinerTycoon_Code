using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterBase : MonoBehaviour
{
    private Animator animator;
    public HandStackController handStackController;
    public HandIKController ikController;
    public Define.CharacterType characterType;
    public int maxItemCapacity;
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int WALK = Animator.StringToHash("Walk");
    private static readonly int SIT = Animator.StringToHash("Sit");
    protected const float ROTATE_SPEED  = 720;
    protected  float speed  = 3f;
    private int lastAnim;
    private bool lastStackState = false;
    protected Define.CharacterState currentState = Define.CharacterState.Idle;
    public virtual Define.CharacterState CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState == value) return;
            currentState = value;
            SetAnimation();
        }
    }
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        TryGetComponent(out ikController);
    }

    private void Start()
    {
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;
        GameManager.Instance.upgradeAction -= Upgrade;
        GameManager.Instance.upgradeAction += Upgrade;
    }
    private void OnDisable()
    {
        ikController.DisableIK();
    }
    public virtual void Init() { }
    protected virtual void Upgrade() { }
    
    public void SetAnimation()
    {
        int currentAnim = -1;
        switch (currentState)
        {
            case Define.CharacterState.Idle:
                currentAnim = IDLE;
                IKStateCheck();
                break;
            case Define.CharacterState.Move:
                IKStateCheck();
                currentAnim = WALK;
                
                break;
            case Define.CharacterState.Sit:
                IKStateCheck();
                currentAnim = SIT;
                break;
        }
        if (currentAnim == lastAnim) return;
        animator.CrossFade(currentAnim, 0.1f);
        lastAnim = currentAnim;
    }

    private void IKStateCheck()
    {
        if (handStackController.isStacked == true)
        {
            if (lastStackState == true || currentState == Define.CharacterState.Sit) return;

            ikController.EnableIK();
            lastStackState = true;
        }
        else
        {
            if (lastStackState == false || currentState == Define.CharacterState.Sit) return;

            ikController.DisableIK();
            lastStackState = false;
        }
    }
}
