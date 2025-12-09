using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerBranchManager : MonoBehaviour
{
    public static CustomerBranchManager Instance;
    [SerializeField] private BreadBasket breadBasket;
    [SerializeField] private Counter counter;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void DecideBreadOrCounter(CustomerController customer)
    {
        int randomNum = Random.Range(0, 2);
        switch (randomNum)
        {
            case 0 :
                breadBasket.EnterCustomer(customer);
                break;
            case 1:
                counter.CounterLineEnter(customer);
                break;
        }
    }
    public void MoveToCounter(CustomerController customer)
    {
        counter.CounterLineEnter(customer);
    }
}
