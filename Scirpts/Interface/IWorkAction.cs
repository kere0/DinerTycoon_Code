using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorkAction
{
    IEnumerator Execute(EmployeeController worker, Action OnComplete = null);
}