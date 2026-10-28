using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BotView : MonoBehaviour
{
    public BotData data;
    public Bot RuntimeObj { get; protected set; }

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Image powerBar;

    protected virtual void Start()
    {
        RuntimeObj = new Bot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
    private void Update()
    {
        RuntimeObj.InteractionBounds = new(transform.position, new Vector3(2f, 2f, 0));
        powerBar.fillAmount = RuntimeObj.Power / 100;
    }

    public void StartMoving()
    {
        if(RuntimeObj.Path.Count > 0)
            StartCoroutine(FollowPath());
    }
    public void StopMoving()
    {
        StopAllCoroutines();
    }
    IEnumerator FollowPath()
    {
        for (int i = 1; i < RuntimeObj.Path.Count; i++)
        {
            Vector3 target = RuntimeObj.Path[i];

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
