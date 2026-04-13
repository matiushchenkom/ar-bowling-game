using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the intro sequence of the game.
/// Handles smooth fading of text elements and the main background panel.
/// </summary>
public class PreviewManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The main background panel that fades out at the end")]
    public CanvasGroup panelCanvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI authorText;
    public TextMeshProUGUI descriptionText;

    [Header("Timing Settings")]
    [Tooltip("How fast the text fades (lower is slower)")]
    public float fadeSpeed = 0.4f; 
    
    [Tooltip("How long the title and author names stay visible")]
    public float titleDuration = 2.0f;

    [Tooltip("How long the project description stays visible")]
    public float descriptionDuration = 5.0f; 

    void Start()
    {
        // Initial state: Hidden text, visible panel
        titleText.alpha = 0;
        authorText.alpha = 0;
        descriptionText.alpha = 0;
        panelCanvasGroup.alpha = 1;
        
        StartCoroutine(PlayPreview());
    }

    /// <summary>
    /// Sequential intro animation using Coroutines for timing.
    /// </summary>
    IEnumerator PlayPreview()
    {
        // 1. Project Title fade in
        yield return StartCoroutine(FadeText(titleText, 0, 1));
        
        // 2. Author name fade in with a slight delay
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeText(authorText, 0, 1));
        
        yield return new WaitForSeconds(titleDuration);

        // 3. Simultaneous fade out of title and author
        StartCoroutine(FadeText(titleText, 1, 0));
        yield return StartCoroutine(FadeText(authorText, 1, 0));

        yield return new WaitForSeconds(0.5f); 

        // 4. Description fade in
        yield return StartCoroutine(FadeText(descriptionText, 0, 1));
        yield return new WaitForSeconds(descriptionDuration);

        // 5. Description fade out
        yield return StartCoroutine(FadeText(descriptionText, 1, 0));

        // 6. Background panel fade out
        float elapsed = 0;
        while (elapsed < 1.0f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            panelCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsed);
            yield return null;
        }

        // Deactivate the preview object to save resources
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Universal function to animate TextMeshProUGUI transparency.
    /// </summary>
    IEnumerator FadeText(TextMeshProUGUI text, float start, float end)
    {
        float elapsed = 0;
        while (elapsed < 1.0f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            text.alpha = Mathf.Lerp(start, end, elapsed);
            yield return null;
        }
        text.alpha = end;
    }
}