using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool isGameStart = false;
    public bool isLoadPrefab = false;
    public bool isLoadTextAsset = false;
    public Action gameStartAction;
    public Action upgradeAction;
    [SerializeField] private PlayerController playerController; 
    [SerializeField] private TableAreaManager tableAreaManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        Managers.DataManager.SetResourceData();
    }
    public void UpgradeAction()
    {
        upgradeAction?.Invoke();
    }
    public void LoadedData()
    {
        if (isLoadPrefab == true && isLoadTextAsset == true)
        {
            if (isGameStart == false)
            {
                isGameStart = true;  
                gameStartAction?.Invoke();
                Debug.Log("데이터 로드완료 : 게임 시작");
            }
        }
    }
    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause == true) 
            SaveData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == false) 
            SaveData();
    }

    private void SaveData()
    {
        Managers.SaveManager.saveData.activeTableCount = tableAreaManager.activeTables.Count;
        Managers.SaveManager.saveData.money = WalletManager.Instance.money;
        Managers.SaveManager.SaveToFirebase();
    }
}
