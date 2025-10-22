using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Kapı Ayarları")]
    public float openDistance = 3f; // Kapının açılacağı mesafe
    public float openSpeed = 2f; // Açılma hızı
    public bool isOpen = false; // Kapının açık olup olmadığı
    
    [Header("Kapı Tipi")]
    public DoorType doorType = DoorType.Up; // Kapı açılma yönü
    
    public enum DoorType
    {
        Up,     // Yukarı açılır
        Right,  // Sağa açılır
        Left,   // Sola açılır
        Down    // Aşağı açılır
    }
    
    [Header("Ses Efektleri")]
    public AudioClip openSound;
    public AudioClip closeSound;
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private AudioSource audioSource;
    private bool isMoving = false;
    
    void Start()
    {
        // Başlangıç pozisyonunu kaydet
        closedPosition = transform.position;
        
        // Kapı tipine göre açık pozisyonu hesapla
        Vector3 openDirection = GetOpenDirection();
        openPosition = closedPosition + openDirection * openDistance;
        
        // AudioSource component'ini al veya ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Başlangıçta kapı kapalı olsun
        if (isOpen)
        {
            transform.position = openPosition;
        }
        else
        {
            transform.position = closedPosition;
        }
    }
    
    void Update()
    {
        if (isMoving)
        {
            MoveDoor();
        }
    }
    
    public void ToggleDoor()
    {
        Debug.Log($"ToggleDoor çağrıldı! Mevcut durum: {isOpen}, Hareket halinde: {isMoving}");
        
        if (!isMoving)
        {
            isOpen = !isOpen;
            isMoving = true;
            
            Debug.Log($"Kapı durumu değişti: {isOpen}");
            
            // Ses efekti çal
            PlayDoorSound();
        }
        else
        {
            Debug.Log("Kapı zaten hareket halinde!");
        }
    }
    
    private void MoveDoor()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        Debug.Log($"MoveDoor: Target={targetPosition}, Current={transform.position}, Distance={distance}");
        
        if (distance > 0.01f)
        {
            // Kapıyı hedef pozisyona doğru hareket ettir
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
            transform.position = newPosition;
            Debug.Log($"Kapı hareket ediyor: {transform.position}");
        }
        else
        {
            // Hedef pozisyona ulaştı
            transform.position = targetPosition;
            isMoving = false;
            Debug.Log($"Kapı hedef pozisyona ulaştı! isMoving = {isMoving}");
        }
    }
    
    private void PlayDoorSound()
    {
        if (audioSource != null)
        {
            AudioClip soundToPlay = isOpen ? openSound : closeSound;
            if (soundToPlay != null)
            {
                audioSource.PlayOneShot(soundToPlay);
            }
        }
    }
    
    // Kapıyı zorla aç/kapat (başka script'lerden çağrılabilir)
    public void SetDoorState(bool open)
    {
        if (!isMoving)
        {
            isOpen = open;
            isMoving = true;
            PlayDoorSound();
        }
    }
    
    // Kapının açık olup olmadığını kontrol et
    public bool IsDoorOpen()
    {
        return isOpen;
    }
    
    // Kapı tipine göre açılma yönünü hesapla
    private Vector3 GetOpenDirection()
    {
        switch (doorType)
        {
            case DoorType.Up:
                return Vector3.up;
            case DoorType.Right:
                return Vector3.right;
            case DoorType.Left:
                return Vector3.left;
            case DoorType.Down:
                return Vector3.down;
            default:
                return Vector3.up;
        }
    }
}
