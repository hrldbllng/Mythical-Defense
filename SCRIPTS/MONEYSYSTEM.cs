using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public int startingMoney = 100;
    public int currentMoney;

    public Text moneyText; // Reference to the UI Text element
    public GameObject floatingTextPrefab; // Reference to the floating text prefab
    public Transform floatingTextSpawnPoint; // Spawn point for floating text

    void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyText();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyText();
        ShowFloatingText("+" + amount, Color.green);
    }

    public void SubtractMoney(int amount)
    {
        currentMoney -= amount;
        UpdateMoneyText();
        ShowFloatingText("-" + amount, Color.red);
    }

    void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString(); // Update the text of the UI element
        }
    }

    void ShowFloatingText(string text, Color color)
    {
        // Instantiate floating text prefab
        GameObject floatingTextObject = Instantiate(floatingTextPrefab, floatingTextSpawnPoint.position, Quaternion.identity);
        floatingTextObject.transform.SetParent(floatingTextSpawnPoint, false);
        Text floatingText = floatingTextObject.GetComponent<Text>();
        floatingText.text = text;

        // Set text color
        floatingText.color = color;

        // Start the floating and fading coroutine
        StartCoroutine(FloatingTextEffect(floatingTextObject));
    }

    IEnumerator FloatingTextEffect(GameObject floatingTextObject)
    {
        Text floatingText = floatingTextObject.GetComponent<Text>();
        float duration = 1f; // Duration of floating and fading
        float startTime = Time.time;

        // Floating speed and direction
        float speed = 1f;
        float floatHeight = 1f;
        Vector3 startPosition = floatingTextSpawnPoint.position;
        Vector3 endPosition = startPosition + Vector3.up * floatHeight;

        // Fade speed
        float fadeSpeed = 1f;

        while (Time.time < startTime + duration)
        {
            // Move the floating text upward
            floatingTextObject.transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / duration);

            // Fade out the floating text
            float alpha = 1 - (Time.time - startTime) / duration;
            floatingText.color = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, alpha);

            yield return null;
        }

        // Move the floating text downward while fading out
        while (floatingText.color.a > 0)
        {
            floatingTextObject.transform.position -= Vector3.up * speed * Time.deltaTime;

            float alpha = floatingText.color.a - fadeSpeed * Time.deltaTime;
            floatingText.color = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, alpha);

            yield return null;
        }

        // Destroy the floating text object after fading out
        Destroy(floatingTextObject);
    }
}
