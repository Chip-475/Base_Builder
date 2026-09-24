using UnityEngine;

public class GeneratorView : MonoBehaviour
{
    public new GeneratorData Data => base.Data as GeneratorData;

    private void Start()
    {
        new PowerPole(Data, this);
    }
}
