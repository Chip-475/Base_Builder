using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class Generator : Building
{
    public new GeneratorData Data => base.Data as GeneratorData;
    public new GeneratorView SceneObj => base.SceneObj as GeneratorView;

    public string id;
    public List<PowerPole> ConnectedPoles;


    public Bounds ConnectionBounds { get; private set; }
    public float Power;

    public Generator(GeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {
        id=System.Guid.NewGuid().ToString(); 
        checkNetwork();
    }
    public void checkNetwork()
    {
        NetworkManager[] networks = PowerManager.instance.powerObj.GetComponentsInChildren<NetworkManager>();
        foreach (NetworkManager network in networks)
        {
            if (network.generatorIds.Contains(id)) return;
            foreach (string pole in network.polesIds)
            {
                PowerManager.GetPowerPoleById(pole).CanConnectTo(PowerManager.GetGeneratorById(id));
            }
        }
        foreach (Machine machine in PowerManager.instance.machineDB)
        {
            if (CanConnectTo(machine)) break;
        }
        PowerManager.instance.createNewNetwork();
    }
    public void Fuel(ResourceSO fuel, int quantity)
    {
        if (!Data.allowedFuels.Contains(fuel)) return;
        //implement somehow burn fuel
    }
    public bool CanConnectTo(Building other)
    {
        if (other == null) return false;

        Cell[] myCells = GetCellsInBounds(ConnectionBounds);
        var otherPole = other as PowerPole;
        if (otherPole != null)
        {
            Cell[] otherCells = Building.GetCellsInBounds(otherPole.ConnectionBounds);

            foreach (var cell in myCells)
                foreach (var otherCell in otherCells)
                    if (cell.Coords == otherCell.Coords)
                        return true;
        }
        else
        {
            Cell[] otherCells = Building.GetCellsInBounds(other.GetBounds());
            foreach (var cell in myCells)
                foreach (var otherCell in otherCells)
                    if (cell.Coords == otherCell.Coords)
                        return true;
        }
        return false;
    }
}
