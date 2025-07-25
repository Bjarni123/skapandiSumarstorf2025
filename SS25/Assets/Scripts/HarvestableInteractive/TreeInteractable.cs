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

    private bool isChopped = false;

    public override void Interact()
    {
        if (isChopped == true)
        {
            Debug.Log("Tree already chopped!");
            return;
        }
        StartCoroutine(RegrowTreeAfterDelay());

        isChopped = true;

        fullTree.SetActive(false);
        choppedTrunk.SetActive(true);
        
        for (int i = 0; i < 3; i++)
        {
            float xOffset = Random.Range(0f, 1f);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);
            GameObject droppedLog = Instantiate(logPrefab, spawnPos, Quaternion.identity);

            float verticalDistance = Random.Range(0.5f, 0.8f);

            StartCoroutine(SlideLogDown(droppedLog, verticalDistance, 0.5f));
        }
        Debug.Log("Tree chopped!");

        // TODO: drop wood based on tool, play animation, destroy tree
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

