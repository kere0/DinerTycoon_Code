using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : WorkerController
{
    [SerializeField] private JoystickUI joystick;
    private CharacterController controller;
  
    protected override void Awake()
    {
        base.Awake();
        controller = GetComponentInChildren<CharacterController>();
        characterType = Define.CharacterType.Player;
    }

    // CSV로 받아서 사용
    public override void Init()
    {
        speed = Managers.SaveManager.saveData.playerSpeed;
        maxItemCapacity = Managers.SaveManager.saveData.playerCapacity;
    }
    private void Update()
    {
        Move();
    }
    protected override void Upgrade()
    {
        speed = Managers.SaveManager.saveData.playerSpeed;
        maxItemCapacity = Managers.SaveManager.saveData.playerCapacity;
    }
    private void Move()
    {
        Vector3 joystickDir = joystick.JoystickDir;
        Vector3 moveDir = new Vector3(joystickDir.x, 0, joystickDir.y);
        if (joystickDir != Vector3.zero)
        {
            CurrentState = Define.CharacterState.Move;
            controller.Move(moveDir * speed * Time.deltaTime);
            Quaternion rotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, ROTATE_SPEED * Time.deltaTime);
        }
        else
        {
            CurrentState = Define.CharacterState.Idle;
        }
        transform.position = new Vector3(transform.position.x, 1.305f, transform.position.z);
    }
}
