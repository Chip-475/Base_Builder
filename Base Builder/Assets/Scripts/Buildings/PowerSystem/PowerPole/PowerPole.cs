using System;
using UnityEngine;
using System.Collections.Generic;
public class PowerPole : Building
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    public new PowerPoleView SceneObj => base.SceneObj as PowerPoleView;

    public string poleId;
    public List<Building> connectedBuildings = new List<Building>();

    public PowerPole(PowerPoleData data, PowerPoleView sceneObj) : base(data, sceneObj)
    {
        poleId = Guid.NewGuid().ToString();
        PowerManager.powerPolesDB[poleId] = this;
        PowerManager.instance.RegisterBuilding();
    }
    public override void Destroy()
    {
        PowerManager.powerPolesDB.Remove(poleId);
        PowerManager.instance.UnregisterBuilding();
        //WorldManager.World.UnregisterBuilding(Coords);
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public bool CanConnectTo(Building other)
    {
        if (other == null) return false;
        return Helpers.Overlaps(GetCellsInBounds(ConnectionBounds), GetCellsInBounds(other.ConnectionBounds));
    }
    public void getConnectedBuildings()
    {
        connectedBuildings.Clear();
        foreach (var building in Network.ConnectedBuildings)
        {
            if (building == this) continue;
            if (CanConnectTo(building))
            {
                connectedBuildings.Add(building);
            }
        }
    }
    public void drawConnection(Vector3 target)
    {
            GameObject go= new GameObject("ConnectionLine");
            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            Network.lineRenderers.Add(lineRenderer);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, SceneObj.transform.position);
            lineRenderer.SetPosition(1, target);
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Network.NetworkColor;
            lineRenderer.endColor = Network.NetworkColor;
    }
}
