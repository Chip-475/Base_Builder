using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpaceshipView : BuildingView
{
    public new SpaceshipData Data => base.Data as SpaceshipData;
    public Spaceship RuntimeObj { get; private set; }
    [SerializeField] Transform botSpawnPoint;

    private void Start()
    {
        RuntimeObj = new Spaceship(Data, this, botSpawnPoint);
    }
    private void OnMouseDown()
    {
        RuntimeObj.OnClick();
    }
}
