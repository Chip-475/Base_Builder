using UnityEngine;

public class poleManager : MonoBehaviour
{
    public static poleManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        PowerPole[] powerPoles =FindObjectsByType<PowerPole>();
        foreach (PowerPole pole in powerPoles)
        {
            foreach (PowerPole otherPole in powerPoles)
            {
                if (pole != otherPole && pole.CanConnectTo(otherPole))
                {
                    pole.Connect(otherPole);
                }
            }
        }
    }
}
