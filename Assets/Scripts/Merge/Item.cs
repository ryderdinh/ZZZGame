using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    [Tooltip("Size of the item in cells (1 or more).")] [SerializeField]
    private Vector2Int itemSize = new(1, 1);

    [Tooltip("Reference to the MapGame this item interacts with.")] [SerializeField]
    private MapGame mapGame;

    [SerializeField] private int forceId;

    public Vector3 initialPosition;
    private int _id;

    private bool isDragging;
    private Vector3 offset;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        if (forceId != 0)
            _id = forceId;
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        initialPosition = transform.position;
        isDragging = true;
    }


    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + offset;
        mapGame.HandleOnDragItem(isDragging, this, itemSize);
    }


    private void OnMouseUp()
    {
        if (mapGame == null) return;

        isDragging = false;
        mapGame.HandleOnDragItem(isDragging, this, itemSize);
    }


    private Vector3 GetMouseWorldPosition()
    {
        var mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private int GetId()
    {
        return _id;
    }
}