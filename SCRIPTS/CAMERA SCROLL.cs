using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ScrollCamera : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public RectTransform mapCanvas;  // Reference to the map canvas
    public List<Button> stopScrollButtons; // List of buttons to stop scrolling
    private bool canScroll = true; // Flag to control camera scrolling
    private bool isDraggingTowerButton = false; // Flag to track whether the tower button is being dragged
    private bool isButtonPressed = false; // Flag to track whether any button is being pressed

    void Start()
    {
        foreach (Button button in stopScrollButtons)
        {
            // Add an onClick event to each button in the list
            button.onClick.AddListener(() => ButtonPressed(true));
        }
    }

    void Update()
    {
        if (canScroll && !isButtonPressed)
        {
            HandleScrollInput();
        }
    }

    void ButtonPressed(bool value)
    {
        isButtonPressed = value;
    }

    void HandleScrollInput()
    {
        if (Input.touchSupported)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // Handle single touch for scrolling
            if (touch.phase == TouchPhase.Moved && !isDraggingTowerButton)
            {
                Vector2 deltaPosition = touch.deltaPosition;
                ScrollCameraWithDelta(deltaPosition);
            }
        }
    }

    void HandleMouseInput()
    {
        // Handle mouse or touch input for scrolling
        if (Input.GetMouseButton(0) && !isDraggingTowerButton)
        {
            Vector2 inputPosition = Input.mousePosition;

            // Check if the mouse or touch is not over a UI element
            if (!EventSystem.current.IsPointerOverGameObject() || !EventSystem.current.IsPointerOverGameObject(0))
            {
                Vector2 deltaMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                ScrollCameraWithDelta(deltaMouse);
            }
        }
    }

    void ScrollCameraWithDelta(Vector2 delta)
    {
        if (mapCanvas == null)
        {
            Debug.LogError("Map Canvas not assigned in the inspector!");
            return;
        }

        // Calculate the horizontal scrolling direction and apply it to the camera's position
        float horizontalDelta = -delta.x;
        Vector3 scrollDirection = new Vector3(horizontalDelta, 0, 0);
        Vector3 newPosition = transform.position + scrollDirection * scrollSpeed * Time.deltaTime;

        // Clamp the camera position within the bounds of the map canvas
        float mapCanvasWidth = mapCanvas.rect.width * mapCanvas.lossyScale.x;

        float minX = mapCanvas.position.x - mapCanvasWidth / 10f;
        float maxX = mapCanvas.position.x + mapCanvasWidth / 10f;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // Update the camera's position
        transform.position = newPosition;
    }
}
