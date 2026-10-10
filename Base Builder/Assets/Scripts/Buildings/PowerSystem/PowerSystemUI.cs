using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class PowerSystemUI : MonoBehaviour
{
    public static PowerSystemUI instance;
    public Button toggleButton;
    public bool isActive=false;
    private void Awake()
    {
        instance = this;

        toggleButton.onClick.AddListener(() => Toggle());
    }
    public void Toggle()
    {
        isActive = !isActive;
        Draw();
    }
    public void Draw()
    {
        foreach (NetworkManager network in PowerManager.instance.powerObj.GetComponentsInChildren<NetworkManager>())
        {
            foreach (Building building in network.ConnectedBuildings)
            {
                List<Cell> cellList=Helpers.GetCellsInBounds(building.ConnectionBounds).ToList();
                foreach (Cell cell in cellList)
                {
                    cell.sr.color = isActive ? network.NetworkColor : Color.white;
                }
            }
        }
    }
}
