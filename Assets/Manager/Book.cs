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

    // Cho phép các lớp khác đọc thông tin Component khi cần
    public BoxCollider2D BoxCollider => boxCollider;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    private BookController controller;

    private void Awake()
    {
        if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<BookController>();
    }

    public void UpdateBlockedState()
    {
        controller.UpdateBlockedState();
    }

    public void SetBlockedState(bool value)
    {
        IsBlocked = value;
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