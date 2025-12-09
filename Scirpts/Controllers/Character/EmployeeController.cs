using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EmployeeController : WorkerController
{
    public NavMeshAgent agent;
    private IWorkAction currentAction;
    private StationBase targetStation;
    private bool isWorking = false;
    public bool isMoving = false;
    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out agent);
        agent.angularSpeed = ROTATE_SPEED;
        characterType = Define.CharacterType.Employee;
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    public override void Init()
    {
        agent.speed = Managers.SaveManager.saveData.employeeSpeed;
        maxItemCapacity = Managers.SaveManager.saveData.employeeCapacity;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        StartCoroutine(AIBehaviorLoop());
    }
    void Update() 
    {
        MoveCheck();
        Move();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(AIBehaviorLoop());
        }
    }
    protected override void Upgrade()
    {
        agent.speed = Managers.SaveManager.saveData.employeeSpeed;
        maxItemCapacity = Managers.SaveManager.saveData.employeeCapacity;
    }
    private IEnumerator AIBehaviorLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (true)
        {
            IWorkAction workAction = WorkAIManager.Instance.CreateAction();
            if (workAction != null) 
            { 
                yield return workAction.Execute(this);
            }
            yield return wait;
        }
    }
    private void Move()
    {
        if (isMoving == false) return;
        if (agent.hasPath == false) return;

        Vector3 steer = agent.steeringTarget;
        // NavMesh가 계산한 다음 위치
        Vector3 next = agent.nextPosition;
   
        transform.position = Vector3.MoveTowards(transform.position, next, 
            Managers.SaveManager.saveData.employeeSpeed * Time.deltaTime);
        agent.nextPosition = transform.position;
        Vector3 dir = steer - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(dir),
                ROTATE_SPEED * Time.deltaTime
            );
        }
    }
    public void SetDestination(Vector3 targetPosition, Action arrived = null)
    {
        agent.SetDestination(targetPosition);
        isMoving = true;
    }
    public void MoveCheck()
    {
        CurrentState = isMoving ? Define.CharacterState.Move : Define.CharacterState.Idle;
    }
}
