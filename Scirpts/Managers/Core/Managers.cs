using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers Instance;
    private bool init = false;
    private readonly PoolManager poolManager = new PoolManager();
    private readonly FirestoreManager firestoreManager = new FirestoreManager();
    private readonly DataManager dataManager = new DataManager();
    private readonly ResourceManager resourceManager = new ResourceManager();
    private readonly CSVLoader csvLoader = new CSVLoader();
    private readonly SaveManager saveManager = new SaveManager();
    public static PoolManager Pool => Instance.poolManager;
    public static FirestoreManager FirestoreManager => Instance.firestoreManager;
    public static DataManager DataManager => Instance.dataManager;
    public static ResourceManager Resource => Instance.resourceManager;
    public static CSVLoader CSVLoader => Instance.csvLoader;
    public static SaveManager SaveManager => Instance.saveManager;
    
    private void Awake()
    { 
        if (init == false)
        {
            init = true;
            GameObject go = GameObject.Find("Managers");
            if (go == null)
            {
                go = new GameObject("Managers");
                go.AddComponent<Managers>();
                DontDestroyOnLoad(go);
            }
            Instance = go.GetComponent<Managers>();
        }
    }

    private void Start()
    {
        FirestoreManager.Init();
    }
    public void Clear()
    {
        Pool.pools.Clear();
    }
    
}
