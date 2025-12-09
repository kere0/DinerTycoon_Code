using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PileBase : MonoBehaviour
{
    public GameObject prefab; // 풀링할 기본 프리팹
    public Stack<ItemBase> stackObjects = new Stack<ItemBase>();
    [SerializeField] protected int width;
    [SerializeField] protected int depth;
    [SerializeField] protected int sampleHeight;
    [SerializeField] protected Vector3 pileSize = new Vector3(1f, 0.5f, 1f);
    public Action HandStateCheck;
    public int StackCount => stackObjects.Count;

    public ItemBase SpawnObject(Action end = null)
    {
        Vector3 worldSpawnPos = transform.TransformPoint(GetLocalSpawnPosition(stackObjects.Count, pileSize));
        GameObject spawnObj = Managers.Resource.Instantiate(prefab.name, pooling: true);
        spawnObj.transform.position = worldSpawnPos;

        spawnObj.transform.SetParent(transform, true); // true면 worldPosition 유지
        ItemBase item = spawnObj.GetComponent<ItemBase>();
        stackObjects.Push(item);
        return item;
    }

    public void AddToPile(ItemBase item, Action end = null)
    {
        stackObjects.Push(item);
        HandStateCheck?.Invoke();
        item.transform.SetParent(transform);

        Vector3 localDest = GetLocalSpawnPosition(stackObjects.Count - 1, item.itemData.size);
        // 로컬 점프 애니메이션
        SoundManager.Instance.PlaySFX(Define.SFXType.Object);
        item.transform.DOLocalJump(localDest, 3, 1, 0.2f)
            .OnComplete(() =>
            {
                item.transform.localPosition = localDest;
                end?.Invoke();
            });
    }
    public ItemBase PopObject()
    {
        if (stackObjects.Count == 0) return null;
        return stackObjects.Pop();
    }
    private Vector3 GetLocalSpawnPosition(int currentIndex, Vector3 size)
    {
        Vector3 offset = new Vector3(((width - 1) * pileSize.x) / 2, 0, ((depth - 1) * pileSize.z) / 2);
        Vector3 startPosition = -offset;

        float row = currentIndex % width;
        float cloum = (currentIndex / width) % depth;
        float height = currentIndex / (width * depth);

        float x = startPosition.x - row * size.x;
        float z = startPosition.z + cloum * size.z;
        float y = height * size.y;

        return new Vector3(x, y, z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        int previewCount = width * depth * sampleHeight;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
 
        for (int i = 0; i < previewCount; i++)
        {
            Vector3 localPos = GetLocalSpawnPosition(i, pileSize);
            localPos.y += pileSize.y / 2f;
            Gizmos.DrawWireCube(localPos, pileSize);
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}
