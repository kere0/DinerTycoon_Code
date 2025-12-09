using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    private List<WorkerController> workers = new List<WorkerController>();
    private void OnTriggerEnter(Collider other)
    {
        if (workers.Contains(other.gameObject.GetComponent<WorkerController>()) == false)
        {
            workers.Add(other.gameObject.GetComponent<WorkerController>());
        }
        Vector3 toPlayer = other.gameObject.transform.position - transform.position;
        float dot = Vector3.Dot(transform.forward, toPlayer);
        if (dot > 0)
        {
            // 플레이어가 문 앞쪽에 있음
            OpenDoorForward();
        }
        else
        {
            // 플레이어가 문 뒤쪽에 있음
            OpenDoorBackward();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (workers.Contains(other.gameObject.GetComponent<WorkerController>()) == true)
        {
            workers.Remove(other.gameObject.GetComponent<WorkerController>());
        }

        if (workers.Count == 0)
        {
            leftDoor.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f);
            rightDoor.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f);
        }
    }
    private void OpenDoorBackward()
    {
        leftDoor.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 0.2f);
        rightDoor.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), 0.2f);
    }

    private void OpenDoorForward()
    {
        leftDoor.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), 0.2f);
        rightDoor.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 0.2f);
    }

    
}
