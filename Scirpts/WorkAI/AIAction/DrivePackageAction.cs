using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivePackageAction : IWorkAction
{
    private InteractionAreaUI startArea;
    public DrivePackageAction(InteractionAreaUI startStationPos)
    {
        startArea = startStationPos;
    }
    public IEnumerator Execute(EmployeeController worker, Action OnComplete = null)
    {
        // 지정된 알바만 상호작용 가능하도록 해줌
        yield return MoveArea(worker,startArea);
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator MoveArea(EmployeeController worker, InteractionAreaUI area)
    {
        area.RegisterWorker(worker);
        worker.SetDestination(area.transform.position);
        while (worker.agent.pathPending == true || worker.agent.remainingDistance > worker.agent.stoppingDistance ||
               worker.agent.velocity.sqrMagnitude > 0.01f)
        {
            if (area.interactable.Worker != null)
            {
                if (area.interactable.Worker.characterType == Define.CharacterType.Player)
                {
                    worker.agent.isStopped = true;
                    worker.isMoving = false;
                    worker.agent.ResetPath();
                    yield return new WaitForSeconds(1f);
                    yield break;
                }
            }
            if (area.interactable.Worker == worker)
            {
                if (worker.agent.remainingDistance < 0.1)
                {
                    worker.agent.isStopped = true;
                    worker.isMoving = false;
                    worker.agent.ResetPath();
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        area.DeRegisterWorker(worker);
    }
}
