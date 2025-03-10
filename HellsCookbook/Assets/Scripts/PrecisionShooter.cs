using UnityEngine;

public class PrecisionShooter : MonoBehaviour
{
    [Header("Настройки стрельбы")]
    public GameObject bulletPrefab;    // Префаб пули
    public Transform firePoint;       // Точка вылета пуль
    public float bulletSpeed = 50f;   // Скорость пули
    public LayerMask targetMask;      // Слои для проверки попадания

    [Header("Прицел")]
    public RectTransform crosshair;   // UI-объект прицела
    public float maxRayDistance = 100f; // Макс. дистанция луча

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
        // Центрирование прицела (если используется UI)
        if (crosshair != null)
        {
            crosshair.position = mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        }
    }

    void Shoot()
    {
        // Получаем точку прицеливания через луч из центра экрана
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

        // Рассчитываем направление с учетом точки выстрела
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // Создаем пулю
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        // Задаем скорость
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }
    }

    // Визуализация луча для отладки
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