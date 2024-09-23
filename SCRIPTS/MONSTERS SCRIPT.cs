using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonstersScript : MonoBehaviour
{
    public delegate void MonsterDeathAction(int expReward);
    public static event MonsterDeathAction OnMonsterDeath;

    public float speed = 3f;
    public int health = 100;
    public int damage = 10;
    public float attackRange = 3f;
    public float attackRate = 2f;
    public float attackSpace = 1.0f; // Define attack space

    public int moneyReward = 50; // Money rewarded to the player when the monster dies
    public int expReward = 10; // Experience points rewarded to the player when the monster dies

    public Color damageColor = Color.red; // Color to indicate damage
    public float damageDuration = 0.1f; // Duration of the damage effect in seconds

    private float nextAttackTime = 0f;
    private bool facingRight = true; // Track the direction the monster is facing
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private MoneyManager moneyManager; // Reference to the MoneyManager script
    private Animator animator; // Reference to the Animator component
    private GameObject[] towers; // Array to store tower game objects
    private GameObject juanD; // Reference to JuanD game object
    private Transform target; // Current target to attack

    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform damageTextSpawnPoint; // Spawn point for damage text

    public GameObject bloodEffectPrefab1; // Reference to blood effect prefab 1
    public GameObject bloodEffectPrefab2; // Reference to blood effect prefab 2
    public float bloodEffectDuration = 1f; // Duration of blood effect animation

    void Start()
    {
        // Get the SpriteRenderer component attached to the monster
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Find and assign the MoneyManager script in the scene
        moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager script not found in the scene.");
        }
        // Get the Animator component attached to the monster
        animator = GetComponent<Animator>();

        // Find all towers in the scene
        towers = GameObject.FindGameObjectsWithTag("Towers");
        // Find JuanD in the scene
        juanD = GameObject.FindGameObjectWithTag("JuanD");

        // Set initial target to the nearest tower
        target = GetNearestTower();


    }

    void Update()
    {
        // Check if the current target is null (no towers left) and JuanD exists
        if (target == null && juanD != null)
        {
            // Set target to JuanD
            target = juanD.transform;
        }

        // Move towards the target and attack if in range
        MoveAndAttack();

        // Set the isAttacking parameter in the Animator to trigger attack animation
        animator.SetBool("Attack", Time.time < nextAttackTime);
    }
    Transform GetNearestTower()
    {
        Transform nearestTower = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject tower in towers)
        {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTower = tower.transform;
            }
        }

        return nearestTower;
    }
    void MoveAndAttack()
    {
        if (target != null)
        {
            // Check if the target is within attack range
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                // If the monster is not already within attack space, move away from the target
                if (distance > attackSpace)
                {
                    Vector3 direction = (transform.position - target.position).normalized;
                    transform.position += direction * attackSpace;
                }
                // Attack after moving
                Attack(target);
                nextAttackTime = Time.time + 1f / attackRate;
            }
            else
            {
                // Move towards the target
                Vector3 targetPosition = target.position;
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;



                // Flip the monster sprite if moving left
                if (direction.x < 0 && facingRight)
                {
                    Flip();
                }
                // Flip the monster sprite if moving right
                else if (direction.x > 0 && !facingRight)
                {
                    Flip();
                }
            }
        }
    }



    void Attack(Transform target)
    {
        // Deal damage to the target
        if (target.CompareTag("Towers"))
        {
            // Attack towers
            TowerScript towerScript = target.GetComponent<TowerScript>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(damage);
            }
        }
        else if (target.CompareTag("JuanD"))
        {
            // Attack JuanD
            JuanD juanDScript = target.GetComponent<JuanD>();
            if (juanDScript != null)
            {
                juanDScript.TakeDamage(damage);
            }
        }

        // Move the monster away from the target before attacking
        Vector3 direction = (transform.position - target.position).normalized;
        transform.position += direction * attackSpace;

        // Trigger the OnMonsterDeath event
        if (OnMonsterDeath != null)
        {
            OnMonsterDeath(expReward);
        }
    }




    public void TakeDamage(int damage)
    {
        health -= damage;

        SpawnDamageText(damage);

        // If the monster is still alive, apply the damage effect
        if (health > 0)
        {
            ApplyDamageEffect();
        }
        else
        {
            // Instantiate a random blood effect prefab at the position of JuanD
            GameObject bloodEffectPrefab = (UnityEngine.Random.Range(0, 2) == 0) ? bloodEffectPrefab1 : bloodEffectPrefab2;
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

            // Destroy the blood effect after the specified duration
            Destroy(bloodEffect, bloodEffectDuration);
            DieMonster();
        }
    }

    private void SpawnDamageText(int damage)
    {
        // Instantiate damage text prefab
        GameObject damageTextObject = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity);
        damageTextObject.transform.SetParent(damageTextSpawnPoint, false);
        Text damageText = damageTextObject.GetComponent<Text>();
        damageText.text = damage.ToString();

        // Set text color
        damageText.color = Color.red;

        // Set the scale based on the direction the monster is facing
        Vector3 scale = damageTextObject.transform.localScale;
        scale.x = transform.localScale.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        damageTextObject.transform.localScale = scale;

        // Start the floating and fading coroutine
        StartCoroutine(FloatingDamageText(damageTextObject));
    }



    IEnumerator FloatingDamageText(GameObject damageTextObject)
    {
        Text damageText = damageTextObject.GetComponent<Text>();
        float duration = 1f; // Duration of floating and fading
        float startTime = Time.time;

        // Floating speed and direction
        float speed = 1f;
        float floatHeight = 1f;
        Vector3 startPosition = damageTextObject.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * floatHeight;

        // Fade speed
        float fadeSpeed = 1f;

        while (Time.time < startTime + duration)
        {
            // Move the damage text upward
            damageTextObject.transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / duration);

            // Fade out the damage text
            float alpha = 1 - (Time.time - startTime) / duration;
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, alpha);

            yield return null;
        }

        // Move the damage text downward while fading out
        while (damageText.color.a > 0)
        {
            damageTextObject.transform.position -= Vector3.up * speed * Time.deltaTime;

            float alpha = damageText.color.a - fadeSpeed * Time.deltaTime;
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, alpha);

            yield return null;
        }

        // Destroy the damage text object after fading out
        Destroy(damageTextObject);
    }


    void ApplyDamageEffect()
    {
        // Change the color of the monster temporarily to indicate damage
        spriteRenderer.color = damageColor;
        // Reset the color after the specified duration
        Invoke("ResetColor", damageDuration);
    }

    void ResetColor()
    {
        // Reset the color back to normal
        spriteRenderer.color = Color.white;
    }

    void DieMonster()
    {
        // Add exp to the Experience Manager
        FindObjectOfType<ExperienceManager>().GainExp(expReward);

        // Add money to the Money Manager
        if (moneyManager != null)
        {
            moneyManager.AddMoney(moneyReward); // Add money to the player's current money
        }
        else
        {
            Debug.LogError("MoneyManager reference is null.");
        }

        // Destroy the monster object
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void Flip()
    {
        facingRight = !facingRight;

        // Get the local scale of the monster
        Vector3 scale = transform.localScale;
        // Flip the x component of the scale
        scale.x *= -1;
        // Update the local scale of the monster
        transform.localScale = scale;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided object is a tower or JuanD
        if (collision.gameObject.CompareTag("Towers") || collision.gameObject.CompareTag("JuanD"))
        {
            // Prevent the monster from moving further
            transform.position = Vector3.MoveTowards(transform.position, collision.transform.position, -speed * Time.deltaTime);
        }
    }
}
