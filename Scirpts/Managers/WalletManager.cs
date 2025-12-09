using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DefaultExecutionOrder(-30)]
public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;
    public int money = 300;
    public event Action<int> OnMoneyChanged;
    private bool isReceiving = false;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GameManager.Instance.gameStartAction -= SetMoney;
        GameManager.Instance.gameStartAction += SetMoney;
    }
    private void SetMoney()
    {
        money = Managers.SaveManager.saveData.money;
        OnMoneyChanged?.Invoke(money);
    }
    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }
    public IEnumerator UseMoneyCoroutine(PlayerController player, Vector3 targetPosition, TextMeshProUGUI text, Image fillImage, int cost, int payAmount, Action<int> end = null)
    {
        int unlockCost = cost;
        int currentGauge = 0;
        WaitForSeconds wait = new WaitForSeconds(0.03f);
        isReceiving = true;
        for (int i = 0; i < payAmount; i++)
        {
            player.handStackController.pileBase.SpawnObject();
            ItemBase money = player.handStackController.PopObject();
            money.transform.position = player.handStackController.pileBase.transform.position;
            SoundManager.Instance.PlaySFX(Define.SFXType.Money);
            money.transform.DOJump(targetPosition, 1.5f, 1, 0.3f)
                .OnComplete(() =>
                {
                    Managers.Pool.ObjPush(money.gameObject);

                });
            this.money--;
            unlockCost--;
            currentGauge++;
            fillImage.fillAmount = currentGauge / 50f;
            text.text = unlockCost.ToString();
            OnMoneyChanged?.Invoke(this.money);

            yield return wait;

            if (unlockCost <= 0)
                break;
        }
        isReceiving = false;
        end?.Invoke(unlockCost);
    }
}