using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    public List<IInteractable> interactables = new();
    [SerializeField] float interactionRange;

    public IInteractable currentInteractable;
    public GameObject interactionPrompt;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        currentInteractable = FindClosestInteractable();
        if (currentInteractable == null) interactionPrompt.SetActive(false);
        else interactionPrompt.SetActive(true);
    }

    IInteractable FindClosestInteractable()
    {
        if (PlayerManager.Instance.IsCarryingBot)
            return currentInteractable;

        List<IInteractable> allInteractables = new(interactables);
        List<IInteractable> inRange = new();
        IInteractable toReturn = null;
        float dis = float.MaxValue;

        foreach (var interactable in allInteractables)
            foreach(var cell in interactable.InteractionCells)
                if (cell == PlayerManager.Instance.GetPlayerCell())
                {
                    inRange.Add(interactable);
                    break;
                }

        foreach (var interactable in inRange)
        {
            var distance = Vector3.Distance(interactable.Position, PlayerManager.Instance.transform.position);
            if ( distance < dis)
            {
                toReturn = interactable;
                dis = distance;
            }
        }

        return toReturn;
    }
}
