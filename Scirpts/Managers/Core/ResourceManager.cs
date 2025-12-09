using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class ResourceManager
{
    Dictionary<string, Object> resources = new Dictionary<string, Object>();
    public T Load<T>(string key) where T : Object
    {
        if (resources.TryGetValue(key, out Object resource))
        {
            return resource as T;
        }
        else
        {
            Debug.LogError($"{key} 없음");
            return null;
        }
    }
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>(key);
        if (prefab == null)
        {
            Debug.LogError($"{key} 프리팹 없음");
            return null;
        }
        if (pooling == true)
        {
            return Managers.Pool.ObjPop(prefab);
        }
        GameObject go = Object.Instantiate(prefab, parent);
        return go;
    }
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        if (resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }
        var asyncOperation = Addressables.LoadAssetAsync<T>(key);
        asyncOperation.Completed += (op) =>
        {
            if (resources.ContainsKey(key) == false)
            {
                resources.Add(key, op.Result);
                callback?.Invoke(op.Result);
            }
        };
    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback = null) where T : Object
    {
        var allAsyncOperation = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        allAsyncOperation.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;
            foreach (var result in op.Result)
            {
                LoadAsync<T>(result.PrimaryKey, (obj) =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };
    }
}
