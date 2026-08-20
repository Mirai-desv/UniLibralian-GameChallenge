using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(BookController))]
public class Book : MonoBehaviour, IPointerClickHandler
{
    [Header("Book Data & States")]
    [field: SerializeField] public bool IsBlocked {get; private set;} = false;
    [field: SerializeField] public bool IsMoving {get; set;} = false;
    [field: SerializeField] public BookType Type { get; private set; }
    
    [Header("References")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Blocked Visual")]
    [Tooltip("Sprite riêng dùng làm lớp phủ tối màu khi sách bị che. " +
             "Để trống thì code sẽ tự tạo overlay dùng chung silhouette với sprite sách hiện tại.")]
    [SerializeField] private Sprite blockedOverlaySprite;
    [Tooltip("Độ đậm của lớp phủ tối màu (0 = trong suốt, 1 = đen kín).")]
    [Range(0f, 1f)]
    [SerializeField] private float blockedOverlayAlpha = 0.55f;

    // SpriteRenderer riêng, nằm ngay trên sprite chính, chỉ bật lên khi bị che.
    // Cách này không phụ thuộc vào material/shader có nhân màu (tint) hay không,
    // nên đáng tin cậy hơn việc đổi trực tiếp spriteRenderer.color.
    private SpriteRenderer blockedOverlayRenderer;

    // Cho phép các lớp khác đọc thông tin Component khi cần
    public BoxCollider2D BoxCollider => boxCollider;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    private BookController controller;

    private void Awake()
    {
        if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<BookController>();

        SetupBlockedOverlay();
    }

    // Tạo (hoặc cấu hình lại) 1 SpriteRenderer con làm lớp phủ tối màu, nằm đúng vị trí
    // và đúng kích thước sách, sortingOrder cao hơn sprite chính 1 bậc để luôn nổi lên trên.
    // Mặc định overlay bị tắt (enabled = false); chỉ bật khi Book đang IsBlocked.
    private void SetupBlockedOverlay()
    {
        GameObject overlayObj = new GameObject("BlockedOverlay");
        overlayObj.transform.SetParent(transform, worldPositionStays: false);
        overlayObj.transform.localPosition = Vector3.zero;
        overlayObj.transform.localRotation = Quaternion.identity;
        overlayObj.transform.localScale = Vector3.one;

        blockedOverlayRenderer = overlayObj.AddComponent<SpriteRenderer>();

        // Nếu người thiết kế có gán sẵn 1 ảnh tối màu riêng (blockedOverlaySprite) thì dùng ảnh đó.
        // Ngược lại, tự dùng LẠI đúng sprite hiện tại của sách để overlay có cùng silhouette,
        // rồi tô đen bán trong suốt đè lên -> không cần đọc pixel của texture gốc (an toàn dù
        // texture có bật Read/Write hay không).
        blockedOverlayRenderer.sprite = blockedOverlaySprite != null
            ? blockedOverlaySprite
            : (spriteRenderer != null ? spriteRenderer.sprite : null);

        if(spriteRenderer != null)
        {
            blockedOverlayRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            blockedOverlayRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        }

        blockedOverlayRenderer.color = new Color(0f, 0f, 0f, blockedOverlayAlpha);
        blockedOverlayRenderer.enabled = false;
    }

    private void Start()
    {
        // Đăng ký với GameManager để được tính vào allBooks (dùng cho check bị che)
        // và đếm tổng số sách cần xếp xong để xác định điều kiện thắng level
        GameManager.instance?.RegisterBook(this);
    }

    // Gọi ngay sau khi Instantiate để gán dữ liệu cho sách được spawn động từ LevelData
    public void Initialize(BookType type, int sortingOrder)
    {
        Type = type; //private set nên chỉ gán được từ trong chính class Book

        if(spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }

        // sortingOrder thật sự chỉ được biết chính xác ở đây (Awake chạy trước, lúc đó
        // sortingOrder còn là giá trị mặc định) nên phải đồng bộ lại overlay tại đây.
        if(blockedOverlayRenderer != null)
        {
            blockedOverlayRenderer.sortingOrder = sortingOrder + 1;
        }
    }

    public void UpdateBlockedState()
    {
        controller.UpdateBlockedState();
    }

    public void SetBlockedState(bool value)
    {
        if(IsBlocked == value) return;

        IsBlocked = value;

        if(blockedOverlayRenderer != null)
        {
            // Luôn đồng bộ sortingOrder trước khi bật, phòng trường hợp sortingOrder của
            // sprite chính đã bị đổi ở đâu đó (ví dụ lúc đang bay tạm nâng lên 1000).
            blockedOverlayRenderer.sortingOrder = (spriteRenderer != null ? spriteRenderer.sortingOrder : 0) + 1;
            blockedOverlayRenderer.enabled = IsBlocked;
        }
    }

    public void MoveToPosition(Vector3 targetPosition, Action onComplete)
    {
        controller.RequestMove(targetPosition, onComplete);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.OnBookClicked();
    }
}