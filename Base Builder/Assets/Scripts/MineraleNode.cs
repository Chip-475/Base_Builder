using UnityEngine;

public class MineraleNode : MonoBehaviour
{
    public ResourceSO tipo;
    public int vita;
    public int vitaMax = 10;

    void Start()
    {
        vita = vitaMax;    
    }

    public void colpisci()
    {
        vita -= 1;
        if(vita<=0)
        {
            //drop
            Destroy(gameObject);
        }
    }

}
