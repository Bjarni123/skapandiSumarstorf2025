using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScriptableNPC : MonoBehaviour
{
    [Header("NPC Configuration")]
    public NPCData npcData;
    
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public UnityEngine.UI.Image portraitImage; // New portrait image component
    public TextMeshProUGUI interactionHintText; // Shows "Press E to continue" etc.
    public GameObject interactionPrompt;
    
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    
    private bool playerInRange = false;
    private bool isDialogueActive = false;
    private bool hasBeenTalkedTo = false;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private Coroutine autoProgressCoroutine;
    private bool isTyping = false;
    private AudioSource audioSource;
    
    void Start()
    {
        InitializeNPC();
        HideUI();
    }
    
    void InitializeNPC()
    {
        if (npcData == null)
        {
            Debug.LogError("No NPC Data assigned to " + gameObject.name);
            return;
        }
        
        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Set sprite if available
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && npcData.npcSprite != null)
        {
            spriteRenderer.sprite = npcData.npcSprite;
        }
    }
    
    void HideUI()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (interactionHintText) interactionHintText.gameObject.SetActive(false);
    }
    
    void Update()
    {
        HandleInteraction();
        HandleDialogueProgression();
    }
    
    void HandleInteraction()
    {
        if (playerInRange && !isDialogueActive)
        {
            if (interactionPrompt && !interactionPrompt.activeInHierarchy)
                interactionPrompt.SetActive(true);
                
            if (Input.GetKeyDown(interactKey))
            {
                StartDialogue();
            }
        }
    }
    
    void HandleDialogueProgression()
    {
        if (!isDialogueActive) return;
        
        if (Input.GetKeyDown(interactKey))
        {
            if (isTyping)
            {
                CompleteCurrentLine();
            }
            else
            {
                NextLine();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt) interactionPrompt.SetActive(false);
            if (interactionHintText) interactionHintText.gameObject.SetActive(false);
        }
    }
    
    void StartDialogue()
    {
        if (npcData == null) return;
        
        isDialogueActive = true;
        currentLineIndex = 0;
        
        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(true);
        
        // Choose dialogue based on interaction history
        NPCData.DialogueLine[] currentDialogue = GetCurrentDialogue();
        
        if (currentDialogue.Length > 0)
        {
            if (speakerNameText) speakerNameText.text = npcData.npcName;
            
            // Set portrait image
            if (portraitImage && npcData.portraitImage != null)
            {
                portraitImage.sprite = npcData.portraitImage;
                portraitImage.gameObject.SetActive(true);
            }
            
            DisplayLine(currentDialogue[0]);
        }
        
        if (!hasBeenTalkedTo) hasBeenTalkedTo = true;
    }
    
    NPCData.DialogueLine[] GetCurrentDialogue()
    {
        // You can add custom logic here for conditional dialogue
        if (npcData.hasConditionalDialogue && ShouldUseConditionalDialogue())
        {
            return npcData.conditionalDialogue;
        }
        
        if (hasBeenTalkedTo && npcData.repeatDialogue.Length > 0)
        {
            return npcData.repeatDialogue;
        }
        
        return npcData.firstTimeDialogue;
    }
    
    bool ShouldUseConditionalDialogue()
    {
        // Override this method or add your custom conditions here
        // For example: check player inventory, quest status, etc.
        return false;
    }
    
    void DisplayLine(NPCData.DialogueLine line)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoProgressCoroutine != null) StopCoroutine(autoProgressCoroutine);
        
        typingCoroutine = StartCoroutine(TypeLine(line));
    }
    
    IEnumerator TypeLine(NPCData.DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        // Hide interaction hint while typing
        if (interactionHintText) interactionHintText.gameObject.SetActive(false);
        
        // Play talking sound
        PlayTalkingSound(line);
        
        foreach (char letter in line.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(line.typeSpeed);
        }
        
        isTyping = false;
        
        // Handle pause after line
        if (line.pauseAfterLine)
        {
            yield return new WaitForSeconds(line.pauseDuration);
        }
        
        // Show appropriate interaction hint
        if (!npcData.autoProgressLines)
        {
            ShowContinueHint();
        }
        else
        {
            // Auto-progress to next line
            autoProgressCoroutine = StartCoroutine(AutoProgressLine());
        }
    }
    
    void ShowContinueHint()
    {
        if (interactionHintText)
        {
            NPCData.DialogueLine[] currentDialogue = GetCurrentDialogue();
            bool isLastLine = currentLineIndex >= currentDialogue.Length - 1;
            
            if (isLastLine)
            {
                interactionHintText.text = "Press " + interactKey.ToString() + " to close";
            }
            else
            {
                interactionHintText.text = "Press " + interactKey.ToString() + " to continue";
            }
            
            interactionHintText.gameObject.SetActive(true);
        }
    }
    
    IEnumerator AutoProgressLine()
    {
        yield return new WaitForSeconds(npcData.autoProgressDelay);
        NextLine();
    }
    
    void PlayTalkingSound(NPCData.DialogueLine line)
    {
        if (audioSource == null) return;
        
        AudioClip soundToPlay = line.customSound != null ? line.customSound : npcData.talkingSound;
        float pitchToUse = line.customSound != null ? line.customPitch : npcData.soundPitch;
        
        if (soundToPlay != null)
        {
            audioSource.clip = soundToPlay;
            audioSource.pitch = pitchToUse;
            audioSource.volume = npcData.soundVolume;
            audioSource.Play();
        }
    }
    
    void CompleteCurrentLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoProgressCoroutine != null) StopCoroutine(autoProgressCoroutine);
        
        NPCData.DialogueLine[] currentDialogue = GetCurrentDialogue();
        if (currentLineIndex < currentDialogue.Length)
        {
            dialogueText.text = currentDialogue[currentLineIndex].text;
        }
        
        isTyping = false;
        
        if (!npcData.autoProgressLines)
        {
            ShowContinueHint();
        }
        else
        {
            autoProgressCoroutine = StartCoroutine(AutoProgressLine());
        }
    }
    
    void NextLine()
    {
        currentLineIndex++;
        NPCData.DialogueLine[] currentDialogue = GetCurrentDialogue();
        
        if (currentLineIndex < currentDialogue.Length)
        {
            DisplayLine(currentDialogue[currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }
    
    public void EndDialogue()
    {
        isDialogueActive = false;
        currentLineIndex = 0;
        
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoProgressCoroutine != null) StopCoroutine(autoProgressCoroutine);
        
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (interactionHintText) interactionHintText.gameObject.SetActive(false);
        if (portraitImage) portraitImage.gameObject.SetActive(false);
        
        if (playerInRange && interactionPrompt) 
        {
            interactionPrompt.SetActive(true);
            if (interactionHintText) 
            {
                interactionHintText.text = "Press " + interactKey.ToString() + " to talk";
                interactionHintText.gameObject.SetActive(true);
            }
        }
    }
}