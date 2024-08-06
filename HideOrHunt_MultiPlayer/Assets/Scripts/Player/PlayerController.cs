using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShootController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Input")][SerializeField] private InputActionAsset inputAsset;

    private InputActionAsset Input { get; set; }
    private InputActionMap _actionMap;
    private InputAction _movement;
    private InputAction _spectatorCam;
    private InputAction _spectatorUpDownCam;
    public InputAction clone;
    public InputAction shoot;

    public InputAction anchor;
    [SerializeField] private GameObject candau;

    [Header("Executors")]
    private RewardExecutorSpeed rewardExecutorSpeed;
    private RewardExecutorClone rewardExecutorClone;

    [Header("Component")] private Rigidbody _rigidbody;
    private RoleController _roleController;
    private ShootController _shootController;
    private HealthController _healthController;

    private GameObject _camera;
    [SerializeField] private SpriteSelector _spriteSelector;
    public TextMeshPro nick;
    public bool isDummy;
    public bool isClone;

    public NetworkVariable<FixedString64Bytes> _name = new();
    public NetworkVariable<int> _characterSelected = new();

    [SerializeField] public float speed;

    [Header("Camera Settings")]
    [SerializeField]
    private float depth = 15;

    [SerializeField] private float height = 9;
    private float camPitch;
    [SerializeField] private float m_MouseSensitivity;
    [SerializeField] private float m_CameraZSpeed;
    [SerializeField] private float m_CameraYSpeed;
    public GameObject minimapDot;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _roleController = GetComponent<RoleController>();
        _shootController = GetComponent<ShootController>();
        _healthController = GetComponent<HealthController>();
        rewardExecutorClone = GetComponent<RewardExecutorClone>();
        rewardExecutorSpeed = GetComponent<RewardExecutorSpeed>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnSetName("", _name.Value);
        OnSetCharacter(0, _characterSelected.Value);
        _name.OnValueChanged += OnSetName;
        _characterSelected.OnValueChanged += OnSetCharacter;

        if (IsServer && !isClone)
        {
            GameManager.Instance.All.Add(this);
        }

        if (isDummy) return;
        if (!IsOwner) return;

        SetNameRpc(GameManager.Instance.nickname);
        SetCharacterRpc(GameManager.Instance.gm_selectedCharacter);

        if (isClone) return;

        GameManager.Instance.owner = this;
        minimapDot.SetActive(true);
        minimapDot.GetComponent<MeshRenderer>().material.color = Color.green;
        InitInput();
    }

    private void OnSetName(FixedString64Bytes oldName, FixedString64Bytes newName)
    {
        nick.text = newName.Value;
        name = "Player " + newName.Value;
    }

    private void OnSetCharacter(int _, int newArchetypeIndex)
    {
        _spriteSelector.ChangeArchetype(newArchetypeIndex);
    }

    [Rpc(SendTo.Server)]
    public void SetNameRpc(string nickname)
    {
        _name.Value = nickname;
    }

    [Rpc(SendTo.Server)]
    private void SetCharacterRpc(int archetypeIndex)
    {
        _characterSelected.Value = archetypeIndex;
    }

    private void InitInput()
    {
        Assert.IsNotNull(inputAsset);
        Input = Instantiate(inputAsset);
        _actionMap = Input.FindActionMap("Gameplay");
        _movement = _actionMap.FindAction("Move");
        shoot = _actionMap.FindAction("Shoot");
        clone = _actionMap.FindAction("Action");
        anchor = _actionMap.FindAction("Anchor");
        _spectatorUpDownCam = _actionMap.FindAction("UpDown");
        _spectatorCam = _actionMap.FindAction("Camera");
        _actionMap.Enable();
    }

    private void Update()
    {
        if (IsServer)
        {
            var movement = new Vector3(movementVector.x, 0, movementVector.y);
            float newSpeed = rewardExecutorSpeed.OnBeforeMove(speed);
            if (_healthController.hasBeenRecentlyShot)
                newSpeed *= Constants.Instance.HiderHurtSpeedMultiplier;
            _rigidbody.velocity = movement * newSpeed;
            GetComponent<MissionCompleterPosition>().OnAfterMove(transform.position);
        }

        if (!IsOwner) return;
        if (isDummy) return;
        if (IsClient)
        {
            Move();
        }

        if (_roleController.role.Value == Role.SPECTATOR)
        {
            MoveSpectatorCamera();
            MoveMouseSpectatorCamera();
            return;
        }

        CameraFollow();
    }

    private bool clientFrozenState;
    public void Anchor(InputAction.CallbackContext _)
    {
        clientFrozenState = !clientFrozenState;
        AnchorServerRpc(clientFrozenState);
        candau.SetActive(clientFrozenState);
    }

    [Rpc(SendTo.Server)]
    public void AnchorServerRpc(bool freeze)
    {
        if (freeze)
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private Vector2 movementVector = Vector2.zero;

    private void Move()
    {
        Vector2 newMovementVector = _movement.ReadValue<Vector2>();
        if (newMovementVector == movementVector) return;
        movementVector = newMovementVector;
        MoveServerRpc(movementVector);
    }

    public void RecalculateSpeed()
    {
        MoveServerRpc(movementVector);
    }

    [Rpc(SendTo.Server)]
    private void MoveServerRpc(Vector2 vector)
    {
        movementVector = vector;
    }

    private void CameraFollow()
    {
        if (!IsOwner) return;
        if (_camera == null) return;
        Vector3 playerPosition = transform.position;
        Vector3 newPosition = new Vector3(
            playerPosition.x,
            playerPosition.y + height,
            playerPosition.z - depth
        );
        _camera.transform.position = newPosition;
    }


    private void MoveSpectatorCamera()
    {
        //UpDown
        float value = _spectatorUpDownCam.ReadValue<float>();
        Vector3 ot = _camera.transform.position;
        //pendiente time delta time
        _camera.transform.position = new Vector3(ot.x, ot.y + value / m_CameraYSpeed, ot.z);
        //ForwardBackwards
        Vector2 movement = _movement.ReadValue<Vector2>();
        Vector3 movementVector = new(movement.x, 0, movement.y);
        _camera.transform.position += movementVector * m_CameraZSpeed;
    }

    private void MoveMouseSpectatorCamera()
    {
        Vector2 mouseVector = m_MouseSensitivity * Time.deltaTime * _spectatorCam.ReadValue<Vector2>();
        _camera.transform.Rotate(Vector3.up * mouseVector.x);
        RotateCameraY(mouseVector.y);
    }

    private void RotateCameraY(float amount)
    {
        camPitch += amount;
        camPitch = Mathf.Clamp(camPitch, -90, 90);
        _camera.transform.localEulerAngles = new Vector3(-camPitch, 0, 0);
    }

    public void SceneChange()
    {
        if (!IsOwner) return;
        if (isDummy) return;
        clientFrozenState = false;
        AnchorServerRpc(clientFrozenState);
        _camera = GameObject.Find("Main Camera");
    }
}
