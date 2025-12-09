using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        WalletManager.Instance.OnMoneyChanged -= UpdateUI;
        WalletManager.Instance.OnMoneyChanged += UpdateUI;
        UpdateUI(WalletManager.Instance.money);
    }

    private void UpdateUI(int money)
    {
        moneyText.text = money.ToString();
    }
}
