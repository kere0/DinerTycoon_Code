using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : ItemBase
{
    private Rigidbody rigidbody;
    private Collider collider;
    private void Awake()
    {
        TryGetComponent(out rigidbody);
        TryGetComponent(out collider);
    }
    private void OnDisable()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.isKinematic = true;
        collider.enabled = true;
    }
}
