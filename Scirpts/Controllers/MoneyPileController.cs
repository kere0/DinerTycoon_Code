using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoneyPileController : StationBase
{
    private int moneyPileCount = 0;
    private Queue<ItemBase> moneyQueue= new Queue<ItemBase>();
    private bool isReceiving = false;
    public int MoneyPileCount => moneyPileCount;
    protected override void Interaction(WorkerController worker)
    {
        if (worker.CompareTag("Worker"))
        {
            PlayerController player = worker.GetComponent<PlayerController>();
            if (moneyPileCount > 0)
            {
                WalletManager.Instance.AddMoney(moneyPileCount*5);
                for (int i = 0; i < moneyPileCount; i++)
                {
                    moneyQueue.Enqueue(pileBase.PopObject());
                }
                moneyPileCount = 0;
                StartCoroutine(ReceiveMoneyCoroutine(player));
            }
        }
    }
    public void CreateMoney(int count)
    {
        for (int i = 0; i < count + Managers.SaveManager.saveData.increaseProfit; i++)
        {
            ItemBase itemBase = pileBase.SpawnObject(); 
            moneyPileCount++;
        }
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);
    }

    private IEnumerator ReceiveMoneyCoroutine(PlayerController player)
    {
        PlayerController tempPlayer = player;
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        isReceiving = true;
        while (moneyQueue.Count > 0)
        {
            //SoundManager.Instance.PlaySFX(Define.SFXType.Money);
            ItemBase money = moneyQueue.Dequeue();
            // 플레이어 손 위치 기준으로 날아가게
            Vector3 targetPos = tempPlayer.handStackController.pileBase.transform.position;
            SoundManager.Instance.PlaySFX(Define.SFXType.Money);
            money.transform.DOJump(targetPos, 1.5f, 1, 0.3f)
                .SetEase(Ease.OutQuad).OnUpdate(() =>
                {
                    Vector3 followPos = player.handStackController.pileBase.transform.position;
                    money.transform.DOMove(followPos, 0.05f); 
                })
                .OnComplete(() =>
                {
                    money.transform.DOKill();
                    Managers.Pool.ObjPush(money.gameObject); // 풀로 반환
                });

            yield return wait; // 간격 두고 하나씩 날아가게
            isReceiving = false;
        }
    }
}