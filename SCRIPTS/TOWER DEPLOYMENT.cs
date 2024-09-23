using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerDeployment : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab;
    public int towerCost = 50;
    public RectTransform mapCanvas;

    private GameObject currentTower;
    private MoneyManager moneyManager;

    private bool isButtonPressed = false;
    private bool isTowerPlaced = false;
    public Image spriteRenderer; // Reference to the SpriteRenderer component

    void Start()
    {
        moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager script not found in the scene.");
        }
       
    }

    void Update()
    {
        if (moneyManager != null && moneyManager.currentMoney >= towerCost)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.grey;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (moneyManager != null && moneyManager.currentMoney >= towerCost)
        {
            isButtonPressed = true;
        }
        else
        {
            isButtonPressed = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isButtonPressed && moneyManager != null && moneyManager.currentMoney >= towerCost)
        {
            if (currentTower == null)
            {
                DeployTower();
            }
            else
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPosition.z = 0;

                // Check if the new position is within the boundaries
                if (IsWithinBoundaries(newPosition))
                {
                    currentTower.transform.position = newPosition;
                    UpdateTowerColor(newPosition);
                }
            }
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentTower != null)
        {
            if (isTowerPlaced)
            {
                // Subtract tower cost if the tower was successfully placed
                moneyManager.SubtractMoney(towerCost);
            }

            if (!isTowerPlaced)
            {
                CancelTowerPlacement();
            }

            isButtonPressed = false;
            isTowerPlaced = false;
            currentTower = null;
            Update();
        }
    }


    void DeployTower()
    {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;

        // Check if the spawn position is within the boundaries
        if (moneyManager != null && moneyManager.currentMoney >= towerCost && IsWithinBoundaries(spawnPosition))
        {
            if (towerPrefab != null)
            {
                currentTower = Instantiate(towerPrefab, spawnPosition, Quaternion.identity);

                SetTowerColor(Color.white); // Set tower color to default

                isTowerPlaced = true;
            }
        }
    }


    void CancelTowerPlacement()
    {
        if (currentTower != null)
        {
            Destroy(currentTower);
            currentTower = null;
        }
    }

    bool IsWithinBoundaries(Vector3 position)
    {
        // Get the boundaries of the map canvas
        float mapCanvasWidth = mapCanvas.rect.width * mapCanvas.lossyScale.x;
        float mapCanvasHeight = mapCanvas.rect.height * mapCanvas.lossyScale.y;

        // Calculate the minimum and maximum bounds
        float minX = mapCanvas.position.x - mapCanvasWidth / 3f;
        float maxX = mapCanvas.position.x + mapCanvasWidth / 3f;
        float minY = mapCanvas.position.y - mapCanvasHeight / 4f;
        float maxY = mapCanvas.position.y + mapCanvasHeight / 4f;

        // Check if the position is within the boundaries
        return (position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY);
    }

    void UpdateTowerColor(Vector3 position)
    {
        if (!IsWithinBoundaries(position))
        {
            SetTowerColor(Color.red, 0.5f);
        }
        else
        {
            SetTowerColor(Color.white);
        }
    }

    void SetTowerColor(Color color, float opacity = 1f)
    {
        if (currentTower != null)
        {
            Renderer towerRenderer = currentTower.GetComponent<Renderer>();
            if (towerRenderer != null)
            {
                towerRenderer.material.color = new Color(color.r, color.g, color.b, opacity);
            }
        }
    }
}