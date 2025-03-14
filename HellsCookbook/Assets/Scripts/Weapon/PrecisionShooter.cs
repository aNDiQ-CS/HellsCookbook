using UnityEngine;

public class PrecisionShooter : MonoBehaviour
{
    [Header("��������� ��������")]
    [SerializeField] private GameObject bulletPrefab;    // ������ ����
    [SerializeField] private Transform firePoint;       // ����� ������ ����
    [SerializeField] private float bulletSpeed = 50f;   // �������� ����
    [SerializeField] private float fireRate = 10f;      // ���������������� (��������� � �������)
    [SerializeField] private LayerMask targetMask;      // ���� ��� �������� ���������    

    [Header("������")]
    [SerializeField] private RectTransform crosshair;   // UI-������ �������
    [SerializeField] private float maxRayDistance = 100f; // ����. ��������� ����

    [Header("�������� ������")]
    [SerializeField] private Transform weaponModel;
    [SerializeField] private Vector3 raisedWeaponPosition = new Vector3(0, 0.2f, 0);
    [SerializeField] private Vector3 raisedWeaponRotation = new Vector3(0, -90f, 0);
    [SerializeField] private float raiseSpeed = 15f;

    private PlayerController playerController;
    private Vector3 defaultWeaponPosition;
    private Quaternion defaultWeaponRotation;

    private Camera mainCamera;
    private float nextFireTime;       // ����� ���������� ��������

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        nextFireTime = 0f;

        playerController = GetComponent<PlayerController>();  
        defaultWeaponPosition = weaponModel.localPosition;
        defaultWeaponRotation = weaponModel.localRotation;
    }

    void Update()
    {
        UpdateCrosshair();
        HandleWeaponRaise();

        // �������������� �������� ��� ������� ���
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !playerController.IsRunning)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void UpdateCrosshair()
    {
        if (crosshair != null)
        {
            crosshair.position = mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        }
    }

    void Shoot()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit, maxRayDistance, targetMask)
            ? hit.point
            : ray.GetPoint(maxRayDistance);

        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position,
            Quaternion.LookRotation(shootDirection));

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = shootDirection * bulletSpeed;
        }

    }

    void HandleWeaponRaise()
    {
        Vector3 targetPosition = playerController.IsRunning
            ? defaultWeaponPosition + raisedWeaponPosition
            : defaultWeaponPosition;

        Quaternion targetRotation = playerController.IsRunning
            ? Quaternion.Euler(raisedWeaponRotation)
            : defaultWeaponRotation;

        weaponModel.localPosition = Vector3.Lerp(
            weaponModel.localPosition,
            targetPosition,
            Time.deltaTime * raiseSpeed
        );
        weaponModel.localRotation = Quaternion.Lerp(
            weaponModel.localRotation,
            targetRotation,
            Time.deltaTime * raiseSpeed
        );

        // ������������� ����� ��������
        /*weaponAnimator.SetBool("IsRunning", playerMovement.IsRunning);*/
    }

    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            Gizmos.color = Color.red;
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * maxRayDistance);
        }
    }
}