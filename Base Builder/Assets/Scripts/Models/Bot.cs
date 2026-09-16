using UnityEngine;
using System;
using System.Collections.Generic;
// inventario a peso, path finding (quando ce un mondo)
[System.Serializable]
public struct resourceInfo
{
    public ResourceSO resource;
    public int quant;
    public int getTotal()
    {
        return resource.weightPerUnit * quant;
    }
} 
public class Bot
{
    public Bot(Vector2Int coords, BotType type)
    {
        Id = Guid.NewGuid().ToString();
        this.coords = coords;
        this.type = BotType.None;
        name = PickRandomName();
        Power = maxPower;
        if (type == BotType.Carrier) maxWeight = 100;
        else maxWeight = 20;
    }
    public string Id { get; protected set; }
    Vector2Int coords = new(0, 0);
    BotType type = BotType.None;
    public string name;
    int Power { get; set; }
    int maxPower = 100;
    int maxWeight;
    public List<resourceInfo> inv = new();
    public int getPeso()
    {
        int tot = 0;
        foreach(var stack in  inv)
        {
            tot += stack.getTotal();
        }
        return tot;
    }

    public bool aggRisorsa(ResourceSO res, int quant)
    {
        int pesoPiu = res.weightPerUnit * quant;
        int pesoAtt=getPeso();
        if (pesoAtt + pesoPiu <=maxWeight) return false;
        for(int i=0;i<inv.Count;i++)
        {
            if (inv[i].resource==res)
            {
                resourceInfo stack = inv[i];
                stack.quant = stack.quant + quant;
                inv[i] = stack;
                return true;
            }
        }
        resourceInfo nuovo = new resourceInfo();
        nuovo.resource = res;
        nuovo.quant = quant;
        inv.Add(nuovo);
        return true;
    }

    public bool rimuoviRes(ResourceSO res,int quant)
    {
        for(int i=0;i<inv.Count;i++)
        {
            if (inv[i].resource == res)
            {
                resourceInfo stack = inv[i];
                if (stack.quant < quant) return false;
                stack.quant -= quant;
                if (stack.quant <= 0) inv.RemoveAt(i);
                else inv[i] = stack;
                return true;
            }
        }
        return false;
    }

    public string PickRandomName()
    {
        string[] names = { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };
        return names[UnityEngine.Random.Range(0, names.Length)];
    }
}
public enum BotType
{
    None,
    Miner,
    Carrier, //100  altri 20
    Worker,
    Builder,
    Specialist
}
