using UnityEngine;

public class PowerUpEffect : MonoBehaviour
{
    public int healthBonus = 100;
    public float immuneDuration = 5f;
    public int moneyBonus = 50; // Amount of money to add to the player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collides with the power-up item
        if (collision.CompareTag("JuanD"))
        {
            // Activate the power-up effect
            ActivatePowerUp();

            // Destroy the power-up item
            Destroy(gameObject);
        }
    }

    public void ActivatePowerUp()
    {
        // Find and activate the JuanD script
        JuanD juandScript = GameObject.FindGameObjectWithTag("JuanD").GetComponent<JuanD>();
        if (juandScript != null)
        {
            juandScript.IncreaseHealth(healthBonus);
            juandScript.ActivateImmunity(immuneDuration);
        }

        // Find and add money to the player
        MoneyManager moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager != null)
        {
            moneyManager.AddMoney(moneyBonus);
        }
    }
}
