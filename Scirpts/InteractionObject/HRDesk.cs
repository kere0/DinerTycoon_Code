using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class HRDesk : StationBase
{
    private bool isOpenUI = false;
    [SerializeField] private GameObject HR_Panel;

    // 광고
    [SerializeField] private Button adsSpeedButton;
    [SerializeField] private Button adsCapacityButton;
    [SerializeField] private Button adsEmploymentButton;

    // 일반
    [SerializeField] private Button speedButton;
    [SerializeField] private Button capacityButton;
    [SerializeField] private Button employmentButton;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private TextMeshProUGUI speedCostText;
    [SerializeField] private TextMeshProUGUI capacityCostText;
    [SerializeField] private TextMeshProUGUI employmentText;

    private int speedUpgradeCost = 250;
    private int capacityUpgradeCost = 250;
    private int employmentCost = 250;

    [SerializeField] private TextMeshProUGUI upgradeText;

    protected override void Interaction(WorkerController worker) { }

    private void Awake()
    {
        // 광고
        adsSpeedButton.onClick.AddListener(AdsSpeedButtonClick);
        adsCapacityButton.onClick.AddListener(AdsCapacityButtonClick);
        adsEmploymentButton.onClick.AddListener(AdsEmploymentButtonClick);

        // 일반 버튼
        speedButton.onClick.AddListener(SpeedButtonClick);
        capacityButton.onClick.AddListener(CapacityButtonClick);
        employmentButton.onClick.AddListener(EmploymentButtonClick);
    }

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.gameStartAction -= Init;
        GameManager.Instance.gameStartAction += Init;

        speedCostText.text = speedUpgradeCost.ToString();
        capacityCostText.text = capacityUpgradeCost.ToString();
        employmentText.text = employmentCost.ToString();
    }

    private void Init()
    {
        StartCoroutine(CreateEmployee());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WorkerController>().characterType == Define.CharacterType.Player)
        {
            HR_Panel.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WorkerController>().characterType == Define.CharacterType.Player)
        {
            HR_Panel.SetActive(false);
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

    private void AdsEmploymentButtonClick()
    {
        AdsManager.Instance.ShowRewardAd(UpgradeEmployment);
    }
    private void UpgradeSpeed()
    {
        Managers.SaveManager.saveData.employeeSpeed += 0.5f;
        GameManager.Instance.UpgradeAction();
        upgradeText.text = "이동 속도가 빨라졌어요!";
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);

        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void UpgradeCapacity()
    {
        Managers.SaveManager.saveData.employeeCapacity += 1;
        GameManager.Instance.UpgradeAction();
        upgradeText.text = "운반 용량이 늘어났어요!";
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);

        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private void UpgradeEmployment()
    {
        Managers.SaveManager.saveData.employeeCount += 1;
        StartCoroutine(SpawnEmployee());
        upgradeText.text = "새 직원이 합류했어요!";
        SoundManager.Instance.PlaySFX(Define.SFXType.CreateMoney);

        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private IEnumerator CreateEmployee()
    {
        WaitForSeconds wait = new WaitForSeconds(2f);

        for (int i = 0; i < Managers.SaveManager.saveData.employeeCount; i++)
        {
            StartCoroutine(SpawnEmployee());
            yield return wait;
        }
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
    private void EmploymentButtonClick()
    {
        if (WalletManager.Instance.money < employmentCost)
        {
            upgradeText.text = "돈이 부족해요!";
        }
        else
        {
            WalletManager.Instance.AddMoney(-employmentCost);
            UpgradeEmployment();
        }
        upgradeText.DOKill();
        upgradeText.DOFade(0f, 1f).From(1f).SetDelay(1f).SetEase(Ease.OutCubic);
    }
    private IEnumerator SpawnEmployee()
    {
        yield return null;
        GameObject go = Managers.Resource.Instantiate("Employee", pooling: true);
        go.transform.position = new Vector3(spawnPoint.position.x, 0.086f, spawnPoint.position.z);
        EmployeeController employee = go.GetComponent<EmployeeController>();
        employee.agent.Warp(employee.transform.position);
        employee.Init();
    }
    private void OnDisable()
    {
        if (AdsManager.Instance == null) return;
    }
}