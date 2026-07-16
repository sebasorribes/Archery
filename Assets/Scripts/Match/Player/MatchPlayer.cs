using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MatchPlayer : MonoBehaviour
{
    //temporal, despues acomodar en script correcto para buenas practicas
    private InputActionAsset inputActions;
    private InputAction leftMouse;
    private InputAction rightMouse;
    private InputAction chargeMouse;
    private InputAction aimAction;


    private float maxPower = 40f;
    private float currentPower;
    private float sensitivity = 0.01f;

    private bool leftPressed;
    private bool rightPressed;
    private bool isCharging;

    private float xCurrentYaw = 0f;
    private float yCurrentYaw = 0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform bowPivot;

    private float xVelocity = 0f;
    private float yVelocity = 0f;

    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float maxVelocity = 150f;

    [SerializeField] private float minY = -30f;
    [SerializeField] private float maxY = 30f;

    [SerializeField] private float minX = -30f;
    [SerializeField] private float maxX = 30f;

    //Despues arreglar
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject bow;

    private GameObject currentArrow;

    void Start()
    {
        AssignActions();
        ConnectActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation, bow.transform);
    }

    private void AssignActions()
    {
        var playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

        leftMouse = inputActions.FindAction("LeftClick");
        rightMouse = inputActions.FindAction("RightClick");
        chargeMouse = inputActions.FindAction("ChargePower");
        aimAction = inputActions.FindAction("Aim");
    }

    private void ConnectActions()
    {
        leftMouse.performed += OnLeftClick;
        leftMouse.canceled += OnLeftClick;
        rightMouse.performed += OnRightClick;
        rightMouse.canceled += OnRightClick;
        aimAction.performed += OnAim;
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        leftPressed = context.ReadValueAsButton();
        CheckChargingState(context);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        rightPressed = context.ReadValueAsButton();
        CheckChargingState(context);
    }

    void CheckChargingState(InputAction.CallbackContext context)
    {
        if (leftPressed && rightPressed)
        {
            if (!isCharging)
            {
                StartCharge();
            }
        }
        else
        {
            if (isCharging)
            {
                ReleaseShot();
            }
        }
    }

    void StartCharge()
    {
        isCharging = true;
        currentPower = 0f;
        Debug.Log("Charging");
    }

    void ReleaseShot()
    {
        Shoot(currentPower);
    }

    //agregar validacion de que hay flecha
    void Shoot(float power)
    {
        currentArrow.GetComponent<Arrow>().Shoot(currentPower);
        StartCoroutine(SpawnArrow());
    }

    IEnumerator SpawnArrow()
    {
        yield return new WaitForSeconds(.1f);
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation, bow.transform);
        isCharging = false;
    }
    
    private void OnAim(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();

        xVelocity += mouseDelta.x * acceleration * Time.deltaTime;
        yVelocity += -mouseDelta.y * acceleration * Time.deltaTime;

        xVelocity = Mathf.Clamp(xVelocity, -maxVelocity, maxVelocity);
        yVelocity = Mathf.Clamp(yVelocity, -maxVelocity, maxVelocity);
    }

    
    private void ManageXRotation(float delta)
    {
        if (isCharging) return;
        xCurrentYaw += delta;
        Quaternion newRotation = Quaternion.Euler(0f, xCurrentYaw, 0f);

        transform.rotation = newRotation;
        //bowPivot.rotation = newRotation;

        currentArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        currentArrow.transform.localPosition = new Vector3(0,0,0);
    }
    private void ManageYRotation(float delta)
    {
        if (isCharging) return;
        yCurrentYaw += delta;

        playerCamera.transform.localRotation = Quaternion.Euler(yCurrentYaw, 0f, 0f);
        //bowPivot.rotation = Quaternion.Euler(yCurrentYaw, 0f, 0f);

        currentArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        currentArrow.transform.localPosition = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (isCharging)
        {
            Vector2 mouseDelta = chargeMouse.ReadValue<Vector2>();

            float pull = -mouseDelta.y;

            if (pull > 0)
            {
                currentPower += pull * sensitivity;
                currentPower = Mathf.Clamp(currentPower, 0, maxPower);

                Debug.Log("Current Power: " + currentPower);
            }
        }
        else
        {
            xCurrentYaw += xVelocity * Time.deltaTime;
            yCurrentYaw += yVelocity * Time.deltaTime;

            xCurrentYaw = Mathf.Clamp(xCurrentYaw, minX, maxX);
            yCurrentYaw = Mathf.Clamp(yCurrentYaw, minY, maxY);

            transform.rotation = Quaternion.Euler(0f, xCurrentYaw, 0f);
            playerCamera.transform.localRotation = Quaternion.Euler(yCurrentYaw, 0f, 0f);

            currentArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            currentArrow.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
