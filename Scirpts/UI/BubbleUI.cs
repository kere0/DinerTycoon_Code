using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleUI : MonoBehaviour
{
    [SerializeField] private GameObject background;
    // 주문 할 때
    [SerializeField] private GameObject orderUI;
    [SerializeField] private Image orderImage;
    [SerializeField] private Sprite breadSprite;
    [SerializeField] private Sprite burgerSprite;
    [SerializeField] private Sprite milkSprite;
    [SerializeField] private Sprite driveThruBurgerSprite;
    
    public TextMeshProUGUI orderCountText;
    // 주문 후 어떤 행동할지
    [SerializeField] private GameObject actionUI;
    [SerializeField] private Image actionImage;
    [SerializeField] private Sprite payActionSprite; 
    [SerializeField] private Sprite dineInActionSprite;
    // 감정표현
    [SerializeField] private ParticleSystem emotionParticle;
    
    private void OnOrderUI(int orderCount, Define.ProductType productType)
    {
        CloseUI();
        SetOrderCount(orderCount);
        background.SetActive(true);
        orderUI.SetActive(true);
        switch (productType)
        {
            case Define.ProductType.Bread:
                orderImage.sprite = breadSprite;
                break;
            case Define.ProductType.Burger:
                orderImage.sprite = burgerSprite;
                break;
            case Define.ProductType.Milk:
                orderImage.sprite = milkSprite;
                break;
            case Define.ProductType.PackagingBox:
                orderImage.sprite = driveThruBurgerSprite;
                break;
        }
    }
    private void OnActionUI(Sprite sprite)
    {
        CloseUI();
        actionImage.sprite = sprite;
        background.SetActive(true);
        actionUI.SetActive(true);
    }
    private void OnEmotion()
    {
        CloseUI();
        emotionParticle.Play();
    }
    private void SetOrderCount(int orderCount)
    {
        orderCountText.text = orderCount.ToString();
    }
    public void CloseUI()
    {
        background.SetActive(false);
        orderUI.SetActive(false);
        actionUI.SetActive(false);
    }
    public void ViewBubbleUI(Define.BubbleState actionType, int orderCount = 0,
        Define.ProductType productType = Define.ProductType.None)
    {
        switch (actionType)
        {
            case Define.BubbleState.None:
                CloseUI();
                break;
            // 주문 할 때
            case Define.BubbleState.Order :
                OnOrderUI(orderCount, productType);
                break;
            // 빵 집은 후 바로 계산 할 때
            case Define.BubbleState.Pay :
                OnActionUI(payActionSprite);
                break;
            // 먹고 갈 때
            case Define.BubbleState.DineIn :
                OnActionUI(dineInActionSprite);
                break;
            // 먹고 나갈 때
            case Define.BubbleState.Emotion :
                OnEmotion();
                break;
        }
    }
}
