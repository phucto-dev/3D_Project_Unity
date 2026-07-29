using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("--- UI REF ---")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _npcNameText;
    [SerializeField] private TMP_Text _dialogueText;

    private Queue<string> _sentences;
    private Action _onDialogueCompleteCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        _sentences = new Queue<string>();
    }
    private void Start()
    {
        _dialoguePanel.SetActive(false);
    }
    public void StartDialogue(DialogueSO dialogueData, Action onCompleteCallback)
    {
        Debug.Log("Start goi dialogue");
        _dialoguePanel.SetActive(true);
        _npcNameText.SetText(dialogueData.NPCName);
        _onDialogueCompleteCallback = onCompleteCallback;

        _sentences.Clear();
        foreach (string sentence in dialogueData.Sentences)
        {
            _sentences.Enqueue(sentence);
        }
        SetPlayerActionInputMapDialogue();
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();
        _dialogueText.SetText(sentence);
    }

    private void EndDialogue()
    {
        _dialoguePanel.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.Playing);
        GameManager.Instance.ChangeActionInputInvoke(ActionInputMapType.Player);
        _onDialogueCompleteCallback?.Invoke();
    }
    private void SetPlayerActionInputMapDialogue()
    {
        GameManager.Instance.ChangeGameState(GameState.Playing);
        GameManager.Instance.ChangeActionInputInvoke(ActionInputMapType.Interaction);
    }
}

