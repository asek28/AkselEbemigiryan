using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput input;
    private Rigidbody rb;

    private Vector2 movDir;
    public float movSpd = 5f; // Varsayılan hareket hızı
    public float jumpForce = 10f; // Zıplama kuvveti
    public bool isGrounded = false; // Yerde olup olmadığını kontrol et
    
    [Header("Kamera Referansı")]
    public Transform cameraTransform; // Kamera referansı
    public bool useCameraRelativeMovement = true; // Kamera göreli hareket kullan
    
    [Header("Third Person Kamera")]
    public bool useThirdPersonCamera = true; // Third person kamera kullan

    private InputAction move;
    private InputAction jump;
    private InputAction interact;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        
        // Input ve actions kontrolü
        if (input == null)
        {
            Debug.LogError("PlayerInput component bulunamadı! PlayerController'a PlayerInput component ekleyin.");
            return;
        }
        
        if (input.actions == null)
        {
            Debug.LogError("Input Actions bulunamadı! PlayerInput component'ine Input Actions asset atayın.");
            return;
        }
        
        move = input.actions["Move"];
        jump = input.actions["Jump"];
        interact = input.actions["Interact"];
        
        if (move == null)
        {
            Debug.LogError("KRİTİK HATA: 'Move' eylemi bulunamadı. Input Actions asset'inde 'Move' eylemi tanımlı olduğundan emin olun.");
            return;
        }

        move.Enable();
        if (jump != null)
        {
            jump.Enable();
        }
        if (interact != null)
        {
            interact.Enable();
        }
        
        // Rigidbody ayarları - karakterin düşmesini engelle
        rb.freezeRotation = true; // Rotasyonu dondur
        rb.linearDamping = 5f; // Hava direnci
        rb.angularDamping = 5f; // Açısal direnç
        
        // Kamera referansını otomatik bul
        if (cameraTransform == null)
        {
            if (useThirdPersonCamera)
            {
                // Third person kamera kullan
                ThirdPersonCamera thirdPersonCam = FindObjectOfType<ThirdPersonCamera>();
                if (thirdPersonCam != null)
                {
                    cameraTransform = thirdPersonCam.transform;
                }
            }
            else
            {
                // Ana kamera kullan
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }
            }
            
            if (cameraTransform == null)
            {
                Debug.LogWarning("Kamera bulunamadı! Kamera referansını manuel olarak atayın.");
            }
        }
    }

    void Update()
    {
        // Null kontrolü ekle
        if (move != null)
        {
            movDir = move.ReadValue<Vector2>();
        }
        else
        {
            movDir = Vector2.zero;
        }
        
        // Zıplama kontrolü
        if (jump != null && jump.WasPressedThisFrame() && isGrounded)
        {
            Jump();
        }
        
        // Etkileşim kontrolü
        if (interact != null && interact.WasPressedThisFrame())
        {
            TryInteract();
        }
    }

    private void FixedUpdate()
    {
        // Yerde olup olmadığını kontrol et
        CheckGrounded();
        
        if (movDir != Vector2.zero)
        {
            Vector3 movement;
            
            if (useCameraRelativeMovement && cameraTransform != null)
            {
                // Kamera göreli hareket
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                
                // Y eksenini sıfırla (sadece yatay hareket)
                forward.y = 0f;
                right.y = 0f;
                
                // Normalize et
                forward.Normalize();
                right.Normalize();
                
                // Hareket vektörünü hesapla
                movement = (forward * movDir.y + right * movDir.x) * movSpd;
            }
            else
            {
                // Dünya koordinatlarında hareket
                movement = new Vector3(movDir.x * movSpd, 0, movDir.y * movSpd);
            }
            
            // Y hızını koru (gravity için)
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
    }
    
    private void CheckGrounded()
    {
        // Raycast ile yerde olup olmadığını kontrol et
        RaycastHit hit;
        float rayDistance = 1.1f; // Karakterin altından biraz aşağıya bak
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }
    
    private void TryInteract()
    {
        Debug.Log("E tuşuna basıldı! Tüm kapılar kontrol ediliyor...");
        
        // Tüm kapıları bul ve aç/kapat
        DoorController[] allDoors = FindObjectsOfType<DoorController>();
        Debug.Log($"Sahnedeki toplam kapı sayısı: {allDoors.Length}");
        
        if (allDoors.Length > 0)
        {
            // Tüm kapıları aynı anda aç/kapat
            foreach (DoorController door in allDoors)
            {
                Debug.Log($"Kapı bulundu: {door.name}");
                door.ToggleDoor();
            }
            Debug.Log("Tüm kapılar açıldı/kapandı!");
        }
        else
        {
            Debug.Log("Sahne'de hiç kapı bulunamadı!");
        }
    }
}