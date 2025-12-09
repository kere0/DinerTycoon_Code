using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class EatTable : StationBase
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TableAreaManager tableAreaManager;
    [SerializeField] private GameObject tableSet;
    public Define.TableState tableState =  Define.TableState.None;
    //[SerializeField] private Waypoint exitWaypoint;
    public CustomerController[] occupiedChairs = new CustomerController[2];
    [SerializeField] private MoneyPileController moneyPileControllerController;
    public GameObject table;
    [SerializeField] private GameObject[] chairs = new GameObject[2];
    [SerializeField] private Waypoint[] tableWaypoints = new Waypoint[2];
    [SerializeField] private Waypoint[] exitWaypoint = new Waypoint[2];
    private int unlockedCost = 50;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image fillImage;
    private bool isTutorial = false;

    [SerializeField] private GameObject trash;
    private int foodCount;
    private Coroutine interactCoroutine;
    private Coroutine unlockCoroutine;
    private Coroutine startEatCoroutine;

    private List<CustomerController> setFoodCustomer = new List<CustomerController>();
    protected override void Start()
    {
        base.Start();               
    }
    protected override void Interaction(WorkerController worker)
    {
        if (worker.CompareTag("Worker"))
        {
            PlayerController player = worker.GetComponent<PlayerController>();
            // 언락
            if (isUnlocked == false)
            {
                if (unlockCoroutine == null)
                {
                    int currentMoney = WalletManager.Instance.money;
            
                    if (currentMoney > 0)
                    {
                        int payAmount = Mathf.Min(unlockedCost, currentMoney);
                        unlockCoroutine = StartCoroutine(WalletManager.Instance.
                            UseMoneyCoroutine(player, interactionAreaUI.transform.position, costText, fillImage, unlockedCost, payAmount, cost =>
                        {
                            Debug.Log(cost);
                            // 해금 완료
                            if (cost <= 0)
                            {
                                ActiveTable();
                                SoundManager.Instance.PlaySFX(Define.SFXType.UnLock);
                            }
                            unlockCoroutine = null;
                        }));
                    }
                }
            }
            // 치우기
            if (tableState == Define.TableState.Dirty)
            {
                tableState = Define.TableState.Empty;
                foreach (GameObject chair in chairs)
                {
                    chair.transform.DOLocalRotate(new Vector3(0, 0f, 0), 0.15f);
                }
                if (interactCoroutine == null)
                {
                    interactCoroutine = StartCoroutine(ClearTable(worker));
                }
            }
        }
    }

    public void ActiveTable()
    {
        isUnlocked = true;
        canvasGroup.alpha = 0;
        tableSet.SetActive(true);
        tableState = Define.TableState.Empty;
        tableAreaManager.UnLockedTable(this);
    }
    private IEnumerator ClearTable(WorkerController worker)
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        while (foodCount > 0)
        {
            pileBase.SpawnObject();
            ItemBase trash = pileBase.PopObject();
            worker.handStackController.pileBase.AddToPile(trash);
            foodCount--;
            yield return wait;
        }
        trash.SetActive(false);
        foodCount = 0;
        interactCoroutine = null;
        tableAreaManager.OnTableEmptyAction();
    }

    private IEnumerator StartEatEmotion(CustomerController customer)
    {
        WaitForSeconds wait = new WaitForSeconds(3.5f);
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            customer.ShowBubbleUI(Define.BubbleState.Emotion);
            yield return wait;
        }
    }
    public IEnumerator StartEating()
    {
        WaitForSeconds wait = new WaitForSeconds(2f);
        foodCount = pileBase.StackCount;
        Debug.Log(foodCount + "dgnalskdgnasldkgnaslkdnglk푸드카운트");
        Coroutine[] eatEmotionCoroutine = new Coroutine[2];
        for (int i = 0; i < occupiedChairs.Length; i++)
        {
            if (occupiedChairs[i] != null)
            {
                eatEmotionCoroutine[i] = StartCoroutine(StartEatEmotion(occupiedChairs[i]));
            }
        }
        while (pileBase.StackCount > 0)
        {
            yield return wait;
            Managers.Pool.ObjPush(pileBase.PopObject().gameObject);
        }
        yield return new WaitForSeconds(0.2f);
        FinishEating(foodCount);
        for (int i = 0; i < eatEmotionCoroutine.Length; i++)
        {
            if (eatEmotionCoroutine[i] != null)
            {
                StopCoroutine(eatEmotionCoroutine[i]);
            }
        }
    }
    private IEnumerator PlaceFoodOnTable(CustomerController customer)
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (customer.handStackController.pileBase.StackCount > 0)
        {
            ItemBase customerPopObject = customer.handStackController.PopObject();
            customer.handStackController.HandStateCheck();
            customerPopObject.transform.rotation = Quaternion.Euler(pileBase.transform.forward);
            pileBase.AddToPile(customerPopObject, () =>
            {
                customerPopObject.transform.localRotation = Quaternion.Euler(0, 90f, 0);
            });
            yield return wait;
        }
        setFoodCustomer.Add(customer);
        if (tableState == Define.TableState.Occupied && setFoodCustomer.Count ==  2)
        {
            if (startEatCoroutine == null)
            {
                startEatCoroutine = StartCoroutine(StartEating());
            }
        }
    }
    private void FinishEating(int count)
    {
        // 나가기 구현
        tableState = Define.TableState.Dirty;
        setFoodCustomer = new List<CustomerController>();
        // 의자 돌리기
        for (int i = 0; i < occupiedChairs.Length; i++)
        {
            if (i == 0)
            {
                chairs[0].transform.DOLocalRotate(new Vector3(0, 45f, 0), 0.2f);
            }
            else if (i == 1)
            {
                chairs[1].transform.DOLocalRotate(new Vector3(0, -45f, 0), 0.2f);
            }
        }
        trash.SetActive(true);
        // 돈 생성
        moneyPileControllerController.CreateMoney(count);
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(Exit(occupiedChairs[i], exitWaypoint[i]));
            occupiedChairs[i].handStackController.isStacked = false;
            occupiedChairs[i].CurrentState = Define.CharacterState.Move;
            occupiedChairs[i] = null;
        }
        startEatCoroutine = null;
    }
    public void SeatCustomer(CustomerController customer)
    {
        for (int i = 0; i < occupiedChairs.Length; i++)
        {
            if (occupiedChairs[i] == null)
            {
                occupiedChairs[i] = customer;
                StartCoroutine(MoveTable(customer, tableWaypoints[i], chairs[i]));
                 UpdateTableState();

                 return;
            }
        }
    }
    private IEnumerator MoveTable(CustomerController customer, Waypoint tableWaypoint, GameObject chair)
    {
        for(int i = 0; i < tableWaypoint.Waypoints.Count; i++)
        {
            bool reached = false;
            customer.MoveWaypoint(tableWaypoint.Waypoints[i].position, () => reached = true);
            yield return new WaitUntil(() => reached); 
        }
        customer.MoveWaypoint(chair.transform.position, () =>
        {
            Vector3 dir = (table.transform.position - customer.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            customer.transform.DORotateQuaternion(targetRot, 0.2f);
            customer.ShowBubbleUI(Define.BubbleState.None);
            StartCoroutine(PlaceFoodOnTable(customer));
            customer.transform.GetChild(0).localPosition = new Vector3(0, 0.4f, 0.4f);
            customer.ikController.EnableEatIK();
            customer.CurrentState = Define.CharacterState.Sit;
            customer.SetAnimation();
        });
    }
    private void UpdateTableState()
    {
        for (int i = 0; i < occupiedChairs.Length; i++)
        {
            if (occupiedChairs[i] == null)
            {
                tableState = Define.TableState.Empty;
                return;
            }
        }
        tableState = Define.TableState.Occupied;
    }
    private IEnumerator Exit(CustomerController customer, Waypoint waypoint)
    {
        for(int i = 0; i < waypoint.Waypoints.Count; i++)
        {
            bool reached = false;
            customer.MoveWaypoint(waypoint.Waypoints[i].position, () => reached = true);
            yield return new WaitUntil(() => reached); 
        }
        Managers.Pool.ObjPush(customer.gameObject);
    }
}
