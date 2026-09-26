using UnityEngine;
using System;

public class Bot_View : MonoBehaviour
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
