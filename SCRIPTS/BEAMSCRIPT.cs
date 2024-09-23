using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateBeam : MonoBehaviour
{
    public int ultimateDamage = 50; // Damage dealt by the ultimate beam
    public Animator animator; // Reference to the animator component

    //private bool hasPlayed = false; // Flag to track if the animation has played

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider belongs to a monster
        if (collision.CompareTag("Monsters"))
        {
            // Deal damage to the monster
            if (collision.TryGetComponent(out MonstersScript monsterScript))
            {
                monsterScript.TakeDamage(ultimateDamage);
            }
            else if (collision.TryGetComponent(out SantelmoMonster santelmoMonsterScript))
            {
                santelmoMonsterScript.TakeDamage(ultimateDamage);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Play the beam animation
        animator = GetComponent<Animator>(); // Get the animator component
    }

    // Method to destroy the beam after the animation has played
    public void DestroyBeam()
    {
        Destroy(gameObject); // Destroy the beam game object
    }
}
