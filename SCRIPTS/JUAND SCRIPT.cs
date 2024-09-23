using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class JuanD : MonoBehaviour
{
    public float speed = 5f; // Movement speed of JuanD
    public int maxHealth = 100; // Maximum health of JuanD
    public int currentHealth; // Current health of JuanD
    public float attackRange = 1.5f; // Range within which JuanD can attack monsters
    public int damage = 20; // Damage dealt by JuanD's attack

    public Color damageColor = Color.red; // Color to indicate damage
    public float damageDuration = 0.1f; // Duration of the damage effect in seconds

    public Joystick joystick; // Reference to the joystick for movement
    public Button attackButton; // Reference to the attack button

    private bool isAttacking = false; // Flag to track if an attack is in progress

    private Animator animator; // Reference to the Animator component

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public RectTransform mapCanvas; // Reference to the map canvas
    public float scrollSpeed = 1f; // Speed of horizontal camera scrolling

    public Camera mainCamera; // Reference to the main camera

    public Slider healthBarSlider; // Reference to the health bar slider

    public GameObject gameOverScreen; // Reference to the game over screen prefab
    private bool isGameOver = false; // Flag to track if game over screen is already shown

    public Text endTimeText; // Reference to the UI Text element to display the end time


    private float originalSpeed;
    private int originalDamage;

    private Color originalColor; // Store the original color of JuanD

    private bool isPowerUpActive = false;

    private Vector3 lastMovementDirection = Vector3.right; // Store the last movement direction

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip[] soundEffects; // Array of sound effects to play
    public float soundInterval = 20f; // Interval between playing sound effects

    public AudioSource attackAudioSource; // Reference to the AudioSource for attack sound effects
    public AudioClip attackSoundEffect; // Attack sound effect

    public AudioSource hitAudioSource; // Reference to the AudioSource for hit sound effects
    public AudioClip hitSoundEffect; // Hit sound effect

    public AudioSource dieAudioSource; // Reference to the AudioSource for death sound effects
    public AudioClip dieSoundEffect; // Death sound effect


    public Button ultimateButton; // Reference to the ultimate ability button
    public GameObject ultimateBeamPrefab; // Reference to the ultimate beam prefab
    public Transform firePoint; // Reference to the fire point for the ultimate beam
    public float beamDuration = 1.5f; // Duration for which the beam remains active
    public float beamDelay = 50f; // Delay before the beam becomes active

    private bool isUltimateActive = false; // Flag to track if the ultimate ability is active
    private GameObject ultimateBeam; // Reference to the instantiated ultimate beam

    public GameObject bloodEffectPrefab1; // Reference to blood effect prefab 1
    public GameObject bloodEffectPrefab2; // Reference to blood effect prefab 2
    public float bloodEffectDuration = 1f; // Duration of blood effect animation
    public GameObject damageTextPrefab; // Reference to the damage text prefab
    public Transform damageTextSpawnPoint; // Spawn point for damage text

    private bool isImmune = false; // Flag to track if JuanD is immune to damage
    private float attackAnimationDuration = 1f; // Duration of the attack animation in seconds


    private void Start()
    {
        // Find and assign the health bar slider in the scene
        healthBarSlider = GameObject.FindObjectOfType<Slider>();
        if (healthBarSlider == null)
        {
            Debug.LogError("Health bar slider not found in the scene.");
        }

        // Set the maximum value of the health bar slider to the maximum health of JuanD
        healthBarSlider.maxValue = maxHealth;
        // Set the current value of the health bar slider to the initial health of JuanD
        healthBarSlider.value = currentHealth;
        currentHealth = maxHealth; // Initialize current health to maximum health

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the main camera
        mainCamera = Camera.main;

        // Add a listener to the attack button
        attackButton.onClick.AddListener(Attack);

        originalSpeed = speed;
        originalDamage = damage;

        // Subscribe to Santelmo's death event
        SantelmoMonster.OnMonsterDeath += ActivatePowerUp;

        originalColor = spriteRenderer.color;

        InvokeRepeating("PlayRandomSound", soundInterval, soundInterval);

        // Add a listener to the ultimate button
        ultimateButton.onClick.AddListener(ActivateUltimate);

        // Instantiate the ultimate beam and deactivate it
        ultimateBeam = Instantiate(ultimateBeamPrefab, transform.position, Quaternion.identity);
        ultimateBeam.SetActive(false);

        // Set the duration of the attack animation
        attackAnimationDuration = 1f; // Adjust this value according to the duration of your attack animation
    }


    private void PlayRandomSound()
    {
        // Check if there are any sound effects assigned
        if (soundEffects.Length > 0)
        {
            // Choose a random sound effect from the array
            AudioClip randomClip = soundEffects[UnityEngine.Random.Range(0, soundEffects.Length)];

            // Play the chosen sound effect
            audioSource.PlayOneShot(randomClip);
        }
    }
    private void Update()
    {
        // Move JuanD using joystick input
        Move();

        // Update camera position to follow JuanD horizontally
        UpdateCameraPosition();
        healthBarSlider.value = currentHealth;
    }

    private void Move()
    {
        // Check if an attack is not already in progress
        if (!isAttacking)
        {
            // Get horizontal and vertical input from joystick
            float horizontalInput = joystick.Horizontal;
            float verticalInput = joystick.Vertical;

            // Calculate movement direction based on input
            Vector3 movementDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

            // Remember the last movement direction if it's not zero
            if (movementDirection.magnitude > 0)
            {
                lastMovementDirection = movementDirection;
            }

            // Calculate the desired position after movement
            Vector3 desiredPosition = transform.position + movementDirection * speed * Time.deltaTime;

            // Get the width and height of the map canvas
            float mapCanvasWidth = mapCanvas.rect.width * mapCanvas.lossyScale.x;
            float mapCanvasHeight = mapCanvas.rect.height * mapCanvas.lossyScale.y;

            // Calculate the minimum and maximum x and y positions for the character within the map canvas
            float minX = mapCanvas.position.x - mapCanvasWidth / 2f + transform.localScale.x / 2f;
            float maxX = mapCanvas.position.x + mapCanvasWidth / 2f - transform.localScale.x / 2f;
            float minY = mapCanvas.position.y - mapCanvasHeight / 4f + transform.localScale.y / 2f;
            float maxY = mapCanvas.position.y + mapCanvasHeight / 4f - transform.localScale.y / 2f;

            // Apply the clamped position
            float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

            // Set the scale of JuanD based on power-up status
            Vector3 scale = isPowerUpActive ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.38f, 0.38f, 0.38f);
            transform.localScale = scale;

            // Set the position of JuanD
            transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);

            // Set animator parameters for movement
            if (movementDirection.magnitude > 0) // Check if there is any input from the joystick
            {
                // Set animator parameter for walking direction
                if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput)) // Character is moving vertically
                {
                    if (verticalInput > 0) // Moving upwards
                    {
                        animator.SetBool("Walk", false); // Stop horizontal walking animation
                        animator.SetBool("IsMovingUp", true); // Set the animator parameter for walking upwards
                        animator.SetBool("IsMovingDown", false); // Ensure the downwards movement parameter is false
                    }
                    else // Moving downwards
                    {
                        animator.SetBool("Walk", false); // Stop horizontal walking animation
                        animator.SetBool("IsMovingUp", false); // Ensure the upwards movement parameter is false
                        animator.SetBool("IsMovingDown", true); // Set the animator parameter for walking downwards
                    }
                }
                else // Character is moving horizontally
                {
                    animator.SetBool("Walk", true); // Start horizontal walking animation
                    animator.SetBool("IsMovingUp", false); // Ensure the upwards movement parameter is false
                    animator.SetBool("IsMovingDown", false); // Ensure the downwards movement parameter is false
                }

                // Flip character horizontally if moving towards the left side
                if (movementDirection.x < 0)
                {
                    // Flip the character horizontally
                    transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                }
                else if (movementDirection.x > 0)
                {
                    // Face right, set the scale back to normal
                    transform.localScale = scale;
                }
            }
            else
            {
                // If there's no input, stop all movement animations
                animator.SetBool("Walk", false);
                animator.SetBool("IsMovingUp", false);
                animator.SetBool("IsMovingDown", false);
            }

            // Set the last movement direction for horizontal movement
            if (lastMovementDirection.x < 0)
            {
                // Flip the character horizontally
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (lastMovementDirection.x > 0)
            {
                // Face right, set the scale back to normal
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }














    public void Attack()
    {
        // Check if an attack is not already in progress
        if (!isAttacking)
        {
            // Play the attack sound effect if assigned
            if (attackAudioSource != null && attackSoundEffect != null)
            {
                attackAudioSource.PlayOneShot(attackSoundEffect);
            }

            // Set the attacking flag to true to prevent subsequent attacks until this one is finished
            isAttacking = true;

            // Trigger attack animation
            animator.SetTrigger("Attack");

            // Delay the actual attack by a short duration to synchronize with the animation
            StartCoroutine(DelayedAttack());
        }
    }

    private IEnumerator DelayedAttack()
    {
        // Wait for the attack animation to finish
        yield return new WaitForSeconds(attackAnimationDuration);

        // Deal damage after the animation is finished
        DealDamage();
    }

    private bool IsPlayerMoving()
    {
        return joystick.Horizontal != 0 || joystick.Vertical != 0;
    }


    // Method to be called when the attack animation is finished
    private void DealDamage()
    {
        // Find all enemy monsters within attack range
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monsters");
        foreach (GameObject monster in monsters)
        {
            float distance = Vector2.Distance(transform.position, monster.transform.position);
            if (distance <= attackRange)
            {
                // Deal damage to the monster
                if (monster.TryGetComponent(out MonstersScript monsterScript))
                {
                    monsterScript.TakeDamage(damage);
                }
                else if (monster.TryGetComponent(out SantelmoMonster santelmoMonsterScript))
                {
                    santelmoMonsterScript.TakeDamage(damage);
                }
            }
        }
        
        // Transition back to idle state
        animator.SetTrigger("Idle");

        // Reset the attacking flag to allow for the next attack
        isAttacking = false;
    }

  




    public void TakeDamage(int damage)
    {
        // Check if JuanD is immune to damage
        if (!isImmune)
        {
            // Reduce JuanD's health
            currentHealth -= damage;

            // Play hit sound effect if assigned
            if (hitAudioSource != null && hitSoundEffect != null)
            {
                hitAudioSource.PlayOneShot(hitSoundEffect);
            }

            // Instantiate a random blood effect prefab at the position of JuanD
            GameObject bloodEffectPrefab = (UnityEngine.Random.Range(0, 2) == 0) ? bloodEffectPrefab1 : bloodEffectPrefab2;
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

            // Destroy the blood effect after the specified duration
            Destroy(bloodEffect, bloodEffectDuration);

            // Spawn damage text
            SpawnDamageText(damage);

            // Update the value of the health bar slider
            healthBarSlider.value = currentHealth;

            // Check if JuanD's health is depleted
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // Apply damage effect
                ApplyDamageEffect();
            }
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





    private void Die()
    {
        // Play death sound effect if assigned
        if (dieAudioSource != null && dieSoundEffect != null)
        {
            dieAudioSource.PlayOneShot(dieSoundEffect);
        }

        // Check if game over screen is already shown to avoid duplication
        if (!isGameOver)
        {
            // Set the game over flag to true
            isGameOver = true;

            // Activate the game over screen GameObject
            gameOverScreen.SetActive(true);

            // Get a reference to the TimeWatch component in the scene
            TimeWatch timeWatch = FindObjectOfType<TimeWatch>();

            // Stop the timer if it's running
            if (timeWatch != null && timeWatch.IsTimerStarted())
            {
                timeWatch.enabled = false; // Disable the TimeWatch script to stop the timer

                // Get the elapsed time as score
                int score = timeWatch.GetElapsedTimeAsScore();

                // Set the end time on the game over screen
                if (endTimeText != null)
                {
                    endTimeText.text = "Score: " + score.ToString();
                }


            }

            // Destroy JuanD object
            Destroy(gameObject);

            // Optional: You may want to disable other game components here to prevent further interactions.
        }
    }



    private void ApplyDamageEffect()
    {
        // Change the color of JuanD temporarily to indicate damage
        spriteRenderer.color = damageColor;
        // Reset the color after the specified duration
        Invoke("ResetColor", damageDuration);
    }

    private void ResetColor()
    {
        // Reset the color back to normal
        spriteRenderer.color = Color.white;
    }

    private void UpdateCameraPosition()
    {
        if (mainCamera != null && mapCanvas != null)
        {
            // Get the width of the map canvas
            float mapCanvasWidth = mapCanvas.rect.width * mapCanvas.lossyScale.x;

            // Calculate the minimum and maximum x positions for the camera
            float minX = mapCanvas.position.x - mapCanvasWidth / 2f + mainCamera.orthographicSize * mainCamera.aspect;
            float maxX = mapCanvas.position.x + mapCanvasWidth / 2f - mainCamera.orthographicSize * mainCamera.aspect;

            // Get the position of JuanD
            Vector3 juandPosition = transform.position;

            // Clamp the camera's x position within the bounds of the map canvas
            float clampedX = Mathf.Clamp(juandPosition.x, minX, maxX);

            // Update the camera position to follow JuanD horizontally
            mainCamera.transform.position = new Vector3(clampedX, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
        else
        {
            Debug.LogWarning("Main camera or map canvas not found!");
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    private void OnDestroy()
    {
        // Unsubscribe from Santelmo's death event to avoid memory leaks
        SantelmoMonster.OnMonsterDeath -= ActivatePowerUp;
    }

    private void ActivatePowerUp()
    {
        isPowerUpActive = true;

        // Store the original color of JuanD
        originalColor = spriteRenderer.color;

        // Start the blinking effect
        StartCoroutine(BlinkEffect());

        // Increase health by 100
        currentHealth += 100;

        // Increase speed to 6
        speed = 6f;

        // Increase damage to 100
        damage = 100;

        // Increase size of JuanD
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Set size to 1.1 times the original size


        // Reset power-up after 10 seconds
        Invoke("ResetPowerUp", 10f);
    }

    private void ResetPowerUp()
    {
        // Reset health, speed, damage, and size to their original values
        currentHealth = maxHealth;
        speed = originalSpeed;
        damage = originalDamage;
        transform.localScale /= 0.38f;

        // Reset power-up flag
        isPowerUpActive = false;
    }

    private IEnumerator BlinkEffect()
    {
        // Define the blinking duration and interval
        float blinkDuration = 10f; // Duration of the entire blink effect
        float blinkInterval = 0.1f; // Interval between color changes

        // Define the colors for blinking
        Color yellowColor = Color.blue;

        // Loop through the blink duration
        float timer = 0f;
        while (timer < blinkDuration)
        {
            // Toggle between original color and yellow color
            spriteRenderer.color = (spriteRenderer.color == originalColor) ? yellowColor : originalColor;

            // Wait for the blink interval
            yield return new WaitForSeconds(blinkInterval);

            // Increment timer
            timer += blinkInterval;
        }

        // Reset the color to original after blinking
        spriteRenderer.color = originalColor;
    }

    private void ActivateUltimate()
    {
        if (!isUltimateActive)
        {
            // Play the casting beam animation
            animator.SetTrigger("CastBeam");

            // Delay before the beam becomes active
            StartCoroutine(ActivateBeamWithDelay());
        }
    }

    IEnumerator ActivateBeamWithDelay()
    {
        yield return new WaitForSeconds(beamDelay);

        // Activate ultimate ability
        isUltimateActive = true;
        ultimateBeam.SetActive(true);

        // Set the position of the ultimate beam to the fire point
        ultimateBeam.transform.position = firePoint.position;

        // Find all enemy monsters within ultimate range and deal damage to them
        GameObject[] monsters = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject monster in monsters)
        {
            // Check the position of each monster against the position of the ultimate beam
            if (Vector2.Distance(ultimateBeam.transform.position, monster.transform.position) < 2f)
            {
                // Deal damage to the monster
                if (monster.TryGetComponent(out MonstersScript monsterScript))
                {
                    monsterScript.TakeDamage(damage);
                }
                else if (monster.TryGetComponent(out SantelmoMonster santelmoMonsterScript))
                {
                    santelmoMonsterScript.TakeDamage(damage);
                }
            }
        }

        // Destroy the beam after the specified duration
        Invoke("DeactivateUltimate", beamDuration);
    }

    private void DeactivateUltimate()
    {
        // Deactivate ultimate ability
        isUltimateActive = false;
        ultimateBeam.SetActive(false);
        ReturnToIdleState();
    }

    // Method to return the animation to the idle state
    public void ReturnToIdleState()
    {
        animator.SetTrigger("Idle");
    }

    public void IncreaseHealth(int bonus)
    {
        currentHealth += bonus;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed maxHealth
        healthBarSlider.value = currentHealth;
    }

    public void ActivateImmunity(float duration)
    {
        StartCoroutine(Immunity(duration));
    }

    private IEnumerator Immunity(float duration)
    {
        isImmune = true;
        yield return new WaitForSeconds(duration);
        isImmune = false;
    }
}