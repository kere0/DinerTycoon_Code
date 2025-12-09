using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadBasket : StationBase
{
    private const float BASKET_BREAD_MAX_COUNT = 18;
    [SerializeField] private Waypoint BreadBasketWaypoint;
    [SerializeField] private Transform lookPoint;
    public CustomerController[] occupied = new CustomerController[7];
    public Queue<CustomerController> customerQueue = new Queue<CustomerController>();
    private CustomerController targetCustomer;
    [SerializeField] private MilkMachine milkMachine;

    private float serveInterval = 0.2f; // 자동 서빙 간격
    protected override void Start()
    {
        base.Start();
        productType = Define.ProductType.Bread;
        StartCoroutine(AutoServing());
    }
    protected override void Interaction(WorkerController worker)
    {
        HandStackController handStack = worker.handStackController;
        if (handStack.pileBase.StackCount != 0)
        {
            if (handStack.pileBase.stackObjects.Peek().itemData.itemType != Define.ProductType.Bread)
                return;
        }
        if (handStack.pileBase.StackCount != 0 && pileBase.stackObjects.Count < BASKET_BREAD_MAX_COUNT)
        {
            ItemBase item = handStack.PopObject();
            pileBase.AddToPile(item);
            item.transform.rotation = Quaternion.Euler(0f, 200f, 0f);
        }
    }
    public void EnterCustomer(CustomerController customer)
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == null) // 빈 자리 찾기
            {
                occupied[i] = customer;
                customerQueue.Enqueue(customer);
                customer.MoveWaypoint(BreadBasketWaypoint.Waypoints[i].position,
                    (() => { customer.SetRotation(lookPoint.position); Order(customer); }));
                break;
            }
        }
    }
    private void ExitCustomer(CustomerController customer)
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == customer)
            {
                occupied[i] = null; // 자리 비우기
                customerQueue.Dequeue();
                break;
            }
        }
    }
    private void Order(CustomerController customer)
    {
        int randomValue = Random.Range(1, 4);
        customer.ShowBubbleUI(Define.BubbleState.Order, randomValue, Define.ProductType.Bread);
    }
    private IEnumerator AutoServing()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return new WaitForSeconds(serveInterval);
            if (Worker != null)
            {
                yield return new WaitUntil(() => Worker == null || Worker.handStackController.pileBase.StackCount == 0);
            }
            if (pileBase.stackObjects.Count != 0 && customerQueue.Count > 0)
            {
                if (targetCustomer == null)
                {
                    targetCustomer = customerQueue.Peek();
                }

                if (targetCustomer != null)
                {
                    // 손님이 Idle일 때만 서빙
                    yield return new WaitUntil(() => targetCustomer.CurrentState == Define.CharacterState.Idle);

                    ItemBase itemBase = pileBase.PopObject();
                    targetCustomer.handStackController.ReceiveBread(itemBase);
                    targetCustomer.orderInfo.orderCount--;
                    if (targetCustomer.orderInfo.orderCount == 0)
                    {
                        targetCustomer.ShowBubbleUI(Define.BubbleState.None);
                    }
                    else
                    {
                        targetCustomer.ShowBubbleUI(Define.BubbleState.Order, targetCustomer.orderInfo.orderCount, Define.ProductType.Bread);
                    }
                    // 주문 다 받으면 이동
                    if (targetCustomer.orderInfo.orderCount == 0)
                    {
                        ExitCustomer(targetCustomer);
                        yield return wait;
                        CustomerBranchManager.Instance.MoveToCounter(targetCustomer);
                        //NextAction();
                        targetCustomer = null;
                    }
                }
            }
        }
    }
}
