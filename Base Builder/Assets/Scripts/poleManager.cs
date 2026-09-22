using UnityEngine;

public class poleManager : MonoBehaviour
{
    public static poleManager instance;
    public PowerPole[] powerPoles;
    public int powerProduction;
    public int powerConsumption;
    public int totalPower;
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        powerPoles =FindObjectsByType<PowerPole>();
        Machine[] machines = FindObjectsByType<Machine>();
        foreach (PowerPole pole in powerPoles)
        {
            foreach (PowerPole otherPole in powerPoles)
            {
                if (pole != otherPole && pole.CanConnectTo(otherPole))
                {
                    pole.ConnectToAPole(otherPole);
                }
            }
        }
        foreach (Machine machine in machines)
        {
            machine.checkConnection();
        }   
    }
    private void Update()
    {
        totalPower=powerProduction-powerConsumption;
    }
}
