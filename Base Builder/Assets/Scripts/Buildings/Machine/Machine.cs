using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Machine : Building, IInteractable, IWorkplace
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    public bool Powered { get; private set; }
    public float RequestedPower { get; private set; }
    public MachineType Type { get; protected set; } = MachineType.None;

    //per smelter and alloy furnace automatic
    public Dictionary<ResourceSO, int> input{get; private set;}=new Dictionary<ResourceSO, int>();
    public Dictionary<ResourceSO,int> output{get; private set;}=new Dictionary<ResourceSO, int>();
    public bool lavoro{get;private set;}
    public bool aspeEne{get;private set;}
    public RecipeSO ricOra{get; private set;}
    public float progresso{get; private set;}

    public bool cuociAuto
    {
        get
        {
            return Type == MachineType.Alloy_Furnace || Type == MachineType.Smelter;
        }
    }


    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        Type = data.machineType;
        PowerManager.machineDB.Add(this);
        PowerManager.instance.RegisterBuilding();
        InteractionManager.Instance.interactables.Add(this);
    }
    public override void Destroy()
    {
        FreeUpCells(GetCellsInBounds(GetBounds()));
        BuildingManager.UnregisterBuilding(this);
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public bool RequestPower(float amount)
    {
        NetworkManager currentNetwork = Network;
        if (currentNetwork == null || !currentNetwork.RequestPower(this, amount))
        {
            if (currentNetwork != null) currentNetwork.StopPowerRequest(this);
            RequestedPower = 0;
            Powered = false;
            return false;
        }

        RequestedPower = amount;
        Powered = true;
        return true;
    }
    public bool RequestPower(RecipeSO recipe)
    {
        if (recipe == null) return false;
        return RequestPower(recipe.energyCost);
    }
    public void StopPowerRequest()
    {
        if (Network != null) Network.StopPowerRequest(this);
        RequestedPower = 0;
        Powered = false;
    }
    public void RefreshPowerRequest()
    {
        RequestPower(RequestedPower);
    }
    /*
    public bool start(RecipeSO ric)
    {
        if(ric==null||lavoro||Data.usableRecipes==null||!Data.usableRecipes.Contains(ric))return false;
        foreach(KeyValuePair<ResourceSO,int> richiesta in ric.inputs)
        {
            Depot.Inventory.Items.TryGetValue(richiesta.Key,out int posseduta);
            if(posseduta<richiesta.Value)return false;
        }
        return true;
    }*/
    /*
    public void avvioAuto()
    {
        if(lavoro||Data.usableRecipes==null)return;
        foreach(RecipeSO recipe in Data.usableRecipes)
        {
            if(!start(recipe))continue;
            if(recipe.energyCost>0)//)
            {
                aspeEne=true;
                return;
            }
        }
        aspeEne=false;
        //funzione craft
        return;
    }*/

    /*async UniTaskVoid prova()
    {
        var token=SceneObj.GetCancellationTokenOnDestroy();
        while(true)
        {
            bool annulato=await UniTask.Delay(500,cancellationToken token).SuppressCancellationThrow();
            
        }
    }*/
    

    public bool CanInteractWith()
    {
        Bounds area = new Bounds(Coords,Data.interactionBounds.size);
        /*var cells = GetCellsInBounds(Data.interactionBounds);
        foreach (var item in cells)
            if (item == PlayerManager.Instance.GetPlayerCell()) return true;
        */
        Cell cell = PlayerManager.Instance.GetPlayerCell();
        foreach (Cell cella in GetCellsInBounds(area))
        {
            if(cella==cell)return true;
        }
        return false;
    }
    public void Interact()
    {
       /* if (!CanInteractWith())
        {
            Debug.Log("can't interact");
            return;
        }*/
        Debug.Log("inside");
        MachineUI.Instance.Machine = this;
        MachineUI.Instance.Open(this);
    }

    //cuoci automatico
    // ingressi e uscite
    public bool accettaIn(ResourceSO res)
    {
        if(Data.usableRecipes==null)
        {
            Debug.Log("ricette non ci sono");
            return false;
        }
        foreach(RecipeSO r in Data.usableRecipes)
        {
            if (r!=null && r.inputs.ContainsKey(res)) return true;
        }
        return false;
    }
    
    public bool aggIn(ResourceSO res,int quant)
    {
        if(!cuociAuto)
        {
            Debug.Log(Type + " non cuoce da sola stronzo");
            return false;
        }
        if (res == null || quant <= 0||!accettaIn(res)) return false;
        input.TryGetValue(res, out int attuale);
        input[res]= attuale+quant;
        tryAutoStart();
        return true;
        return false;
    }

    // Work
    public override Dictionary<ResourceSO, int> GetWorkOutputs()
    {
        throw new NotImplementedException();
    }

    public bool depositDaBot(Bot bot)
    {
        if (!cuociAuto || bot is not WorkerBot) return false;
        bool depo = false;
        List<ResourceSO> ris = new List<ResourceSO>(bot.Inventory.Items.Keys);
        foreach(ResourceSO r in ris)
        {
            if (!accettaIn(r)) continue;
            int quant = bot.Inventory.Items[r];
            if(aggIn(r,quant))
            {
                bot.Inventory.Items.Remove(r);
                depo = true;
            }
        }
        return depo;
    }

    public bool prendiOut(ResourceSO res,int quant)
    {
        output.TryGetValue(res,out int dispo);
        if (quant <= 0 || dispo < quant) return false;
        output[res] = dispo - quant;
        if (output[res] == 0) output.Remove(res);
        return true;
    }

    //avvio lavorazione
    public bool inputAll(RecipeSO recipe)
    {
        if (recipe.inputs.Count == 0) return false;
        foreach(KeyValuePair<ResourceSO,int> ric in recipe.inputs)
        {
            input.TryGetValue(ric.Key, out int poss);
            if (poss < ric.Value) return false;
        }
        return true;
    }

    public bool start(RecipeSO rec)
    {
        if (rec == null || lavoro||Data.usableRecipes==null||!Data.usableRecipes.Contains(rec))return false;
        return inputAll(rec);

    }

    public void tryAutoStart()
    {
        if (!cuociAuto||lavoro||Data.usableRecipes==null)return;
        foreach(RecipeSO rec in Data.usableRecipes)
        {
            if (!start(rec)) continue;
            if(rec.energyCost>0&&!RequestPower(rec))
            {
                aspeEne = true;
                return;
            }
            aspeEne = false;
            startCraft(rec).Forget();
            return;
        }
        aspeEne = false;
    }

    async UniTask startCraft(RecipeSO rec)
    {
        lavoro = true;
        ricOra = rec;
        progresso = 0f;
        foreach(KeyValuePair<ResourceSO,int> richiesta in rec.inputs)
        {
            input[richiesta.Key] = input[richiesta.Key] - richiesta.Value;
            if(input[richiesta.Key] == 0) input.Remove(richiesta.Key);
        }
        var token = SceneObj.GetCancellationTokenOnDestroy();
        float trascorso = 0f;
        while(trascorso<rec.completionTime)
        {
            bool annullato = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();
            if(annullato)
            {
                lavoro = false;
                ricOra = null;
                return;
            }
            if(rec.energyCost<=0||Powered)
            {
                trascorso = trascorso + Time.deltaTime;
                progresso = Mathf.Clamp01(trascorso / rec.completionTime);
            }
        }
        if (rec.energyCost > 0) StopPowerRequest();
        foreach(KeyValuePair<ResourceSO,int>prodotto in rec.outputs)
        {
            output.TryGetValue(prodotto.Key, out int attuale);
            output[prodotto.Key] = attuale+prodotto.Value;
        }
        lavoro = false;
        ricOra = null;
        progresso = 0f;
        tryAutoStart();
    }
}
public enum MachineType
{
    None,
    Advanced_Assembler,
    Alloy_Furnace,
    Assembler,
    Press,
    Smelter,
}
