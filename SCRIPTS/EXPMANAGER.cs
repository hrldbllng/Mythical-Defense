using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public int maxExp = 100; // Maximum experience points
    public int currentExp = 0; // Current experience points
    public GameObject ultimateGameObject; // Reference to the GameObject for JuanD's ultimate ability
    public Slider expSlider; // Reference to the UI slider for experience points
    public Button ultimateButton; // Reference to the ultimate ability button

    private void Start()
    {
        // Disable the ultimate ability GameObject at the start
        if (ultimateGameObject != null)
        {
            ultimateGameObject.SetActive(false);
        }

        // Initialize the UI slider for experience points
        if (expSlider != null)
        {
            expSlider.maxValue = maxExp;
            expSlider.value = currentExp;
        }

        // Initialize the ultimate ability button
        if (ultimateButton != null)
        {
            ultimateButton.onClick.AddListener(ActivateUltimate);
            ultimateButton.gameObject.SetActive(false);
        }
    }

    public void GainExp(int exp)
    {
        // Add the gained experience points
        currentExp += exp;

        // Update the UI slider for experience points
        if (expSlider != null)
        {
            expSlider.value = currentExp;
        }

        // Check if the current experience points have reached the maximum value
        if (currentExp >= maxExp)
        {
            // Activate the ultimate ability button
            if (ultimateButton != null)
            {
                ultimateButton.gameObject.SetActive(true);
            }
        }
    }

    private void ActivateUltimate()
    {
        // Check if the ultimate GameObject is assigned
        if (ultimateGameObject != null)
        {
            // Activate the ultimate ability GameObject
            ultimateGameObject.SetActive(true);

            // Reset the experience points
            currentExp = 0;

            // Update the UI slider for experience points
            if (expSlider != null)
            {
                expSlider.value = currentExp;
            }

            // Deactivate the ultimate ability button
            if (ultimateButton != null)
            {
                ultimateButton.gameObject.SetActive(false);
            }
        }
    }
}
