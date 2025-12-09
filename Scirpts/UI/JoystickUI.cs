using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private CanvasGroup joystickGroup;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject touchPoint;
    public Vector2 JoystickDir { get;  private set; }
    
    private float maxCursorRadius;
    private Vector2 startTouchPos;
    private float fadeDuration = 0.5f;

    private Color bgOriginalColor;
    private Color cursorOriginalColor;
    private Color touchPointOriginalColor;
    
    public Image joystickBlocker;

    void Awake()
    {
        joystickGroup.alpha = 0f;
        maxCursorRadius = background.GetComponent<RectTransform>().sizeDelta.x;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        joystickGroup.DOKill();
        joystickGroup.alpha = 1;
        startTouchPos = eventData.position;
        background.transform.position = startTouchPos;
        cursor.transform.position = startTouchPos;
        touchPoint.transform.position = startTouchPos;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        JoystickDir = Vector2.zero;
        joystickGroup.DOFade(0, fadeDuration);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchDir = eventData.position - startTouchPos;
        float moveDist = Mathf.Min(touchDir.magnitude, maxCursorRadius);
        JoystickDir = touchDir.normalized;
        cursor.transform.position = startTouchPos + JoystickDir * moveDist;
        touchPoint.transform.position = eventData.position;
    }
}
