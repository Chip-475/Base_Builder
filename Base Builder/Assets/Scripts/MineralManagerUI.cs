using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
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
        
    }


}
