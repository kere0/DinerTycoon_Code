using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class OrderInfo
{
    public Define.OrderType orderType = Define.OrderType.None;
    public Define.ProductType productType = Define.ProductType.None;
    public int orderCount = 0;
    
    public void Clear()
    {
        orderType = Define.OrderType.None;
        productType = Define.ProductType.None;
        orderCount = 0;
    }
}
public class Counter : StationBase
{
    [SerializeField] private Waypoint counterWaypoint;
    [SerializeField] private Waypoint dineInWaypoint;
    [SerializeField] private Waypoint exitWaypoint;
    public Queue<CustomerController> dineInWaitQueue = new Queue<CustomerController>();
    public Queue<CustomerController> customerQueue = new Queue<CustomerController>();
    [SerializeField] private MilkMachine milkMachine;
    
    [SerializeField] private BurgerServingTable burgerServingTable;
    [SerializeField] private MilkServingTable milkServingTable;
    [SerializeField] private TableAreaManager tableAreaManager;
    [SerializeField] private MoneyPileController moneyPileController;
    [SerializeField] private CustomerController targetCustomer;
    private bool isProcessing = false;
    protected override void Start()
    {
        base.Start();
        moneyPileController = transform.GetComponentInChildren<MoneyPileController>();
        tableAreaManager.TableEmptyAction -= EmptyTable;
        tableAreaManager.TableEmptyAction += EmptyTable;
    } 
    protected override void Interaction(WorkerController worker)
    {
        if (customerQueue.Count == 0 || isProcessing) return;
        targetCustomer ??= customerQueue.Peek();

        if (targetCustomer.CurrentState == Define.CharacterState.Idle)
        {
            isProcessing = true;
            StartCoroutine(ProcessOrder());
        }
    }
    private void EmptyTable()
    {
        if (dineInWaitQueue.Count != 0)
        {
            tableAreaManager.TrySeatCustomer(dineInWaitQueue.Dequeue());
        }
    }
    public void CounterLineEnter(CustomerController customer)
    {
        LineEnter(customer, counterWaypoint, customerQueue);
    }
    private void DineInWaitLineEnter(CustomerController customer)
    {
        LineEnter(customer, dineInWaypoint, dineInWaitQueue);
    }
    private void LineEnter(CustomerController customer, Waypoint waypoint, Queue<CustomerController> queue)
    {
        if (queue.Count >= waypoint.Waypoints.Count)
        { 
            return;
        }
        queue.Enqueue(customer);
        int index = queue.Count - 1; 
        
        Vector3 leftDir = -waypoint.Waypoints[0].right;
        Quaternion targetRot = Quaternion.LookRotation(leftDir, Vector3.up);
        customer.MoveWaypoint(waypoint.Waypoints[index].position, 
            () =>
            {
                customer.transform.DORotateQuaternion(targetRot, 0.2f);
                if (customer.orderInfo.orderType == Define.OrderType.None)
                {
                    Order(customer);
                }
                else
                {
                    customer.ShowBubbleUI(Define.BubbleState.None);
                }
            });
    }
    private void Order(CustomerController customer)
    {
        // 카운터에서 주문하는 경우
        if (customer.handStackController.pileBase.StackCount == 0)
        {
            Define.ProductType randomOrder = Define.ProductType.Burger;
            // 우유생산기계 UnLocked이면 랜덤 
            if (milkMachine.isUnlocked == true)
            {
                randomOrder = (Define.ProductType)Random.Range(2, 4);
            }
            switch (randomOrder)
            {
                case Define.ProductType.Burger :
                    int orderCount = Random.Range(1, 4);
                    customer.orderInfo.productType = Define.ProductType.Burger;
                    customer.ShowBubbleUI(Define.BubbleState.Order, orderCount, Define.ProductType.Burger);
                    break;
                case Define.ProductType.Milk :
                    customer.orderInfo.productType = Define.ProductType.Milk;
                    customer.ShowBubbleUI(Define.BubbleState.Order, 1, Define.ProductType.Milk);
                    break;
            }
        }
        else
        {
            customer.orderInfo.productType = Define.ProductType.Bread;
            customer.ShowBubbleUI(Define.BubbleState.Pay);
        }
    }
    private IEnumerator ProcessOrder()
    {
        if (targetCustomer.orderInfo.productType == Define.ProductType.Milk)
        {
            targetCustomer.orderInfo.orderType = Define.OrderType.TakeOut;
        }
        else
        {
            if (targetCustomer.orderInfo.orderType == Define.OrderType.None)
            {
                Define.OrderType orderType = (Define.OrderType)Random.Range(1, 3);
                targetCustomer.orderInfo.orderType = orderType;
            }
        }
        Debug.Log(targetCustomer.orderInfo.productType);
        bool isCompleted = false;
        switch (targetCustomer.orderInfo.orderType)
        {
            case Define.OrderType.TakeOut :
                yield return ServeFood(onDone => isCompleted = onDone);
                if(isCompleted == false)
                yield break;
                CustomerManager.Instance.currentCustomer--;
                moneyPileController.CreateMoney(targetCustomer.handStackController.pileBase.StackCount * 5);
                StartCoroutine(Exit(targetCustomer));
                ArrangeLine(customerQueue, counterWaypoint);
                isProcessing = false;
                targetCustomer.ShowBubbleUI(Define.BubbleState.Emotion); // 이모션으로 바꿔야함
                targetCustomer = null;
                break;
            case Define.OrderType.DineIn :
                yield return ServeFood(onDone => isCompleted = onDone);
                if(isCompleted == false)
                    yield break;
                CustomerManager.Instance.currentCustomer--;
                // 돈 생성
                moneyPileController.CreateMoney(targetCustomer.handStackController.pileBase.StackCount *
                                                targetCustomer.handStackController.pileBase.stackObjects.Peek().itemData.value);
                // 테이블에 자리있는지 체크하고 테이블로 보냄
                if (tableAreaManager.TrySeatCustomer(targetCustomer) == false)
                {
                    DineInWaitLineEnter(targetCustomer);
                }
                ArrangeLine(customerQueue, counterWaypoint);
                isProcessing = false;
                targetCustomer.ShowBubbleUI(Define.BubbleState.DineIn);
                targetCustomer = null;  
                break;
        }
    }

