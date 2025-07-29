using UnityEngine;
using System.Collections;

public class StoneInteractable : Interactable
{
    [SerializeField]
    private GameObject fullStone;

    [SerializeField]
    private GameObject stone2;

    [SerializeField]
    private GameObject orePrefab;

    private float regrowDelay = 6f;

    [SerializeField]
    private float fadeDuration = 0.5f;

    [SerializeField]
    private InteractionProgressBar progressBar;

    private bool isMined = false;

    public override void Interact()
    {
        if (isMined || isChopping || !isPlayerInRange)
            return;

        var player = GameObject.FindWithTag("Player");
        var weapon = player?.GetComponent<AgentWeapon>();
        var pickaxe = weapon?.GetPickaxe();

        if (pickaxe == null)
        {
            Debug.Log("You need a pickaxe to mine this!");
            return;
        }

        float mineSpeed = weapon.GetPickaxeParameter("Chop Speed");
        
        isChopping = true;

        progressBar.StartBar(mineSpeed, OnMineComplete);
    }

    protected override void CancelChop()
    {
        isChopping = false;
        progressBar.CancelBar();
        Debug.Log("Mining cancelled.");
    }

    private void OnMineComplete()
    {
        if (isMined)
            return;

        isChopping = false;
        isMined = true;

        Debug.Log("Stone mined!");
        StartCoroutine(FadeOutStone());
        StartCoroutine(RegrowStoneAfterDelay());

        var player = GameObject.FindWithTag("Player");
        var weapon = player?.GetComponent<AgentWeapon>();

        int dropMin = Mathf.RoundToInt(weapon?.GetPickaxeParameter("Drop Min") ?? 3);
        int dropMax = Mathf.RoundToInt(weapon?.GetPickaxeParameter("Drop Max") ?? 3);

        int oreAmount = Random.Range(dropMin, dropMax + 1);

        for (int i = 0; i < oreAmount; i++)
        {
            float xOffset = Random.Range(0f, 1f);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);
            GameObject droppedStone = Instantiate(orePrefab, spawnPos, Quaternion.identity);

            float verticalDistance = Random.Range(0.5f, 0.8f);
            StartCoroutine(SlideOreDown(droppedStone, verticalDistance, 0.5f));
        }
    }

    private IEnumerator RegrowStoneAfterDelay()
    {
        yield return new WaitForSeconds(regrowDelay);

        // Hide chopped trunk
        stone2.SetActive(false);

        // Show full tree
        fullStone.SetActive(true);

        // Allow tree to be chopped again
        isMined = false;

        Debug.Log("Stone has regrown! TT");
    }

    private IEnumerator FadeOutStone()
    {
        stone2.SetActive(true);

        // Get all SpriteRenderers inside the fullTree object
        SpriteRenderer[] renderers = fullStone.GetComponentsInChildren<SpriteRenderer>();

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

        fullStone.SetActive(false);

        // Reset fullTree sprites to opaque so it's ready when regrown
        foreach (var sr in renderers)
        {
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
        }
    }

    private IEnumerator SlideOreDown(GameObject ore, float distance, float duration)
    {
        if (ore == null) yield break;

        Vector3 startPos = ore.transform.position;
        Vector3 endPos = startPos + new Vector3(0, -distance, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (ore == null) yield break;

            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 2f); // Ease out
            ore.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (ore != null)
            ore.transform.position = endPos;
    }
}
