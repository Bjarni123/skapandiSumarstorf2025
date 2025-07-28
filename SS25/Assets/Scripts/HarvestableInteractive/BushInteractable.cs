using UnityEngine;
using System.Collections;

public class BushInteractable : Interactable
{
    [SerializeField]
    private GameObject berryBush;

    [SerializeField]
    private GameObject bush;

    [SerializeField]
    private GameObject berriesPrefab;

    [SerializeField]
    private float pickTime = 1.5f;

    private int berryAmount = 3;
    
    private float regrowDelay = 60f;

    [SerializeField]
    private float fadeDuration = 0.5f;
    
    [SerializeField]
    private InteractionProgressBar progressBar;

    private bool isHarvested = false;

    public override void Interact()
    {
        if (isChopping || isHarvested || !isPlayerInRange)
            return;

        isChopping = true;
        progressBar.StartBar(pickTime, OnPickComplete);
    }

    protected override void CancelChop()
        {
            isChopping = false;
            progressBar.CancelBar();
            Debug.Log("Picking cancelled.");
        }

    private void OnPickComplete()
    {
        isChopping = false;
        isHarvested = true;

        Debug.Log("Berries picked!");
        StartCoroutine(FadeOutBerryBush());
        StartCoroutine(RegrowBushAfterDelay());

        for (int i = 0; i < berryAmount; i++)
        {
            float xOffset = Random.Range(0f, 1f);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);
            GameObject droppedBerries = Instantiate(berriesPrefab, spawnPos, Quaternion.identity);

            float verticalDistance = Random.Range(1f, 1.5f);
            StartCoroutine(SlideBerriesDown(droppedBerries, verticalDistance, 0.5f));
        }
    }

    private IEnumerator RegrowBushAfterDelay()
    {
        yield return new WaitForSeconds(regrowDelay);

        bush.SetActive(false);

        berryBush.SetActive(true);

        isHarvested = false;

        Debug.Log("Berries has regrown!");
    }
    private IEnumerator FadeOutBerryBush()
    {
        bush.SetActive(true);

        // Get all SpriteRenderers inside the fullTree object
        SpriteRenderer[] renderers = berryBush.GetComponentsInChildren<SpriteRenderer>();

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

        berryBush.SetActive(false);

        // Reset fullTree sprites to opaque so it's ready when regrown
        foreach (var sr in renderers)
        {
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
        }
    }

    private IEnumerator SlideBerriesDown(GameObject berries, float distance, float duration)
    {
        if (berries == null) yield break;

        Vector3 startPos = berries.transform.position;
        Vector3 endPos = startPos + new Vector3(0, -distance, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (berries == null) yield break;

            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 2f);
            berries.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (berries != null)
            berries.transform.position = endPos;
    }
}
