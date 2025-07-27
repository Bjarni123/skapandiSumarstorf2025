using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New NPC Data", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("NPC Identity")]
    public string npcName = "Villager";
    public Sprite npcSprite;
    public Sprite portraitImage; // New portrait for dialogue UI
    
    [Header("Dialogue Settings")]
    public bool autoProgressLines = false;
    public float autoProgressDelay = 2f; // Time to wait before auto-progressing
    
    [Header("Audio Settings")]
    public AudioClip talkingSound;
    [Range(0.5f, 2f)]
    public float soundPitch = 1f;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;
    
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string text;
        
        [Range(0.01f, 0.2f)]
        public float typeSpeed = 0.05f;
        
        [Header("Line-Specific Audio (Optional)")]
        public AudioClip customSound;
        [Range(0.5f, 2f)]
        public float customPitch = 1f;
        
        [Header("Special Actions")]
        public bool pauseAfterLine = false;
        public float pauseDuration = 1f;
    }
    
    [Header("First Time Dialogue")]
    public DialogueLine[] firstTimeDialogue;
    
    [Header("Repeat Dialogue")]
    public DialogueLine[] repeatDialogue;
    
    [Header("Optional: Conditional Dialogue")]
    public bool hasConditionalDialogue = false;
    public DialogueLine[] conditionalDialogue;
    public string conditionDescription = "Describe when this dialogue should play";
}