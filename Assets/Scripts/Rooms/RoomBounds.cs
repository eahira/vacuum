using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
public class RoomBounds : MonoBehaviour
{
    [Header("Room size in tiles")]
    [SerializeField] private int widthInTiles = 16;
    [SerializeField] private int heightInTiles = 10;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null) return;

        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}