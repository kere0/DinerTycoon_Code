using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class DataManager
{
    public Dictionary<Type, Dictionary<string, object>> dataTables = new();

    public void SetResourceData()
    {
        LoadPrefabResource();
        LoadTextAssetResource(() =>
        {
            Managers.SaveManager.LoadAll(() =>
            {
                GameManager.Instance.isLoadTextAsset = true;
                GameManager.Instance.LoadedData();
            });
        });
    }
    private void LoadPrefabResource()
    {
        Managers.Resource.LoadAllAsync<GameObject>("Prefab", (key, count, totalCount) => 
        {
            Debug.Log($"{key}, {count}, {totalCount}");
            if (count == totalCount)
            {
                Debug.Log("프리팹리소스 로드 완료");
                GameManager.Instance.isLoadPrefab = true;
                GameManager.Instance.LoadedData();
            }
        });
    }
    private void LoadTextAssetResource(Action OnComplete = null)
    {
        Managers.Resource.LoadAllAsync<TextAsset>("TextAsset", (key, count, totalCount) =>
        {
            Debug.Log($"{key}, {count}, {totalCount}");
            if (count == totalCount)
            {
                RegisterData<CharacterData>("CharacterData");
                RegisterData<UserData>("UserData");
                Debug.Log("텍스트에셋 리소스 로드 완료");
                OnComplete?.Invoke();
            }
        });
    }
    private void RegisterData<T>(string assetName) where T : InterfaceID, new()
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(assetName);
        Dictionary<string, object> data = Managers.CSVLoader.LoadCSV<T>(textAsset);
        dataTables.Add(typeof(T), data);
    }
    public T Get<T>(string id) where T : InterfaceID
    {
        if (dataTables.TryGetValue(typeof(T), out Dictionary<string, object> dict))
        {
            string key = id;
            if (dict.TryGetValue(key, out object value))
            {
                return (T)value;
            }
        }
        Debug.Log($"ID{id} 없음");
        return default;
    }
}
