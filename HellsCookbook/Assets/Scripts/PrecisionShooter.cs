using UnityEngine;

public class PrecisionShooter : MonoBehaviour
{
    [Header("��������� ��������")]
    public GameObject bulletPrefab;    // ������ ����
    public Transform firePoint;       // ����� ������ ����
    public float bulletSpeed = 50f;   // �������� ����
    public LayerMask targetMask;      // ���� ��� �������� ���������

    [Header("������")]
    public RectTransform crosshair;   // UI-������ �������
    public float maxRayDistance = 100f; // ����. ��������� ����

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        UpdateCrosshair();

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void UpdateCrosshair()
    {
        // ������������� ������� (���� ������������ UI)
        if (crosshair != null)
        {
            crosshair.position = mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        }
    }

    void Shoot()
    {
        // �������� ����� ������������ ����� ��� �� ������ ������
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxRayDistance, targetMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxRayDistance);
        }

        // ������������ ����������� � ������ ����� ��������
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // ������� ����
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        // ������ ��������
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }
    }

    // ������������ ���� ��� �������
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