using UnityEngine;
using System.Collections.Generic;
using System;
[DefaultExecutionOrder(10)]
public class MineralManagerUI : MonoBehaviour
{
    [Serializable]
    public class mineralZone
    {
        public string name = "zona";
        public GameObject prefab;
        [Min(1)] public int count = 3;
        //min e max per distanza per ogni zona
        public Vector2Int depositDist=new Vector2Int(5,9);
        [Min(1f)] public float raggio = 3f;
        //distanza miinima in celle
        public float minDist = 6f;
        //dist max dal punto di partenza
        public float maxDist = 15f;
    }

    [Header("setup")]
    [SerializeField] Transform startPoint;
    [SerializeField] Transform container;
    [SerializeField] List<mineralZone> zone = new List<mineralZone>();
    [SerializeField] float distGruppo = 12f;
    [SerializeField] int tent = 200;

    [Header("random")]
    [SerializeField] bool useRandom = true;
    [SerializeField] int seed = 12345;

    readonly List<Vector3Int> coord = new List<Vector3Int>();
    readonly HashSet<Vector3Int> occupato=new HashSet<Vector3Int>();
    readonly List<Vector3Int> centriGruppi=new List<Vector3Int>();
    readonly List<GameObject> spawned = new List<GameObject>();
    readonly List<Cell> celleBloccati = new List<Cell>();
    System.Random rng;
    
    void Start()
    {
        genera();
    }

    public void genera()
    {
        clear();
        if(WorldManager.Instance==null||WorldManager.World==null)
        {
            Debug.Log("word manager non worka");
            return;
        }
        int seedUsato = seed;
        if(useRandom)seedUsato=Environment.TickCount;
        rng = new System.Random(seedUsato);
        Debug.Log("seed " + seedUsato);
        celleLibere();
        Vector3Int start = Vector3Int.zero;
        if (startPoint != null) start = startPoint.position.ToVector3Int();
        foreach (mineralZone zo in zone)
        {
            if (zo.prefab == null)
            {
                Debug.Log("prefab null " + zo.name);
                continue;
            }
            int zoneCreate = 0;
            int depositiCreati = 0;
            for (int i = 0; i < zo.count; i++)
            {
                if(!trovaGruppi(zo, start, out Vector3Int centro)) continue;
                centriGruppi.Add(centro);
                zoneCreate++;
                //depositiCreati+=
            }
        }
    }

    private void clear()
    {
        foreach(Cell cell in celleBloccati)
        {
            cell.canWalkOn = true;
            cell.canBuildOn=true;
        }
        foreach(GameObject ogg in spawned)
        {
            if (ogg != null) Destroy(ogg);
        }
        celleBloccati.Clear();
        spawned.Clear();
        occupato.Clear();
        centriGruppi.Clear();
    }

    private void celleLibere()
    {
        coord.Clear();
        Bounds bounds = WorldManager.World.WorldBounds;
        int minX = Mathf.CeilToInt(bounds.min.x);
        int maxX=Mathf.FloorToInt(bounds.max.x);
        int minY=Mathf.CeilToInt(bounds.min.y);
        int maxY = Mathf.FloorToInt(bounds.max.y);
        for (int x = minX; x <= maxX; x++)
        {
            for(int y=minY; y <= maxY; y++)
            {
                Vector3Int coords = new Vector3Int(x, y, 0);
                Cell cell = WorldManager.World.GetCellAt(coords);
                if (cell != null && cell.canWalkOn && cell.canBuildOn) coord.Add(coords);
            }
        }
    }
    
    private bool trovaGruppi(mineralZone zone,Vector3Int start, out Vector3Int centro)
    {
        
    }
}
