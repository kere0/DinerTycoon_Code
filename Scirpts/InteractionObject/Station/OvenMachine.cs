using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenMachine : StationBase
{
    private Stack<ItemBase> breadStack = new Stack<ItemBase>();
    [SerializeField] private Transform createPos;
    [SerializeField] private GameObject createTrigger;
    [SerializeField] private GameObject endTrigger;
    
    private static readonly int OPEN_OVEN = Animator.StringToHash("OpenOven");
    private static readonly int ACTIVE_OVEN = Animator.StringToHash("ActiveOven");
    private const float MAX_CREATE_BREAD_COUNT = 10;
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;
        isUnlocked = true;
        productType = Define.ProductType.Bread;
        WorkAIManager.Instance.RegisterStation(this);
    }

    protected void Init()
    {
        StartCoroutine(CreateBreadCoroutine());
    }
    protected override void Interaction(WorkerController worker)
    {
        if (breadStack.Count != 0 && worker.maxItemCapacity > worker.handStackController.pileBase.StackCount)
        {
            ItemBase bread = breadStack.Pop();
            worker.handStackController.ReceiveBread(bread);
        }
    }
    private bool CanCreateBread()
    {
        bool canCreateBread = breadStack.Count < MAX_CREATE_BREAD_COUNT;
        float targetSpeed = canCreateBread ? 1f : 0f;
        if (animator.speed != targetSpeed)
        {
            animator.speed = targetSpeed;
        }
        return canCreateBread;
    }
    // 오븐 열리는 애니메이션 코루틴
    IEnumerator CreateBreadCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return new WaitUntil(CanCreateBread);
            GameObject go = Managers.Resource.Instantiate("Bread", pooling: true);
            go.transform.position = createPos.position;
            animator.CrossFade(OPEN_OVEN, 0.1f);
            yield return new WaitUntil(() => createTrigger.activeInHierarchy == false);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.AddForce(3.5f * createPos.forward, ForceMode.Impulse);
            yield return new WaitUntil(() => endTrigger.activeInHierarchy == false);
            animator.CrossFade(ACTIVE_OVEN, 0.1f);
            breadStack.Push(go.GetComponent<ItemBase>());
            yield return wait;
        }
    }

    public override bool CanProvideWorkPoint(out InteractionAreaUI targetArea)
    {
        if (breadStack.Count > 0)
        {
            targetArea = interactionAreaUI;
            return true;
        }
        targetArea = null;
        return false;
    }

}
