using UnityEngine;

public class MineralNode : MonoBehaviour
{
    public ResourceSO tipo;
    public int damageTaken;
    public int dmgPerDrop = 10;

    void Start()
    {
        damageTaken = 0;    
    }

    public void hit(int dmg)
    {

        damageTaken += dmg;
        if(damageTaken>=dmgPerDrop)
        {
            //drop
            damageTaken = 0;
        }
    }

}
