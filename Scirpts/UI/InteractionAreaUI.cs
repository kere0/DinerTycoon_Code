using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InteractionAreaUI : MonoBehaviour
{
    // 상호작용할 오브젝트 넣기
    public IInteractable interactable;
    public Action<WorkerController> interactAction;
    private float interactInterval = 0.3f; 
    private Coroutine playerInteractCoroutine;
    private Dictionary<WorkerController, Coroutine> employeeCoroutines = new();
    
    private Vector3 originalSize;
    private RectTransform rectTransform;
    private List<WorkerController> currentWorker = new();
    private void Awake()
    {
        TryGetComponent(out rectTransform);
        interactable = transform.root.GetComponent<MonoBehaviour>() as IInteractable;
    }
    private void Start()
    {
        originalSize = rectTransform.sizeDelta;
        
    }

    public void RegisterWorker(WorkerController worker)
    {
        currentWorker.Add(worker);
    }
    public void DeRegisterWorker(WorkerController worker)
    {
        currentWorker.Remove(worker);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Worker"))
        {
            rectTransform.sizeDelta = originalSize * 1.05f;
            WorkerController workerController = other.GetComponent<WorkerController>();
            if (workerController == null) return;
            if (workerController.characterType == Define.CharacterType.Employee)
            {
                if (currentWorker.Contains(workerController) == false) return;
            }
            interactable.Worker = workerController;
            if (workerController.characterType == Define.CharacterType.Player)
            {
                if (playerInteractCoroutine == null)
                {
                    playerInteractCoroutine = StartCoroutine(InteractRoutine(workerController));
                }
            }
            else if (workerController.characterType == Define.CharacterType.Employee)
            {
                if (employeeCoroutines.ContainsKey(workerController) == false)
                {
                    Coroutine employeeInteractCoroutine = StartCoroutine(InteractRoutine(workerController));

                    employeeCoroutines.Add(workerController, employeeInteractCoroutine);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Worker"))
        {
            WorkerController employee = other.GetComponent<WorkerController>();
            if (currentWorker.Count != 0)
            {
                if (currentWorker.Contains(employee))
                {
                    currentWorker.Remove(employee);
                }
            }
            interactable.Worker = null;
            if (playerInteractCoroutine != null)
            {
                StopCoroutine(playerInteractCoroutine);
                playerInteractCoroutine = null;
            }
            if (employeeCoroutines.TryGetValue(employee, out Coroutine co))
            {
                StopCoroutine(co);
                employeeCoroutines.Remove(employee);
            }
            rectTransform.sizeDelta = originalSize;
        }
    }
    private IEnumerator InteractRoutine(WorkerController worker)
    {
        WaitForSeconds wait = new WaitForSeconds(interactInterval);
        while (true)
        {
            interactAction?.Invoke(worker);
            yield return wait;
        }
    }
}