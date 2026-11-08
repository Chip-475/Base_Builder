using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    static readonly List<ConstructionSite> sites = new();

    public static IReadOnlyList<ConstructionSite> Sites => sites;

    public BuildingView Prefab { get; private set; }
    public BuildingData Data => Prefab != null ? Prefab.Data : null;
    public Vector3Int Coords => transform.position.ToVector3Int();
    public bool IsPending => !completed && Prefab != null;
    public BuilderBot AssignedBuilder { get; private set; }

    SpriteRenderer spriteRenderer;
    bool completed;

    private void Awake()
    {
        if (!sites.Contains(this))
            sites.Add(this);
    }

    public static ConstructionSite Create(BuildingView prefab, Vector3 position)
    {
        if (prefab == null || prefab.Data == null)
        {
            Debug.LogError("Cannot create a construction site without a building prefab and data.");
            return null;
        }

        GameObject siteObject = new($"Construction Site - {prefab.Data.name}");
        siteObject.transform.position = position;

        ConstructionSite site = siteObject.AddComponent<ConstructionSite>();
        site.Initialize(prefab);
        return site;
    }

    void Initialize(BuildingView prefab)
    {
        Prefab = prefab;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Data.sprite;
        spriteRenderer.color = new Color(1f, 0.75f, 0.15f, 0.65f);

        ReserveCells();
    }

    public static ConstructionSite FindClosestAvailable(Vector3 position, BuilderBot builder)
    {
        ConstructionSite closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (ConstructionSite site in sites)
        {
            if (site == null || !site.IsPending || !site.CanBeClaimedBy(builder))
                continue;

            float distance = (site.transform.position - position).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = site;
            closestDistance = distance;
        }

        if (closest != null)
            closest.AssignedBuilder = builder;

        return closest;
    }

    bool CanBeClaimedBy(BuilderBot builder)
    {
        return AssignedBuilder == null
            || AssignedBuilder == builder
            || AssignedBuilder.SceneObj == null;
    }

    public void Release(BuilderBot builder)
    {
        if (AssignedBuilder == builder)
            AssignedBuilder = null;
    }

    public void Complete(BuilderBot builder)
    {
        if (!IsPending || AssignedBuilder != builder)
            return;

        completed = true;
        AssignedBuilder = null;
        ReleaseCells();

        Instantiate(Prefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    void ReserveCells()
    {
        foreach (Cell cell in GetCellsInBounds())
            if (cell != null)
                cell.canBuildOn = false;
    }

    void ReleaseCells()
    {
        foreach (Cell cell in GetCellsInBounds())
            if (cell != null && cell.Building == null)
                cell.canBuildOn = true;
    }

    IEnumerable<Cell> GetCellsInBounds()
    {
        Bounds bounds = Data.bounds;
        bounds.center += transform.position;

        int minX = Mathf.CeilToInt(bounds.min.x);
        int maxX = Mathf.FloorToInt(bounds.max.x);
        int minY = Mathf.CeilToInt(bounds.min.y);
        int maxY = Mathf.FloorToInt(bounds.max.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Cell cell = WorldManager.World.GetCellAt(new Vector3Int(x, y, 0));
                if (cell != null)
                    yield return cell;
            }
        }
    }

    private void OnDestroy()
    {
        sites.Remove(this);

        if (!completed && Data != null)
            ReleaseCells();
    }
}
