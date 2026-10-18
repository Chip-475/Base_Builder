using System.Collections.Generic;
using UnityEngine;
using System;
public static class storageNetwork
{
    public static List<storage> depositi=new List<storage>();
    public static Dictionary<ResourceSO,int> ogg=new Dictionary<ResourceSO, int>();
    public static event Action OnChanged;
    public static int capacita
    {
        get
        {
            int totale=0;
            //logica del totale
            //foreach(storage s in depositi)
            return totale;
        }
    }

    public static int totalDeposito
    {
        get
        {
            int totale=0;
            foreach(int q in ogg.Values)totale+=q;
            return totale;
        }
    }

    public static int spazio=Mathf.Max(0,capacita-totalDeposito);
    //public static Dictionary<ResourceSO,int>oggs =ogg;
    public static void reset()
    {
        depositi.Clear();
        ogg.Clear();
        OnChanged=null;
    }

    public static void registra(storage depo)
    {
        if(depo==null||depositi.Contains(depo))return;
        depositi.Add(depo);
        OnChanged?.Invoke();
    }

    public static void togli(storage depo)
    {
        if(depositi.Remove(depo))OnChanged?.Invoke();
    }

    public static int getQuant(ResourceSO ris)
    {
        if(ris==null)return 0;
        ogg.TryGetValue(ris,out int q);
        return q;
    }
}
