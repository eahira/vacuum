using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    [Header("Room size in tiles")]
    [SerializeField] private int widthInTiles = 16;
    [SerializeField] private int heightInTiles = 9;

    private BoxCollider2D box;

    private void OnValidate()
    {
        SetupCollider();
    }

    private void Awake()
    {
        SetupCollider();
    }

    private void SetupCollider()
    {
        box = GetComponent<BoxCollider2D>();

        box.isTrigger = true;
        box.size = new Vector2(widthInTiles, heightInTiles);
        box.offset = new Vector2(widthInTiles / 2f, heightInTiles / 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        RoomCameraController.Instance.MoveToRoom(box.bounds.center);
    }

    private void OnDrawGizmos()
    {
        if (box == null)
            box = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    }
}