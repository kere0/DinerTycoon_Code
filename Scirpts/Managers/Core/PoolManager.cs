using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    public Dictionary<string, Pool> pools = new();
    public void ObjInit(GameObject go, Transform transform = null)
    {
        if (pools.ContainsKey(go.name)) return;
        Pool pool = new Pool();
        pool.PoolCreate(go, transform);
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Object.Instantiate(pool.OriginalPrefab, pool.ObjParent);
            obj.name = pool.OriginalPrefab.name;
            obj.SetActive(false);
            pool.poolQueue.Enqueue(obj);
        }
        pools.Add(pool.OriginalPrefab.name, pool);
    }
    public GameObject ObjPop(GameObject gameObject)
    {
        string key = gameObject.name;
        if (pools.ContainsKey(key) == false)
        {
            Debug.Log($"{key}_Pool 없음");
            ObjInit(gameObject);
        }
        GameObject go = null;
        if (pools[key].poolQueue.Count != 0)
        {
            go = pools[key].poolQueue.Dequeue();
            //go.transform.position = position;
            go.SetActive(true);
        }
        else
        {
            go = Object.Instantiate(pools[key].OriginalPrefab, pools[key].ObjParent);
            //go.transform.position = position;
            go.name = pools[key].OriginalPrefab.name;
        }
        pools[key].count++;
        return go;
    }
    public void ObjPush(GameObject go)
    {
        if (pools.ContainsKey(go.name) == false)
        {
            Debug.Log($"{go.name}_Pool 없음");
            return;
        }
        go.transform.SetParent(pools[go.name].ObjParent);
        go.transform.rotation = Quaternion.identity;
        go.SetActive(false);
        pools[go.name].poolQueue.Enqueue(go);
        pools[go.name].count--;
    }
    public int GetActiveCount(string key)
    {
        return pools[key].count;
    }
    public class Pool
    {
        public GameObject OriginalPrefab;
        public Transform ObjParent;
        public Queue<GameObject> poolQueue = new Queue<GameObject>();
        public int count = 0;
        public void PoolCreate(GameObject go, Transform parent = null)
        {
            OriginalPrefab = go;
            GameObject parentPool = new GameObject { name = $"{OriginalPrefab.name}_Pool" };
            ObjParent = parentPool.transform;
            ObjParent.SetParent(parent);
        }
    }
}
