using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float arcHeight = 1f; // Adjust this value to control the height of the arc

    private Transform target;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float distance;
    private float startTime;

    public void SetTarget(Transform target)
    {
        this.target = target;
        startPoint = transform.position;
        endPoint = target.position;
        distance = Vector3.Distance(startPoint, endPoint);
        startTime = Time.time;
    }

    void Update()
    {
        if (target != null)
        {
            float journeyLength = distance / speed;
            float fracJourney = (Time.time - startTime) / journeyLength;
            Vector3 arcPosition = Vector3.Lerp(startPoint, endPoint, fracJourney);
            arcPosition.y += Mathf.Sin(fracJourney * Mathf.PI) * arcHeight;

            if (fracJourney >= 1)
            {
                HitTarget();
                return;
            }

            transform.position = arcPosition;
        }
        else
        {
            // If target is null, destroy the bullet
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        // Check if the target is a monster
        MonstersScript monster = target.GetComponent<MonstersScript>();
        if (monster != null)
        {
            // Damage the monster
            monster.TakeDamage(damage);
        }
        else
        {
            // Check if the target is a SantelmoMonster
            SantelmoMonster santelmo = target.GetComponent<SantelmoMonster>();
            if (santelmo != null)
            {
                // Damage the SantelmoMonster
                santelmo.TakeDamage(damage);
            }
        }

        // Destroy the bullet
        Destroy(gameObject);
    }
}
