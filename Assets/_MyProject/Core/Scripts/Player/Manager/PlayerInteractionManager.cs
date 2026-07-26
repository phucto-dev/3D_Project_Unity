using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    int InteractionPriority { get; }

    string GetInteractText();

    void Interact();
}
public class PlayerInteractionManager : MonoBehaviour
{
    public PlayerInventorySO PlayerInventory;
    private PlayerInput _inputSystem;
    private InputAction _lootAction;
    private List<DropInfo> _listDrop;
    private List<IInteractable> _interactablesInRange = new List<IInteractable>();

    private void Awake()
    {
        _inputSystem = GetComponentInParent<PlayerInput>();
        if (_inputSystem == null) return;
        _lootAction = _inputSystem.actions["Loot"];
    }
    private void OnEnable()
    {
        _lootAction.performed += HandleLootInput;
    }
    private void OnDisable()
    {
        _lootAction.performed -= HandleLootInput;
    }
    private void Start()
    {
        _listDrop = new List<DropInfo>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagDropItem) && other.TryGetComponent<DropInfo>(out DropInfo item))
        {
            if (!_listDrop.Contains(item))
            {
                _listDrop.Add(item);
            }
        }

        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (!_interactablesInRange.Contains(interactable))
            {
                _interactablesInRange.Add(interactable);
            }
        }

        UpdateCurrentInteraction();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagConstant.TagDropItem) && other.TryGetComponent<DropInfo>(out DropInfo item))
        {
            _listDrop.Remove(item);
        }

        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            _interactablesInRange.Remove(interactable);
        }

        UpdateCurrentInteraction();
    }
    private void HandleLootInput(InputAction.CallbackContext ctx)
    {
        Debug.Log("Click F");
        LootItem();
    }

    private void LootItem()
    {
        if (PlayerInventory == null || _listDrop.Count <= 0) return;

        var targetDrop = _listDrop[0];
        int leftOverItems = PlayerInventory.AddItem(targetDrop.ItemData);

        if (leftOverItems > 0)
        {
            targetDrop.SetAmount(leftOverItems);
            Debug.Log("Inventory full");
            return;
        }

        if (targetDrop is IInteractable interact)
        {
            _interactablesInRange.Remove(interact);
        }

        targetDrop.RemovePrefab();
        _listDrop.RemoveAt(0);

        UpdateCurrentInteraction();

        Debug.Log("Got it");
    }

    private void UpdateCurrentInteraction()
    {
        if (_interactablesInRange == null || _interactablesInRange.Count == 0)
        {
            MainMenuController.Instance.HideInteractionUI();
            return;
        }

        IInteractable highestPriorityInteractable = null;
        int maxPriority = 0;

        foreach (var interact in _interactablesInRange)
        {
            if (interact.InteractionPriority > maxPriority)
            {
                maxPriority = interact.InteractionPriority;
                highestPriorityInteractable = interact;
            }
        }

        if (highestPriorityInteractable != null)
        {
            MainMenuController.Instance.SetInteractionText($"[{_lootAction.GetBindingDisplayString()}] {highestPriorityInteractable.GetInteractText()}");
            MainMenuController.Instance.ShowInteractionUI();
        }
    }
}