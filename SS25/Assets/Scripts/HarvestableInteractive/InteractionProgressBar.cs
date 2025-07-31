using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class InteractionProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;

    [SerializeField]
    private TMPro.TextMeshProUGUI countdownText;


    private float duration;
    private float timer;
    private bool isFilling;
    private Action onComplete;
    
    private void Update()
    {
        if (!isFilling) return;

        timer += Time.deltaTime;
        fillImage.fillAmount = timer / duration;

        float timeLeft = Mathf.Clamp(duration - timer, 0f, duration);
        if (countdownText != null)
            // countdownText.text = Mathf.CeilToInt(timeLeft).ToString() + "s";
            countdownText.text = timeLeft.ToString("F1") + "s"; // Decimel second version

        if (timer >= duration)
        {
            isFilling = false;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }

    public void StartBar(float time, Action callback)
    {
        duration = time;
        timer = 0f;
        isFilling = true;
        onComplete = callback;
        fillImage.fillAmount = 0f;
        gameObject.SetActive(true);

        if (countdownText != null)
            countdownText.text = Mathf.CeilToInt(duration).ToString();
    }

    public void CancelBar()
    {
        isFilling = false;
        fillImage.fillAmount = 0f;
        gameObject.SetActive(false);
        onComplete = null;
    }
}
