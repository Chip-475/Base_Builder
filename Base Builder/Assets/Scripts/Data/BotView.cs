using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BotView : MonoBehaviour
{
    [SerializeField] BotData data;
    public Bot RuntimeObj { get; private set; }

    [Header("Components")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    List<Vector3Int> path = new();
    public Bot Bot { get; private set; }
    public bool IsMoving { get; private set; }

    void Start()
    {
        RuntimeObj = new Bot(this, data);
    }

    public bool SetDestination(Vector3Int coords)
    {/*
        var myPos = transform.position.ToVector3Int();
        Pathfinder.Pathfind(myPos, coords, out List<Vector3Int> path);
        this.path = path;*/
        Vector3Int myPos = transform.position.ToVector3Int();
        Pathfinder.Pathfind(myPos, coords, out List<Vector3Int> newPath);
        if (newPath == null || newPath.Count == 0)
        {
            path = new List<Vector3Int>();
            return false;
        }
        path = newPath;
        return true;
    }

    public void StartMoving()
    {
        StopMoving();
        if(path.Count > 0)
            StartCoroutine(FollowPath());
    }
    public void StopMoving()
    {
        StopAllCoroutines();
        IsMoving = false;
    }
    IEnumerator FollowPath()
    {
        IsMoving = true;
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 target = path[i];

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    data.speed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = target;
        }
    }
}
