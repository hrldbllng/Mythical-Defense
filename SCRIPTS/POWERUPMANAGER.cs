using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RandomPowerUp : MonoBehaviour
{
    public GameObject[] powerUps; // Array of power-up prefabs
    public GameObject[] monsters; // Array of monster game objects
    public float powerUpDuration = 10f; // Duration of the power-up effect
    public float warningTime = 3f; // Time before the power-up is destroyed to start warning blink

    private TimeWatch timeWatch; // Reference to the TimeWatch component
    private float nextSpawnTime; // Time of the next power-up spawn
    private float spawnInterval; // Interval between power-up spawns

    private void Start()
    {
        // Find and assign the TimeWatch component in the scene
        timeWatch = FindObjectOfType<TimeWatch>();

        // Set the initial spawn interval
        SetSpawnInterval();

        // Schedule the first power-up spawn
        nextSpawnTime = Time.time + spawnInterval;

        // Start the coroutine to spawn power-ups
        StartCoroutine(SpawnPowerUps());
    }

    private void SetSpawnInterval()
    {
        // Set the spawn interval based on the elapsed time
        float elapsedTime = timeWatch.GetElapsedTime();
        spawnInterval = Mathf.Max(30f, 30f + elapsedTime * 0.1f);
    }

    IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            // Wait until it's time to spawn the next power-up
            while (Time.time < nextSpawnTime)
            {
                yield return null;
            }

            // Randomly select a monster
            GameObject monster = GetActiveMonster();

            if (monster != null)
            {
                // Get the position of the selected monster
                Vector3 spawnPosition = monster.transform.position;

                // Spawn a random power-up at the position of the selected monster
                GameObject powerUpPrefab = powerUps[Random.Range(0, powerUps.Length)];
                GameObject powerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);

                // Start the coroutine to destroy the power-up and show warning blink
                StartCoroutine(DestroyPowerUp(powerUp));
            }
            else
            {
                Debug.LogWarning("No active monster found to carry the power-up.");
            }

            // Schedule the next power-up spawn
            nextSpawnTime = Time.time + spawnInterval;

            // Update the spawn interval based on the elapsed time
            SetSpawnInterval();
        }
    }

    IEnumerator DestroyPowerUp(GameObject powerUp)
    {
        // Wait for warning time before starting the warning blink
        yield return new WaitForSeconds(powerUpDuration - warningTime);

        // Start warning blink
        StartCoroutine(WarningBlink(powerUp));

        // Wait for the remaining time before destroying the power-up
        yield return new WaitForSeconds(warningTime);

        // Destroy the power-up object
        Destroy(powerUp);
    }

    IEnumerator WarningBlink(GameObject powerUp)
    {
        SpriteRenderer spriteRenderer = powerUp.GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        while (true)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    GameObject GetActiveMonster()
    {
        // Check if any monsters are active
        GameObject[] activeMonsters = GameObject.FindGameObjectsWithTag("Monsters");
        if (activeMonsters.Length > 0)
        {
            // Return a randomly selected active monster
            return activeMonsters[Random.Range(0, activeMonsters.Length)];
        }
        return null;
    }
}
