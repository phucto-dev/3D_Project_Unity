using UnityEngine;
using UnityEngine.Events;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("--- NPC DATA ---")]
    [SerializeField] private DialogueSO _dialogueData;

    [Header("Events Trigger")]
    [SerializeField] private ObjectiveChainSO _targetChain;
    public InteractType GetInteractType() => InteractType.Talk;
    public int InteractionPriority => 5;
    public string GetInteractText() => "Talk";

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(_dialogueData, CompleteInteraction);
    }

    private void CompleteInteraction()
    {
        if (_targetChain != null && ObjectiveEventManager.Instance != null)
        {
            ObjectiveEventManager.Instance.StartObjectiveChain(_targetChain);
        }
    }
}
