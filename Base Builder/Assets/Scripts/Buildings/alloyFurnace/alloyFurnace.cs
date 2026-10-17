using UnityEngine;
using System.Collections.Generic;
public class alloyFurnace : Building
{
    public new alloyData data
    {
        get { return base.Data as alloyData; }
    }
    
    public new alloyView sceneObj
    {
        get { return base.SceneObj as alloyView; }
    }
    public Dictionary<ResourceSO, int> input { get; private set; } = new Dictionary<ResourceSO, int>();
    public Dictionary<ResourceSO, int> output { get; private set; } = new Dictionary<ResourceSO, int>();
    public bool lavoro { get;private set; }
    public RecipeSO ricCorr { get; private set; }  
    public alloyFurnace(alloyData data, alloyView sceneObj):base(data, sceneObj)
    {

    }

    public override void Destroy()
    {
        FreeUpCells(GetCellsInBounds(GetBounds()));
        UnityEngine.Object.Destroy(SceneObj.gameObject);
    }
    
    public bool accettaInput(ResourceSO risorsa)
    {
        foreach(RecipeSO recipe in data.ricette)
        {
            if (recipe != null && recipe.inputs.ContainsKey(risorsa)) return true;
        }
        return false;
    }

    public bool aggInput(ResourceSO risorsa, int quant)
    {
        if (risorsa == null || quant < 0) return false;
        if (!accettaInput(risorsa)) return false;
        input.TryGetValue(risorsa, out int attuale);
        input[risorsa] = attuale + quant;
        return true;
    }

    public bool inputPer(RecipeSO ricetta)
    {
        foreach(KeyValuePair<ResourceSO,int>richiesta in ricetta.inputs)
        {
            input.TryGetValue(richiesta.Key, out int poss);
            if (poss < richiesta.Value) return false;
        }
        return true;
    }

    public bool inizio(RecipeSO ric)
    {
        if (ric == null || lavoro) return false;
        if(!data.ricette.Contains(ric)) return false;
        return inputPer(ric);
    }

    public async UniTask<bool> costruisci(RecipeSO ricetta)
    {
        if (!inizio(ricetta)) return false;
        lavoro = true;
        ricCorr = ricetta;
        foreach(KeyValuePair<ResourceSO,int>richiesta in ricetta.inputs)
        {
            input[richiesta.Key] = input[richiesta.Key] - richiesta.Value;
            if (input[richiesta.Key]==0)input.Remove(richiesta.Key);
        }
        bool annullato = await UniTask.Delay(TimeSpan.FromSeconds(ricetta.completionTime), cancellationToken: SceneObj.GetCancellationTokenOnDestroy())
            .SuppressCancellationThrow();
        if(annullato)
        {
            lavoro=false;
            ricCorr = null;
            return false;
        }
        foreach(KeyValuePair<ResourceSO,int>prod in ricetta.outputs)
        {
            output.TryGetValue(prod.Key, out int attuale);
            output[prod.Key] = attuale+prod.Value;
        }
        lavoro = false;
        ricCorr=null;
        return true;
    }

    public bool takeOutput(ResourceSO risorsa,int quant)
    {
        output.TryGetValue(risorsa,out int disp);
        if (quant < 0 || disp < quant) return false;
        output[risorsa] = disp-quant;
        if(output[risorsa]==0)output.Remove(risorsa);
        return true;
    }
}
