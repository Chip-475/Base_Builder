using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class BotView : MonoBehaviour
{
    public BotData data;
    public Bot RuntimeObj { get; private set; }

    [Header("Components")]
    public CircleCollider2D collider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    List<Vector3Int> path = new();

    void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        RuntimeObj = new Bot(this, data);
    }
    private void Update()
    {
        RuntimeObj.InteractionBounds = new(transform.position, new Vector3(2f, 2f, 0));
    }

    public void SetDestination(Vector3Int coords)
    {
        var myPos = transform.position.ToVector3Int();
        Pathfinder.Pathfind(myPos, coords, out List<Vector3Int> path);
        this.path = path;
    }

    public void StartMoving()
    {
        if(path.Count > 0)
            StartCoroutine(FollowPath());
    }
    public void StopMoving()
    {
        StopAllCoroutines();
    }
    IEnumerator FollowPath()
    {
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
