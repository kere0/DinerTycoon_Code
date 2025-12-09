using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDesk : StationBase
{
    private bool isOpenUI = false;
    [SerializeField] private GameObject Upgrade_Panel;

    // 광고
    [SerializeField] private Button adsSpeedButton;
    [SerializeField] private Button adsCapacityButton;
    [SerializeField] private Button adsEmploymentButton;

    // 일반
    [SerializeField] private Button speedButton;
    [SerializeField] private Button capacityButton;
    [SerializeField] private Button increaseProfitButton;

    [SerializeField] private TextMeshProUGUI speedCostText;
    [SerializeField] private TextMeshProUGUI capacityCostText;
    [SerializeField] private TextMeshProUGUI increaseProfitCostText;

    private int speedUpgradeCost = 250;
    private int capacityUpgradeCost = 250;
    private int increaseProfitCost = 250;

    [SerializeField] private TextMeshProUGUI upgradeText;

    protected override void Interaction(WorkerController worker) { }

    private void Awake()
    {
        // 광고
        adsSpeedButton.onClick.AddListener(AdsSpeedButtonClick);
        adsCapacityButton.onClick.AddListener(AdsCapacityButtonClick);
        adsEmploymentButton.onClick.AddListener(AdsIncreaseProfitButtonClick);

        // 일반
        speedButton.onClick.AddListener(SpeedButtonClick);
        capacityButton.onClick.AddListener(CapacityButtonClick);
        increaseProfitButton.onClick.AddListener(IncreaseProfitClick);
    }

    protected override void Start()
    {
        base.Start();
        speedCostText.text = speedUpgradeCost.ToString();
        capacityCostText.text = capacityUpgradeCost.ToString();
        increaseProfitCostText.text = increaseProfitCost.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WorkerController>().characterType == Define.CharacterType.Player)
        {
            Upgrade_Panel.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WorkerController>().characterType == Define.CharacterType.Player)
        {
            Upgrade_Panel.SetActive(false);
        }
    }
    private void AdsSpeedButtonClick()
    {
        AdsManager.Instance.ShowRewardAd(UpgradeSpeed);
    }

    private void AdsCapacityButtonClick()
    {
        AdsManager.Instance.ShowRewardAd(UpgradeCapacity);
    }

    private void AdsIncreaseProfitButtonClick()
    {
        AdsManager.Instance.ShowRewardAd(UpgradeProfit);
    }
    private void UpgradeSpeed()
    {
        Managers.SaveManager.saveData.playerSpeed += 0.5f;
        GameManager.Instance.UpgradeAction();
        upgradeText.text = "이동 속도가 빨라졌어요!";
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);

        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void UpgradeCapacity()
    {
        Managers.SaveManager.saveData.playerCapacity += 1;
        GameManager.Instance.UpgradeAction();
        upgradeText.text = "운반 용량이 늘어났어요!";
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);

        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void UpgradeProfit()
    {
        Managers.SaveManager.saveData.increaseProfit += 1;
        upgradeText.text = "수익이 증가해요!";
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);

        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void SpeedButtonClick()
    {
        if (WalletManager.Instance.money < speedUpgradeCost)
        {
            upgradeText.text = "돈이 부족해요!";
        }
        else
        {
            WalletManager.Instance.AddMoney(-speedUpgradeCost);
            UpgradeSpeed();
        }
        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void CapacityButtonClick()
    {
        if (WalletManager.Instance.money < capacityUpgradeCost)
        {
            upgradeText.text = "돈이 부족해요!";
        }
        else
        {
            WalletManager.Instance.AddMoney(-capacityUpgradeCost);
            UpgradeCapacity();
        }
        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void IncreaseProfitClick()
    {
        if (WalletManager.Instance.money < increaseProfitCost)
        {
            upgradeText.text = "돈이 부족해요!";
        }
        else
        {
            WalletManager.Instance.AddMoney(-increaseProfitCost);
            UpgradeProfit();
        }
        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
}