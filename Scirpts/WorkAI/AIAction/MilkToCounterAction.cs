using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkToCounterAction : IWorkAction
{
    private InteractionAreaUI startArea;
    private InteractionAreaUI targetArea;
    public MilkToCounterAction(InteractionAreaUI startStationPos , InteractionAreaUI interactionAreaUI)
    {
        startArea = startStationPos;
        targetArea = interactionAreaUI;
    }
    public IEnumerator Execute(EmployeeController worker, Action OnComplete = null)
    {
        // 지정된 알바만 상호작용 가능하도록 해줌
        yield return MoveArea(worker,startArea);
        yield return new WaitForSeconds(1f);
        // 지정된 알바만 상호작용 가능하도록 해줌
        yield return MoveArea(worker,targetArea);
    }
    private IEnumerator MoveArea(EmployeeController worker, InteractionAreaUI area)
    {
        area.RegisterWorker(worker);
        worker.SetDestination(area.transform.position);
        while (worker.agent.pathPending == true || worker.agent.remainingDistance > worker.agent.stoppingDistance)
        {
            if (area.interactable.Worker != null)
            {
                if (area != targetArea)
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
            }
            if (area.interactable.Worker == worker)
            {
                worker.agent.isStopped = true;
                worker.isMoving = false;
                worker.agent.ResetPath();
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        area.DeRegisterWorker(worker);
    }
}
