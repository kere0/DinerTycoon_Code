using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-90)]
public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private Waypoint spawnWaypoints;
    private string[] customerNames = {"Builder", "Miner", "Police", "Security", "Worker"};
    public void SpawnCustomer(Action<CustomerController> OnComplete)
    {
        string customerName = customerNames[Random.Range(0, customerNames.Length)];
        GameObject go = Managers.Resource.Instantiate(customerName, pooling: true);
        CustomerController customer = go.GetComponent<CustomerController>();
        customer.transform.position = spawnWaypoints.Waypoints[0].position;
        StartCoroutine(MoveSpawnWaypoint(customer, ()=> OnComplete?.Invoke(customer)));
    }
    private IEnumerator MoveSpawnWaypoint(CustomerController customer, Action OnComplete = null)
    {
        for (int i = 1; i < spawnWaypoints.Waypoints.Count; i++)
        {
            bool reached = false;
            customer.MoveWaypoint(spawnWaypoints.Waypoints[i].position, ()=> reached = true);
            yield return new WaitUntil(() => reached); 
        }
        OnComplete?.Invoke();
    }
}

