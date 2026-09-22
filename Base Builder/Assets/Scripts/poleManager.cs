using UnityEngine;

public class PoleManager : MonoBehaviour
{
    public static PoleManager instance;
    public PowerPole_View[] powerPoles;
    public int powerProduction;
    public int powerConsumption;
    public int totalPower;
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        powerPoles = FindObjectsByType<PowerPole_View>();
        foreach (var pole in powerPoles)
        {
            foreach (var otherPole in powerPoles)
            {
                if (pole != otherPole && pole.PowerPole.CanConnectTo(otherPole))
                {
                    pole.PowerPole.ConnectToPole(otherPole.PowerPole);
                }
            }
        }

        //foreach (var machine in machines)
        //{
        //    machine.checkConnection();
        //}   
    }
    private void Update()
    {
        totalPower=powerProduction-powerConsumption;
    }
}
