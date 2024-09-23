using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActivateAndFadeObject : MonoBehaviour
{
    public GameObject targetObject; // The GameObject to activate and fade
    public float fadeDuration = 1f; // Duration of the fade animation
    public CanvasGroup canvasGroup; // The CanvasGroup component of the target object
    public List<GameObject> objectsToDeactivate; // List of GameObjects to deactivate

    private bool isFading = false; // Flag to track if a fade animation is in progress
    private bool deactivationScheduled = false; // Flag to track if deactivation is scheduled

    public void OnButtonClick()
    {
        // Activate the target object
        targetObject.SetActive(true);

        // Start the fade animation
        FadeIn();

        // Deactivate other objects
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }

        // Schedule the deactivation after 10 seconds
        Invoke("DeactivateObject", 10f);
        deactivationScheduled = true; // Set deactivation scheduled flag
    }

    private void FadeIn()
    {
        if (!isFading && canvasGroup != null)
        {
            isFading = true;
            canvasGroup.alpha = 0f; // Start with transparent
            LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setOnComplete(() => isFading = false); // Fade in
        }
    }

    private void FadeOut()
    {
        if (!isFading && canvasGroup != null)
        {
            isFading = true;
            LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setOnComplete(() => isFading = false); // Fade out
        }
    }

    private void DeactivateObject()
    {
        // If deactivation is scheduled, cancel the fade-out animation
        if (deactivationScheduled)
        {
            FadeOut();
        }
        else
        {
            // If deactivation is not scheduled, proceed with the fade-out animation
            DisableGameObject();
        }
    }

    private void DisableGameObject()
    {
        // Deactivate the target object
        targetObject.SetActive(false);

        // Activate other objects
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(true);
        }
    }
}
