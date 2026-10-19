using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpaceshipView : BuildingView
{
    public new SpaceshipData Data => base.Data as SpaceshipData;
    public Spaceship RuntimeObj { get; private set; }
    [SerializeField] Transform botSpawnPoint;
    [SerializeField] SpaceshipUI ui;

    private void Start()
    {
        RuntimeObj = new Spaceship(Data, this, botSpawnPoint, ui);
    }
    private void OnMouseDown()
    {
        RuntimeObj.OnClick();
    }
}
