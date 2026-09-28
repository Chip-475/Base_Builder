using Cysharp.Threading.Tasks;
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

    public NetworkManager network
    {
        get {return getNetwork();}
    }
    public Bounds ConnectionBounds { get; private set; }
    public float Power;
    public bool _running;
    public bool running 
    {
        get {  return _running; }
        set
        {
            if (_running == value) return;
            _running = value;
            NetworkManager net = network;
            if (net == null) return;
            if (_running) net.generation += Power;
            else net.generation -= Power;
        }
    }
    public Generator(GeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {
        id=Guid.NewGuid().ToString();
        PowerManager.powerGeneratorDB[id] = this;
        NetworkManager net;
        if (!PowerManager.instance.checkNetwork(this, out net))
        {
            net = PowerManager.instance.CreateNewNetwork();
            net.ConnectedBuildings.Add(this);
        }
    }

   

    public void Fuel(ResourceSO fuel, int quantity)
    {
        if (!Data.allowedFuels.Contains(fuel)) return;
        //implement somehow burn fuel
    }
    /*
    public bool CanConnectTo(PowerPole otherPole)
    {
        Cell[] myCells = GetCellsInBounds(ConnectionBounds);
        Cell[] otherCells = otherPole.GetCellsInBounds(otherPole.ConnectionBounds);

        foreach (var cell in myCells)
            foreach (var otherCell in otherCells)
                if (cell.Coords == otherCell.Coords)
                    return true;

        return false;
    }*/
    public void SwitchState() //to link to the ui button
    {
        running = !running;
    }
    public NetworkManager getNetwork()
    {
        NetworkManager net;
        PowerManager.instance.checkNetwork(this,out net);
        return net;
        /*
        NetworkManager[] networks = PowerManager.instance.powerObj.GetComponentsInChildren<NetworkManager>();
        foreach (NetworkManager network in networks)
        {
            if (network.ConnectedBuildings.Contains(this)) return network;
        }
        return null;*/
    }
    public override void Destroy()
    {
        running = false;
        NetworkManager net = network;
        if (net != null) net.ConnectedBuildings.Remove(this);
        PowerManager.powerGeneratorDB.Remove(id);
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }
}
