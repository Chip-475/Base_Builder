using UnityEngine;
using System.Collections.Generic;

public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }



    private void Awake()
    {
        Instance = this;
    }
}
