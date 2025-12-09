using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorkAIManager : MonoBehaviour
{
    public static WorkAIManager Instance;

    private List<StationBase> workStations = new List<StationBase>();
    private Dictionary<Type, InteractionAreaUI> stationIneractionArea = new();
    private void Awake()
    {
        Instance = this;
    }
    public void RegisterInteractionArea(Type station, InteractionAreaUI areaUI)
    {
        stationIneractionArea.Add(station, areaUI);
    }

    public InteractionAreaUI GetInteractionArea(Type station)
    {
        if (stationIneractionArea.TryGetValue(station, out InteractionAreaUI areaUI))
        {
            return areaUI;
        }
        return null;
    }
    public void RegisterStation(StationBase station)
    {
        workStations.Add(station);
    }
    private (StationBase, InteractionAreaUI) GetAvailableWorkStation()
    {
        List<(StationBase, InteractionAreaUI)> availableStation = new List<(StationBase, InteractionAreaUI)>();
        foreach (StationBase station in workStations)
        {
            if (station.CanProvideWorkPoint(out InteractionAreaUI workArea))
                availableStation.Add((station, workArea));
        }
        if (availableStation.Count > 0)
        {
            int randomIndex = Random.Range(0, availableStation.Count);
            return (availableStation[randomIndex].Item1, availableStation[randomIndex].Item2);
        }
        return (null, null);
    }

    public IWorkAction CreateAction()
    {
        (StationBase station, InteractionAreaUI startArea) = GetAvailableWorkStation();
        if (station == null) return null;
        switch (station)
        {
            case OvenMachine:
                return new OvenToBasketAction(startArea, GetInteractionArea(typeof(BreadBasket)));
            case GrillMachine:
                int randomIndex = Random.Range(0, 2);
                if (randomIndex == 0)
                    return new BurgerToCounterAction(startArea, GetInteractionArea(typeof(BurgerServingTable)));
                else
                    return new BurgerToDriveThruAction(startArea, GetInteractionArea(typeof(BurgerPileArea)));
            case MilkMachine:
                return new MilkToCounterAction(startArea, GetInteractionArea(typeof(MilkServingTable)));
            case TableAreaManager:
                 return new CleanTrashAction(startArea, GetInteractionArea(typeof(TrashBox)));
            case PackageArea:
                return new DrivePackageAction(startArea);
            case CompletePileArea:
                return new BurgerToCounterAction(startArea, GetInteractionArea(typeof(DriveThruCounter)));
            default:
                return null;
        }
    }
}