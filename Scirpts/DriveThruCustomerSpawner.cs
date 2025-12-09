using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DriveThruCustomerSpawner : MonoBehaviour
{
 
    [SerializeField] private Transform spawnPoint;
    private string[] customerNames = {"Car1", "Car2", "Car3"};
    public DriveThruCustomerController SpawnDriveThruCustomer()
    {
        string customerName = customerNames[Random.Range(0, customerNames.Length)];
        GameObject go = Managers.Resource.Instantiate(customerName, pooling: true);
        DriveThruCustomerController customer = go.GetComponent<DriveThruCustomerController>();
        customer.transform.position = spawnPoint.position;
        return customer;
    }
}
