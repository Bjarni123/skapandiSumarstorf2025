using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TreeInteractable : Interactable
{
    [SerializeField]
    private GameObject fullTree;       // Trunk + Canopy

    [SerializeField]
    private GameObject choppedTrunk;      // The new chopped sprite

    [SerializeField]
    private GameObject logPrefab;     // The log that will be dropped

    [SerializeField] 
    private float regrowDelay = 5f;

    [SerializeField]
    private float fadeDuration = 0.5f;     // Optional fade time

    // [SerializeField]
    // private InteractionProgressBar progressBar; // Reference to the interaction progress bar

    [SerializeField]
    private float chopTime = 2f;

    private bool isChopped = false;

    public override void Interact()
    {
        if (isChopped || isChopping)
        {
            Debug.Log("Cannot chop right now.");
            return;
        }

        if (!isPlayerInRange)
        {
            Debug.Log("You're not close enough to chop.");
            return;
        }

        isChopping = true;

        // progressBar.StartBar(chopTime, OnChopComplete);

        // TODO: drop wood based on tool, play animation, destroy tree
    }

    protected override void CancelChop()
    {
        isChopping = false;
        // progressBar.CancelBar();
        Debug.Log("Chop cancelled: player walked away.");
    }

    private void OnChopComplete()
    {
        if (isChopped)
            return;

        isChopping = false;
        isChopped = true;

        Debug.Log("Tree chopped!");
        StartCoroutine(FadeOutFullTree());
        StartCoroutine(RegrowTreeAfterDelay());

        for (int i = 0; i < 3; i++)
        {
            float xOffset = Random.Range(0f, 1f);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);
            GameObject droppedLog = Instantiate(logPrefab, spawnPos, Quaternion.identity);

            float verticalDistance = Random.Range(0.5f, 0.8f);
            StartCoroutine(SlideLogDown(droppedLog, verticalDistance, 0.5f));
        }
    }

    private IEnumerator RegrowTreeAfterDelay()
    {
        yield return new WaitForSeconds(regrowDelay);

        // Hide chopped trunk
        choppedTrunk.SetActive(false);

        // Show full tree
        fullTree.SetActive(true);

        // Allow tree to be chopped again
        isChopped = false;

        Debug.Log("Tree has regrown!");
    }

    private IEnumerator FadeOutFullTree()
    {
        choppedTrunk.SetActive(true);

        // Get all SpriteRenderers inside the fullTree object
        SpriteRenderer[] renderers = fullTree.GetComponentsInChildren<SpriteRenderer>();

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeDuration);

            foreach (var sr in renderers)
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }

            yield return null;
        }

        fullTree.SetActive(false);
        
        // Reset fullTree sprites to opaque so it's ready when regrown
        foreach (var sr in renderers)
        {
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
        }
    }

    private IEnumerator SlideLogDown(GameObject log, float distance, float duration)
    {
        Vector3 startPos = log.transform.position;
        Vector3 endPos = startPos + new Vector3(0, -distance, 0);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Ease out: fast then slow
            t = 1f - Mathf.Pow(1f - t, 2f);
            log.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        log.transform.position = endPos;
    }
}

