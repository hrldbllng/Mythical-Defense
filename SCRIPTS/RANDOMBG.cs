using UnityEngine;

public class RandomMapBackground : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundGroup
    {
        public GameObject background; // Map background GameObject
        public GameObject[] associatedObjects; // Additional GameObjects associated with this background
    }

    public BackgroundGroup[] backgroundGroups; // Array of BackgroundGroup objects

    private void Start()
    {
        // Check if there are any background groups available
        if (backgroundGroups != null && backgroundGroups.Length > 0)
        {
            // Choose a random index within the range of available background groups
            int randomIndex = Random.Range(0, backgroundGroups.Length);

            // Deactivate all background groups and their associated objects
            foreach (var bg in backgroundGroups)
            {
                bg.background.SetActive(false);
                foreach (var obj in bg.associatedObjects)
                {
                    obj.SetActive(false);
                }
            }

            // Activate the randomly chosen map background and associated objects
            backgroundGroups[randomIndex].background.SetActive(true);
            foreach (var obj in backgroundGroups[randomIndex].associatedObjects)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("No background groups assigned to RandomMapBackground script.");
        }
    }
}
