//===========================================================================//
//                       FreeFlyCamera (Version 1.2)                         //
//                        (c) 2019 Sergey Stafeyev                           //
//===========================================================================//

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{
    #region UI

    [Space]

    [SerializeField]
    [Tooltip("Sensitivity of mouse rotation")]
    private float _mouseSense = 1.8f;

    [Space]

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float _baseMovementSpeed = 10f;

    [SerializeField]
    [Tooltip("Boost speed - Doubles current camera speed while held down.")]
    private KeyCode _boostSpeed = KeyCode.LeftShift;

    [Space]

    [SerializeField]
    [Tooltip("Amount to adjust camera movement by when using the scroll wheel")]
    private float _speedScalarFactor = 0.5f;

    #endregion UI

    private float _currentSpeedScalar = 1;

    private void Update()
    {
        // Movement
        Vector3 deltaPosition = Vector3.zero;
        float currentSpeed = _baseMovementSpeed;

        if (Input.GetKey(_boostSpeed))
            currentSpeed *= 2;

        if (Input.GetKey(KeyCode.W))
            deltaPosition += transform.forward;

        if (Input.GetKey(KeyCode.S))
            deltaPosition -= transform.forward;

        if (Input.GetKey(KeyCode.A))
            deltaPosition -= transform.right;

        if (Input.GetKey(KeyCode.D))
            deltaPosition += transform.right;

        if (Input.GetKey(KeyCode.E))
            deltaPosition += transform.up;

        if (Input.GetKey(KeyCode.Q))
            deltaPosition -= transform.up;

        // Calc speed scalar
        _currentSpeedScalar += Input.mouseScrollDelta.y * _speedScalarFactor;
        if(_currentSpeedScalar < 0.01f)
        {
            _currentSpeedScalar = 0.01f;
        }

        transform.position += deltaPosition * currentSpeed * _currentSpeedScalar;


        // Rotation
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !IsMouseOverUI())
        {
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -Input.GetAxis("Mouse Y") * _mouseSense,
                Vector3.right
            );

            // Yaw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                transform.eulerAngles.z
            );
        }
    }

    public static bool IsMouseOverUI()
    {
        EventSystem eventSystem = EventSystem.current;
        
        // && !eventSystem.currentSelectedGameObject.GetComponent<EntityLabel>().Equals(null)
        return (eventSystem != null && eventSystem.currentSelectedGameObject != null && !eventSystem.currentSelectedGameObject.tag.Equals("EntityLabel"));
    }
}
