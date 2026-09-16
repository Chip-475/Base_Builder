using UnityEngine;
public class MineralNode : InstalledObject
{
    public ResourceSO tipo;
    public int damageTaken;
    public int dmgPerDrop = 10;
    void Start()
    {
        damageTaken = 0;    
    }
    /*public bool scavaDepo(depo)
    {
        ResourceSO ris = depo.scava();
        if (ris == null) return false;
        return bot.aggRisorsa(ris, 1);
    }*/
    public ResourceSO Hit(int dmg)
    {
        damageTaken += dmg;
        if (damageTaken >= dmgPerDrop)
        {
            damageTaken = 0;
            return tipo;
        }
        else return null;
    }
    
}
