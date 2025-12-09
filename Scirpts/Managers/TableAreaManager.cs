using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class TableAreaManager : StationBase
{
     public List<EatTable> activeTables = new List<EatTable>();
     public List<EatTable> tables = new List<EatTable>();
     public List<GameObject> tableGameObject = new List<GameObject>();
     public event Action TableEmptyAction; // 테이블 자리난거 알리는 이벤트
     public Counter counter;
     private void Start()
     {
          WorkAIManager.Instance.RegisterStation(this);
          GameManager.Instance.gameStartAction -= Init;
          GameManager.Instance.gameStartAction += Init;
     }
     public void Init()
     {
          for (int i = 0; i < Managers.SaveManager.saveData.activeTableCount; i++)
          {
               tableGameObject[i].SetActive(true);
               tables[i].ActiveTable();
          }
     }
     public void  OnTableEmptyAction()
     {
          TableEmptyAction?.Invoke();
          TableEmptyAction?.Invoke();
     }
     public void UnLockedTable(EatTable eatTable)
     {
          activeTables.Add(eatTable);
          tableGameObject[activeTables.Count-1].SetActive(true);
          OnTableEmptyAction();
     }
     public bool TrySeatCustomer(CustomerController customer)
     {
          for (int i = 0; i < activeTables.Count; i++)
          {
               if (activeTables[i].tableState == Define.TableState.Empty)
               {
                    // 빈자리에 앉히는거
                    activeTables[i].SeatCustomer(customer);
                    counter.ArrangeDineInLIne();
                    return true;
               }
          }
          return false;
     }
     public override bool CanProvideWorkPoint(out InteractionAreaUI interactionArea)
     {
          for (int i = 0; i < activeTables.Count; i++)
          {
               if (activeTables[i].tableState == Define.TableState.Dirty)
               {
                    interactionArea = activeTables[i].interactionAreaUI;
                    return true;
               }
          }
          interactionArea = null;
          return false;
     }
     protected override void Interaction(WorkerController worker){}
}
