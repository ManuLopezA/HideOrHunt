using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

[RequireComponent(typeof(LineRenderer))]
public class ShootController : NetworkBehaviour
{
    [SerializeField] private LayerMask serverLayerMask;
    [SerializeField] private LayerMask clientLayerMask;
    [SerializeField] private float shootDuration;
    // [SerializeField] private AudioClip shootAudioClip;

    private float elapsed;

    private LineRenderer shootLine;

    public Action<RaycastHit> m_Action;

    private void Awake()
    {
        shootLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!IsClient) return;
        elapsed += Time.deltaTime;
    }

    public void Shoot(InputAction.CallbackContext _)
    {

        if (elapsed < Constants.Instance.ActionCooldownSec) return;
        elapsed = 0;
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, clientLayerMask)) return;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red, 5);
        Vector3 point = hit.point;
        point.y = 0.2f;
        Vector3 origin = transform.position;
        origin.y = 0.2f;
        Vector3 direction = (point - origin).normalized;
        SendDirectionRpc(direction, point);
    }

    private IEnumerator ShowLaser()
    {
        shootLine.enabled = true;
        yield return new WaitForSeconds(shootDuration);
        shootLine.enabled = false;
    }

    //En aquesta funcio es mira si el player es Hider o Hunter.
    [Rpc(SendTo.Everyone)]
    private void SendDirectionRpc(Vector3 direction, Vector3 point)
    {

        if (IsClient)
        {
            shootLine.SetPosition(0, transform.position);
            shootLine.SetPosition(1, point);
            StartCoroutine(ShowLaser());

        }
        if (IsServer)
        {
            if (GetComponent<RoleController>().role.Value == Role.HUNTER)
                GetComponent<AudioController>().PlayAudioRpc(2);
            if (m_Action == null) return;
            Vector3 origin = transform.position;
            origin.y = 0.2f;
            if (!Physics.Raycast(origin, direction, out var hit, Constants.Instance.ActionRange, serverLayerMask)) return;
            Debug.DrawLine(origin, hit.point, Color.green, 5);
            m_Action(hit);
        }
    }

    public void HandleHiderTransformAction(RaycastHit hit)
    {
        if (!hit.collider.TryGetComponent(out PropController prop)) return;
        if (!GetComponent<RewardExecutorDestransform>().CanTransform()) return;
        // GetComponent<Rigidbody>().velocity = Vector3.zero;
        // GetComponent<AnimationStateMachine>().ForceIdle();
        GetComponent<TransformController>().TransformRpc(prop.name, prop.Size);
        GetComponent<MissionCompleterTransform>().OnAfterTransform(prop.gameObject);
    }

    public void HandleHunterShootAction(RaycastHit hit)
    {

        if (!hit.collider.TryGetComponent(out RoleController roleController))
        {
            GetComponent<IDamageable>().DamageHunter();
            return;
        }
        if (roleController.role.Value != Role.HIDER) return;
        if (hit.collider.GetComponent<IDamageable>().DamageHider())
        {
            PointsManager.Instance.KilledPlayer(GetComponent<NetworkObject>().OwnerClientId, GetComponent<PlayerController>()._name.Value.ToString());
            string otherNick = roleController.GetComponent<PlayerController>()._name.Value.ToString();
            LogManager.Instance.LogAllRpc($"{otherNick} has been eliminated", LogType.DANGER);

            // GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(shootAudioClip);
        };
        if (hit.collider.GetComponent<RewardExecutorFreeze>().HasFreeze(GetComponent<PlayerRewardController>()))
        {
            print($"freezing myself");
        }
    }
}
