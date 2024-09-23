using UnityEngine;

public class MeteorBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float arcHeight = 1f; // Adjust this value to control the height of the arc

    private Transform target;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float distance;
    private float startTime;

    void Start()
    {
        // Determine the initial orientation of the bullet
        if (target != null)
        {
            FlipBulletIfNeeded();
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        startPoint = transform.position;
        endPoint = target.position;
        distance = Vector3.Distance(startPoint, endPoint);
        startTime = Time.time;

        // Flip the bullet's orientation when setting the target
        FlipBulletIfNeeded();
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

            // Flip the bullet's orientation based on its movement direction
            FlipBulletIfNeeded();
        }
        else
        {
            // If target is null, destroy the bullet
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        // Check if the target is a tower
        TowerScript tower = target.GetComponent<TowerScript>();
        if (tower != null)
        {
            // Damage the tower
            tower.TakeDamage(damage);
        }
        JuanD juanDScript = target.GetComponent<JuanD>();
        if (juanDScript != null)
        {
            // Damage the tower
            juanDScript.TakeDamage(damage);
        }

        // Destroy the bullet
        Destroy(gameObject);
    }

    void FlipBulletIfNeeded()
    {
        if (target != null)
        {
            // Flip the bullet horizontally if moving towards the left
            Vector3 direction = (endPoint - startPoint).normalized;
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-0.3838113f, 0.3838113f, 0.3838113f);
            }
            else
            {
                transform.localScale = new Vector3(0.3838113f, 0.3838113f, 0.3838113f);
            }
        }
    }
}
