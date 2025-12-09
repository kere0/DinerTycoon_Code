using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;
    public int currentCustomer = 0;
    [SerializeField] private CustomerSpawner customerSpawner;
    [SerializeField] private CustomerBranchManager customerBranchManager;
    [SerializeField] private DriveThruCustomerSpawner driveThruCustomerSpawner;
    [SerializeField] private Counter counter;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;
    }
    void Init()
    {
        StartCoroutine(SpawnCustomer());
    }
    private IEnumerator SpawnCustomer()
    {
        int maxSpawnCount = 11; // 처음 소환할 손님 수
        while (true)
        { 
            yield return new WaitUntil(() => currentCustomer < maxSpawnCount && counter.dineInWaitQueue.Count < maxSpawnCount);
            float spawnInterval = Random.Range(1f * currentCustomer, 3f * currentCustomer);
            yield return new WaitForSeconds(spawnInterval);
            RequestSpawnCustomer();
        }
    }
    public void RequestSpawnCustomer()
    {
        customerSpawner.SpawnCustomer(OnComplete=> customerBranchManager.DecideBreadOrCounter(OnComplete));
        currentCustomer++;
    }
    public DriveThruCustomerController RequestSpawnDriveThruCustomer()
    {
        return driveThruCustomerSpawner.SpawnDriveThruCustomer();
    }
}
