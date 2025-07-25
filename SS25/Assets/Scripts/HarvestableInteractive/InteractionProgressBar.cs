using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class InteractionProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;

    private float duration;
    private float timer;
    private bool isFilling;
    private Action onComplete;

    public void StartBar(float time, Action callback)
    {
        duration = time;
        timer = 0f;
        isFilling = true;
        onComplete = callback;
        fillImage.fillAmount = 0f;
        gameObject.SetActive(true);
    }

    public void CancelBar()
    {
        isFilling = false;
        fillImage.fillAmount = 0f;
        gameObject.SetActive(false);
        onComplete = null;
    }

    private void Update()
    {
        if (!isFilling) return;

        timer += Time.deltaTime;
        fillImage.fillAmount = timer / duration;

        if (timer >= duration)
        {
            isFilling = false;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
