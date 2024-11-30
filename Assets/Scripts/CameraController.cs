using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput inputActions;
    [SerializeField] private Transform orientation;
    [SerializeField] private float sensX;

    private float yRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (GameManager.Instance.playerSC.health <= 0)
        {
            transform.SetParent(orientation);
            return;
        }
        Vector2 input = inputActions.actions["Look"].ReadValue<Vector2>();     
        
        float mouseX = input.x * sensX;
        yRotation += mouseX;

        // Actual Rotation
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

}
