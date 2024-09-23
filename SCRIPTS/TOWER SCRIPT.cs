using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerScript : MonoBehaviour
{
    public float attackRange = 5f;
    public float attackRate = 2f;
    public int damage = 10; // Declare the damage variable
    public int maxHealth = 100; // Maximum health of the tower
    public int currentHealth; // Current health of the tower

    public GameObject bulletPrefab;
    public Color damageColor = Color.red; // Color to indicate damage
    public float damageDuration = 0.1f; // Duration of the damage effect in seconds

    public bool isAttacking = false; // Flag to determine if the tower is currently attacking

    private float nextAttackTime = 0f;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Animator animator; // Reference to the Animator component

    private bool isFlipping = false; // Flag to track if the tower is currently flipping
    private Coroutine flipCoroutine; // Coroutine reference for flipping animation

    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform damageTextSpawnPoint; // Spawn point for damage text

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to maximum health
        // Get the SpriteRenderer component attached to the tower
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get the Animator component attached to the tower
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            AttackEnemy();
            nextAttackTime = Time.time + 1f / attackRate;
        }

        // Set the isAttacking parameter in the Animator
        animator.SetBool("Attack", isAttacking);
    }

    void AttackEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monsters");

        bool enemyInRange = false; // Flag to check if there's an enemy in range

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance <= attackRange)
            {
                // Instantiate a bullet and set its properties
                GameObject bulletObject = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                BulletScript bullet = bulletObject.GetComponent<BulletScript>();
                bullet.damage = damage;

                // Set the target of the bullet to the enemy
                bullet.SetTarget(enemy.transform);

                // Set isAttacking to true
                isAttacking = true;

                // Flip the tower if the enemy is to the right
                FlipIfNeeded(enemy.transform.position);

                enemyInRange = true; // Set the flag to true since an enemy is in range
                break; // Exit the loop after attacking one enemy
            }
        }

        // If no enemies are in range, set isAttacking to false
        if (!enemyInRange)
        {
            isAttacking = false;
        }
    }


    void FlipIfNeeded(Vector3 targetPosition)
    {
        // Flip the tower sprite if the target is to the right
        if (targetPosition.x > transform.position.x)
        {
            if (!spriteRenderer.flipX && !isFlipping) // Check if the tower is not already flipped and not currently flipping
            {
                flipCoroutine = StartCoroutine(FlipAnimation(true));
            }
        }
        else
        {
            if (spriteRenderer.flipX && !isFlipping) // Check if the tower is flipped and not currently flipping
            {
                flipCoroutine = StartCoroutine(FlipAnimation(false));
            }
        }
    }

    IEnumerator FlipAnimation(bool flipToRight)
    {
        isFlipping = true;

        float flipDuration = 0f; // Duration of the flip animation
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;
        Vector3 flippedScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

        while (elapsedTime < flipDuration)
        {
            Vector3 targetScale = flipToRight ? flippedScale : originalScale;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = flipToRight ? flippedScale : originalScale;

        // Add a delay before returning to the original orientation
        float delayDuration = 30f; // Duration of the delay before returning to original orientation
        yield return new WaitForSeconds(delayDuration);

        elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            Vector3 targetScale = flipToRight ? originalScale : flippedScale;
            transform.localScale = Vector3.Lerp(flipToRight ? flippedScale : originalScale, targetScale, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = flipToRight ? originalScale : flippedScale;

        isFlipping = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        SpawnDamageText(damage);
        //Debug.Log("Tower took damage. Current health: " + currentHealth);

        // Apply damage effect
        ApplyDamageEffect();

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // If health drops to zero or below, destroy the tower
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
        // Change the color of the tower temporarily to indicate damage
        spriteRenderer.color = damageColor;
        // Reset the color after the specified duration
        Invoke("ResetColor", damageDuration);
    }

    void ResetColor()
    {
        // Reset the color back to normal
        spriteRenderer.color = Color.white;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
