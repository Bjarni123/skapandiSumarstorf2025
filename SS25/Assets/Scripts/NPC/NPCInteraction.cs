using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string text;
    public float textSpeed = 0.05f;
}

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue Data")]
    public List<DialogueLine> DialogueLines = new List<DialogueLine>();
    public bool HasBeenTalkedTo = false;
    public List<DialogueLine> RepeatDialogue = new List<DialogueLine>();
    
    [Header("UI References")]
    public GameObject DialoguePanel;
    public TextMeshProUGUI SpeakerNameText;
    public TextMeshProUGUI DialogueText;
    public GameObject ContinuePrompt;

    
    [Header("Interaction")]
    public KeyCode InteractKey = KeyCode.E;
    public GameObject InteractionPrompt;
    
    private bool _playerInRange = false;
    private bool _isDialogueActive = false;
    private int _currentLineIndex = 0;
    private Coroutine _typingCoroutine;
    private bool _isTyping = false;
    
    void Start()
    {
        // Hide UI elements at start
        if (DialoguePanel) DialoguePanel.SetActive(false);
        if (InteractionPrompt) InteractionPrompt.SetActive(false);
        if (ContinuePrompt) ContinuePrompt.SetActive(false);
    }
    
    void Update()
    {
        // Show interaction prompt when player is near
        if (_playerInRange && !_isDialogueActive)
        {
            if (InteractionPrompt && !InteractionPrompt.activeInHierarchy)
                InteractionPrompt.SetActive(true);
                
            // Start dialogue when interaction key is pressed
            if (Input.GetKeyDown(InteractKey))
            {
                StartDialogue();
            }
        }
        
        // Handle dialogue progression
        if (_isDialogueActive && Input.GetKeyDown(InteractKey))
        {
            if (_isTyping)
            {
                // Skip typing animation
                CompleteCurrentLine();
            }
            else
            {
                // Go to next line
                NextLine();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            if (InteractionPrompt) InteractionPrompt.SetActive(false);
        }
    }
    
    void StartDialogue()
    {
        _isDialogueActive = true;
        _currentLineIndex = 0;
        
        // DEBUG: Check the values
        Debug.Log("HasBeenTalkedTo: " + HasBeenTalkedTo);
        Debug.Log("DialogueLines count: " + DialogueLines.Count);
        Debug.Log("RepeatDialogue count: " + RepeatDialogue.Count);
    
        // Hide interaction prompt
        if (InteractionPrompt) InteractionPrompt.SetActive(false);
        
        // Show dialogue panel
        if (DialoguePanel) DialoguePanel.SetActive(true);
        
        // Choose which dialogue to use
        List<DialogueLine> currentDialogue = (!HasBeenTalkedTo && DialogueLines.Count > 0) 
            ? DialogueLines 
            : RepeatDialogue;
        
        Debug.Log("Using dialogue with " + currentDialogue.Count + " lines");


        if (currentDialogue.Count > 0)
        {
            DisplayLine(currentDialogue[0]);
        }
        
        // Mark as talked to after first conversation
        if (!HasBeenTalkedTo) HasBeenTalkedTo = true;
    }
    
    void DisplayLine(DialogueLine line)
    {
        Debug.Log("Displaying line: " + line.text);
        
        // Set speaker name
        if (SpeakerNameText) SpeakerNameText.text = line.speakerName;
        
        // Start typing animation
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeLine(line));
    }
    
    IEnumerator TypeLine(DialogueLine line)
    {
        _isTyping = true;
        DialogueText.text = "";
        
        if (ContinuePrompt) ContinuePrompt.SetActive(false);
        
        foreach (char letter in line.text.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(line.textSpeed);
        }
        
        _isTyping = false;
        if (ContinuePrompt) ContinuePrompt.SetActive(true);
    }
    
    void CompleteCurrentLine()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        
        List<DialogueLine> currentDialogue = (HasBeenTalkedTo && RepeatDialogue.Count > 0) 
            ? RepeatDialogue 
            : DialogueLines;
            
        if (_currentLineIndex < currentDialogue.Count)
        {
            DialogueText.text = currentDialogue[_currentLineIndex].text;
        }
        
        _isTyping = false;
        if (ContinuePrompt) ContinuePrompt.SetActive(true);
    }
    
    void NextLine()
    {
        _currentLineIndex++;
        
        List<DialogueLine> currentDialogue = (HasBeenTalkedTo && RepeatDialogue.Count > 0) 
            ? RepeatDialogue 
            : DialogueLines;
        
        if (_currentLineIndex < currentDialogue.Count)
        {
            DisplayLine(currentDialogue[_currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }
    
    void EndDialogue()
    {
        _isDialogueActive = false;
        _currentLineIndex = 0;
        
        // Hide dialogue panel
        if (DialoguePanel) DialoguePanel.SetActive(false);
        if (ContinuePrompt) ContinuePrompt.SetActive(false);
        
        // Show interaction prompt again if player still in range
        if (_playerInRange && InteractionPrompt) 
            InteractionPrompt.SetActive(true);
    }
}