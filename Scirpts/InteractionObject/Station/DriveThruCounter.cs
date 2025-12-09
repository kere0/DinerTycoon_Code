using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DriveThruCounter : StationBase
{
    public Queue<DriveThruCustomerController> driveThruQueue = new Queue<DriveThruCustomerController>();
    [SerializeField] private MoneyPileController moneyPileController;
    [SerializeField] private DriveThruCustomerController targetCustomer;
    [SerializeField] private Waypoint driveThruWaypoint;
    [SerializeField] private Waypoint exitWaypoint;
    [SerializeField] private InteractionAreaUI counterAreaUI;
    private bool isProcessing = false;
    private const int DriveThruCustomerMaxCount  = 7;

    protected override void Start()
    {
        counterAreaUI.interactAction -= CounterInteraction;
        counterAreaUI.interactAction += CounterInteraction;
        base.Start();
        productType = Define.ProductType.PackagingBox;
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;
    }
    private void Init()
    {
        StartCoroutine(SpawnDriveThruCustomer());
    }
    protected override void Interaction(WorkerController worker)
    {
        PileBase pile = worker.handStackController.pileBase;
        if (pile.StackCount != 0)
        {
            if (pile.stackObjects.Peek().itemData.itemType != productType)
                return;
            ItemBase packagingBox = pile.PopObject();
            pileBase.AddToPile(packagingBox);
            packagingBox.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            worker.handStackController.HandStateCheck();
        }
    }
    private void CounterInteraction(WorkerController worker)
    {
        if (driveThruQueue.Count == 0 || isProcessing || pileBase.StackCount == 0) return;
        targetCustomer ??= driveThruQueue.Peek();
        if (targetCustomer.CurrentState == Define.CharacterState.Idle)
        {
            isProcessing = true;
            StartCoroutine(ProcessOrder());
        }
    }
    private IEnumerator SpawnDriveThruCustomer()
    {
        while (true)
        { 
            yield return new WaitUntil(() => driveThruQueue.Count < DriveThruCustomerMaxCount);
            float spawnInterval = Random.Range(3f * driveThruQueue.Count, 5f * driveThruQueue.Count);
            yield return new WaitForSeconds(spawnInterval);
            DriveThruCustomerController customer = CustomerManager.Instance.RequestSpawnDriveThruCustomer();
            yield return LineEnter(customer);
        }
    }
    private IEnumerator LineEnter(DriveThruCustomerController customer)
    {
        if (driveThruQueue.Count >= driveThruWaypoint.Waypoints.Count) yield break;
        driveThruQueue.Enqueue(customer);
        int index = driveThruQueue.Count - 1; 
        Vector3 leftDir = -driveThruWaypoint.Waypoints[0].right;
        Quaternion targetRot = Quaternion.LookRotation(leftDir, Vector3.up);
        if (index == 0)
        {
            customer.MoveWaypoint(driveThruWaypoint.Waypoints[1].position, ()=>
            {
                customer.MoveWaypoint(driveThruWaypoint.Waypoints[0].position, () => Order(customer));
            });
        }
        else
        {
            customer.MoveWaypoint(driveThruWaypoint.Waypoints[index].position, () =>
            {
                if (index == 1)
                {
                    customer.transform.DORotateQuaternion(targetRot, 0.2f);
                }
            });
        }
    }
    private void Order(DriveThruCustomerController customer)
    {
        int orderCount = Random.Range(1, 4);
        customer.ShowBubbleUI(Define.BubbleState.Order, orderCount, Define.ProductType.PackagingBox);

    }
    private IEnumerator ProcessOrder()
    {
        bool isCompleted = false;
        yield return ServePackagingBox(onDone => isCompleted = onDone);
        if (isCompleted == false)
            yield break;
        moneyPileController.CreateMoney(targetCustomer.handStackController.pileBase.StackCount * 5);
        StartCoroutine(Exit(targetCustomer));
        isProcessing = false;
        targetCustomer.ShowBubbleUI(Define.BubbleState.Emotion); // 이모션으로 바꿔야함
        targetCustomer = null;
        ArrangeLine();
    }
    private IEnumerator ServePackagingBox(Action<bool> onDone = null)
    {
        // 주문개수만큼 받도록
        bool isRecieved = false;
        if (pileBase.StackCount == 0)
            yield break;
        while (targetCustomer.orderInfo.orderCount > 0)
        {
            bool isServed = false;
            if (pileBase.StackCount != 0)
            {
                ItemBase itemBase = pileBase.PopObject();
                SoundManager.Instance.PlaySFX(Define.SFXType.Object);
                itemBase.transform.DOJump(targetCustomer.handStackController.pileBase.transform.position, 3, 1, 0.2f).OnComplete(() =>
                {
                    targetCustomer.orderInfo.orderCount -= 1;
                    isServed = true;
                    Managers.Pool.ObjPush(itemBase.gameObject);
                    if (targetCustomer.orderInfo.orderCount == 0)
                    {
                        targetCustomer.ShowBubbleUI(Define.BubbleState.None);
                    }
                    else
                    {
                        targetCustomer.ShowBubbleUI(Define.BubbleState.Order, targetCustomer.orderInfo.orderCount, Define.ProductType.PackagingBox);
                    }
                });
            }
            else
            {
                isProcessing = false;
                onDone?.Invoke(false);
                yield break;
            }
            yield return new WaitUntil(() => isServed);
            yield return new WaitForSeconds(0.1f);
            if (targetCustomer.orderInfo.orderCount == 0)
            {
                moneyPileController.CreateMoney(targetCustomer.handStackController.pileBase.StackCount * 5);
                onDone?.Invoke(true);
            }
        }
        yield return new WaitForSeconds(0.3f);
    }
    private void ArrangeLine()
    {
        int index = 0;
        driveThruQueue.Dequeue();
        Vector3 leftDir = -driveThruWaypoint.Waypoints[0].right;
        Quaternion targetRot = Quaternion.LookRotation(leftDir, Vector3.up);
        foreach (DriveThruCustomerController customer in driveThruQueue)
        {
            customer.orderNumber = index;
            if (customer.orderNumber == 0) 
            {
                // 모서리에 있으면 도착 후 방향틀고 움직임
                customer.MoveWaypoint(driveThruWaypoint.Waypoints[1].position, ()=>
                {
                    customer.MoveWaypoint(driveThruWaypoint.Waypoints[0].position, () => Order(customer));
                });
            }
            else
            {
                customer.MoveWaypoint(driveThruWaypoint.Waypoints[index].position, () =>
                {
                    if (customer.orderNumber == 1)
                    {
                        customer.transform.DORotateQuaternion(targetRot, 0.2f);
                    }
                });
            }
            index++; 
        }
    }
    private IEnumerator Exit(DriveThruCustomerController customer)
    {
        bool reached = false;
        customer.MoveWaypoint(exitWaypoint.Waypoints[0].position, () => reached = true);
        yield return new WaitUntil(() => reached); 
        Managers.Pool.ObjPush(customer.gameObject);
    }
}
 