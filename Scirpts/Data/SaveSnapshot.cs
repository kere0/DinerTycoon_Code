using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveSnapshot : MonoBehaviour
{
    [FirestoreData]
    public class SaveData
    {
        [FirestoreProperty] public int money { get; set; }
        [FirestoreProperty] public float playerSpeed { get; set; }
        [FirestoreProperty] public int playerCapacity { get; set; }
        [FirestoreProperty] public int increaseProfit { get; set; }
        [FirestoreProperty] public float customerSpeed { get; set; }
        [FirestoreProperty] public int customerCapacity { get; set; }
        [FirestoreProperty] public float employeeSpeed { get; set; }
        [FirestoreProperty] public int employeeCapacity { get; set; }
        [FirestoreProperty] public int employeeCount { get; set; }
        [FirestoreProperty] public int activeTableCount { get; set; }
    }
}
