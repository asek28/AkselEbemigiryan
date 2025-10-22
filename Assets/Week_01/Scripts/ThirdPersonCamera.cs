using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Kamera Ayarları")]
    public Transform target; // Takip edilecek hedef (Player)
    public float distance = 5f; // Player'dan uzaklık
    public float height = 2f; // Yükseklik
    public float mouseSensitivity = 2f; // Mouse hassasiyeti
    public float smoothSpeed = 5f; // Yumuşak hareket hızı
    
    [Header("Kamera Sınırları")]
    public float minVerticalAngle = -30f; // Minimum dikey açı
    public float maxVerticalAngle = 60f; // Maksimum dikey açı
    public float minDistance = 2f; // Minimum mesafe
    public float maxDistance = 10f; // Maksimum mesafe
    
    [Header("Collision Detection")]
    public LayerMask obstacleLayer = -1; // Engel katmanları
    public float collisionRadius = 0.3f; // Collision yarıçapı
    
    private float currentX = 0f; // Yatay açı
    private float currentY = 0f; // Dikey açı
    private float currentDistance; // Mevcut mesafe
    private Vector3 currentVelocity; // Yumuşak hareket için
    
    void Start()
    {
        // Hedef atanmamışsa Player'ı bul
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("Player bulunamadı! Target'ı manuel olarak atayın.");
            }
        }
        
        // Başlangıç mesafesini ayarla
        currentDistance = distance;
        
        // Mouse'u kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Mouse input'u al
        HandleMouseInput();
        
        // Kamera pozisyonunu güncelle
        UpdateCameraPosition();
    }
    
    void HandleMouseInput()
    {
        // Mouse hareketini al
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Açıları güncelle
        currentX += mouseX;
        currentY -= mouseY; // Y eksenini ters çevir
        
        // Dikey açıyı sınırla
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        
        // Mouse wheel ile mesafe ayarla
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * 2f;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }
    
    void UpdateCameraPosition()
    {
        if (target == null) return;
        
        // Hedef pozisyonu hesapla
        Vector3 targetPosition = CalculateTargetPosition();
        
        // Collision detection
        Vector3 finalPosition = CheckForObstacles(targetPosition);
        
        // Kamerayı yumuşak bir şekilde hareket ettir
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref currentVelocity, 1f / smoothSpeed);
        
        // Kamerayı hedefe bakacak şekilde döndür
        transform.LookAt(target.position + Vector3.up * height);
    }
    
    Vector3 CalculateTargetPosition()
    {
        // Açıları radyana çevir
        float radiansX = currentX * Mathf.Deg2Rad;
        float radiansY = currentY * Mathf.Deg2Rad;
        
        // Kameranın hedef pozisyonunu hesapla
        Vector3 direction = new Vector3(
            Mathf.Sin(radiansX) * Mathf.Cos(radiansY),
            Mathf.Sin(radiansY),
            Mathf.Cos(radiansX) * Mathf.Cos(radiansY)
        );
        
        return target.position + direction * currentDistance + Vector3.up * height;
    }
    
    Vector3 CheckForObstacles(Vector3 targetPosition)
    {
        // Hedef ile kamera arasındaki mesafeyi hesapla
        Vector3 direction = targetPosition - target.position;
        float distance = direction.magnitude;
        
        // Raycast ile engel kontrolü
        RaycastHit hit;
        if (Physics.SphereCast(target.position, collisionRadius, direction.normalized, out hit, distance, obstacleLayer))
        {
            // Engel bulundu, kamerayı engelden biraz uzakta konumlandır
            return hit.point - direction.normalized * collisionRadius;
        }
        
        return targetPosition;
    }
    
    // Kamera sıfırlama
    public void ResetCamera()
    {
        currentX = 0f;
        currentY = 0f;
        currentDistance = distance;
    }
    
    // Mouse kilidini aç/kapat
    public void ToggleMouseLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    // ESC tuşu ile mouse kilidini aç
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMouseLock();
        }
    }
}