    private void ArrangeLine(Queue<CustomerController> queue, Waypoint waypoint)
    {
        int index = 0;
        customerQueue.Dequeue();
        foreach (CustomerController customer in queue)
        {
            customer.MoveWaypoint(waypoint.Waypoints[index].position);
            index++; 
        }
    }

    public void ArrangeDineInLIne()
    {
        int index = 0;
        foreach (CustomerController customer in dineInWaitQueue)
        {
            customer.MoveWaypoint(dineInWaypoint.Waypoints[index].position);
            index++; 
        }
    }
    private IEnumerator ServeFood(Action<bool> onDone)
    {
        Debug.Log(targetCustomer.orderInfo.productType + "ServeFood~~~~~~~~~~~~~~~~~");
        bool isRecieved = false;
        bool isFinished = false;
        switch (targetCustomer.orderInfo.productType)
        {
            case Define.ProductType.Bread:
                yield return BreadHandleOrder(()=>
                {
                    isRecieved = true;
                    isFinished = true;
                });
                break;
            case Define.ProductType.Burger:
                yield return burgerServingTable.HandleOrder(targetCustomer.orderInfo.orderType, pileBase, targetCustomer,
                    OnComplete =>
                    {
                        isRecieved = OnComplete;
                        isFinished = true;
                    });
                break;
            case Define.ProductType.Milk:
                yield return milkServingTable.HandleOrder(targetCustomer.orderInfo.orderType, pileBase, targetCustomer,
                    OnComplete =>
                    {
                        isRecieved = OnComplete;
                        isFinished = true;
                    });
                break;
        }
        yield return new  WaitUntil(() => isFinished == true);
        if (isRecieved == false)
        {
            onDone?.Invoke(isRecieved);
            isProcessing = false;
            yield break; 
        }
        else
        {
            onDone?.Invoke(isRecieved);
        }
        yield return new WaitForSeconds(0.3f);
    }  
    private IEnumerator Exit(CustomerController customer)
    {
        for(int i = 0; i < exitWaypoint.Waypoints.Count; i++)
        {
            bool reached = false;
            customer.MoveWaypoint(exitWaypoint.Waypoints[i].position, () => reached = true);
            yield return new WaitUntil(() => reached); 
        }
        Managers.Pool.ObjPush(customer.gameObject);
    }

    private IEnumerator BreadHandleOrder(Action OnComplete = null)
    {
        if (targetCustomer.orderInfo.orderType == Define.OrderType.TakeOut)
        {
            BigTakeOutBox bigTakeOutBox = pileBase.SpawnObject().GetComponent<BigTakeOutBox>();
            while (targetCustomer.handStackController.pileBase.StackCount > 0)
            {
                bool isServed = false;
        
                ItemBase itemBase = targetCustomer.handStackController.pileBase.PopObject();
                SoundManager.Instance.PlaySFX(Define.SFXType.Object);
                itemBase.transform.DOJump(bigTakeOutBox.transform.position, 3, 1, 0.2f).OnComplete(() =>
                {
                    isServed = true;
                    Managers.Pool.ObjPush(itemBase.gameObject);
                    targetCustomer.handStackController.HandStateCheck();
                });
                yield return new WaitUntil(() => isServed);
                yield return new WaitForSeconds(0.1f);
            }
            if (targetCustomer.handStackController.pileBase.StackCount == 0)
            {
                BigTakeOutBox takeOutBox = pileBase.PopObject().GetComponent<BigTakeOutBox>();
                takeOutBox.CloseBag(()=> targetCustomer.handStackController.pileBase.AddToPile(takeOutBox, () => OnComplete?.Invoke()));
            }
        }
        else
        {
            OnComplete?.Invoke();
        }
    }
}
