using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SantelmoMonster : MonoBehaviour
{
    public float speed = 3f;
    public int health = 100;
    public int damage = 20; // Increased damage for Santelmo
    public float attackRange = 5f; // Increased attack range for Santelmo
    public float attackRate = 3f; // Decreased attack rate for Santelmo

    public int moneyReward = 70; // Increased money reward for Santelmo
    public int expReward = 10;
    public Color damageColor = Color.red;
    public float damageDuration = 0.1f;

    public GameObject meteorBulletPrefab; // Prefab of the meteor bullet to be instantiated
    public Transform firePoint; // Point from where the meteor bullet will be fired

    private float nextAttackTime = 0f;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private MoneyManager moneyManager;
    private Animator animator;
    private GameObject[] towers;
    private GameObject juanD;
    private Transform target;

    private bool isAttacking = false; // Track if the monster is currently attacking

    public delegate void MonsterDeathAction();
    public static event MonsterDeathAction OnMonsterDeath;
    public bool triggerMonsterDeathEvent = true;


    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform damageTextSpawnPoint; // Spawn point for damage text
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager script not found in the scene.");
        }
        animator = GetComponent<Animator>();

        towers = GameObject.FindGameObjectsWithTag("Towers");
        juanD = GameObject.FindGameObjectWithTag("JuanD");

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
        EndAttackAnimation();
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
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Check if Santelmo is too close to the target
            if (distanceToTarget <= attackRange)
            {
                Vector3 direction = (transform.position - target.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                // Move towards the target
                Vector3 targetPosition = target.position;
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                if (direction.x < 0 && facingRight)
                {
                    Flip();
                }
                else if (direction.x > 0 && !facingRight)
                {
                    Flip();
                }
            }

            // Check if Santelmo is in attack range and can attack
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                isAttacking = true;
                Attack(target);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }


    void Attack(Transform target)
    {
        // Santelmo will spawn a meteor bullet and launch it towards the target
        GameObject bulletObject = Instantiate(meteorBulletPrefab, firePoint.position, Quaternion.identity);
        MeteorBullet meteorBullet = bulletObject.GetComponent<MeteorBullet>();
        if (meteorBullet != null)
        {
            meteorBullet.SetTarget(target); // Set the target for the meteor bullet
        }
    }

    void EndAttackAnimation()
    {
        isAttacking = false; // Set attacking back to false when attack animation ends
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
        spriteRenderer.color = damageColor;
        Invoke("ResetColor", damageDuration);
    }

    void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }

    void DieMonster()
    {
        // Add experience points to the ExperienceManager
        var experienceManager = FindObjectOfType<ExperienceManager>();
        if (experienceManager != null)
        {
            experienceManager.currentExp += expReward;
            if (experienceManager.expSlider != null)
            {
                experienceManager.expSlider.value = experienceManager.currentExp;
            }
        }
        else
        {
            Debug.LogError("ExperienceManager reference is null.");
        }

        // Add money reward to the MoneyManager
        var moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager != null)
        {
            moneyManager.AddMoney(moneyReward);
        }
        else
        {
            Debug.LogError("MoneyManager reference is null.");
        }

        // Trigger the OnMonsterDeath event
        if (triggerMonsterDeathEvent)
        {
            OnMonsterDeath?.Invoke();
        }

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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
