using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // Reference to the monster prefab
    public Transform[] spawnPoints; // Array of spawn points (positions from where monsters will be spawned)

    public float initialSpawnInterval = 2f; // Initial interval between monster spawns
    private float spawnInterval; // Current interval between monster spawns

    private float spawnTimer = 0f; // Timer to track when to spawn the next monster
    private TimeWatch timeWatch; // Reference to the TimeWatch script

    private float intervalCheckTime = 0f; // Timer to track when to check if the spawn interval needs to decrease
    private float intervalCheckInterval = 120f; // Interval to check if the spawn interval needs to decrease (2 minutes)

    private bool spawnIntervalDecreased = false; // Flag to track if the spawn interval has been decreased

    public Canvas spawnCanvas; // Reference to the canvas where monsters will spawn

    void Start()
    {
        // Find the TimeWatch script in the scene
        timeWatch = FindObjectOfType<TimeWatch>();

        if (timeWatch == null)
        {
            Debug.LogError("TimeWatch script not found in the scene!");
        }

        // Set the initial spawn interval
        spawnInterval = initialSpawnInterval;
    }

    void Update()
    {
        // Check if the timer has started in the TimeWatch script
        if (timeWatch != null && timeWatch.IsTimerStarted())
        {
            // Increment the spawn timer
            spawnTimer += Time.deltaTime;

            // Increment the interval check timer
            intervalCheckTime += Time.deltaTime;

            // Check if it's time to spawn a monster
            if (spawnTimer >= spawnInterval)
            {
                // Spawn a monster
                SpawnMonster();

                // Reset the spawn timer
                spawnTimer = 0f;
            }

            // Check if it's time to reduce the spawn interval
            if (intervalCheckTime >= intervalCheckInterval && !spawnIntervalDecreased)
            {
                // Reduce spawn interval by 50%
                spawnInterval *= 0.5f;

                // Set flag to indicate spawn interval has been decreased
                spawnIntervalDecreased = true;
            }
        }
    }

    void SpawnMonster()
    {
        // Check if the monster prefab and spawn points are set
        if (monsterPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Monster prefab or spawn points not set!");
            return;
        }

        // Randomly select a spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate a monster at the spawn point
        GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);

        // Set the parent of the monster to the specified canvas
        monster.transform.SetParent(spawnCanvas.transform);
    }
}
