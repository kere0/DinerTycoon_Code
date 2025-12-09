using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 offset;
    private Vector3 startPos = new Vector3(9.55f, 10f, 25.1f);
    private bool isZoomIn = true;
    void Start()
    {
        transform.position = startPos;
        offset = transform.position - player.position;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            (isZoomIn ? (Action)ZoomOut : ZoomIn).Invoke();
            isZoomIn = !isZoomIn;
        }
    }
    void LateUpdate()
    {
        transform.position = player.position + offset;
    }
    private void ZoomOut()
    {
        DOTween.To(() => Camera.main.orthographicSize,
            x => Camera.main.orthographicSize = x,
            20f,        
            0.5f);
    }

    private void ZoomIn()
    {
        DOTween.To(() => Camera.main.orthographicSize,
            x => Camera.main.orthographicSize = x,
            8f,        // 목표 크기
            0.5f);
    }
}
