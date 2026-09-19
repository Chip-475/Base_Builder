using UnityEngine;
using System;

public class BotView : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] BotType type;

    [Header("Components")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    void Start()
    {
        new Bot(this, type: type);
    }
}
