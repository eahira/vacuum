using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance { get; private set; }

    [SerializeField] private float moveSpeed = 10f;

    private Vector3 targetPosition;

    private void Awake()
    {
        Instance = this;
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    public void MoveToRoom(Vector3 roomCenter)
    {
        targetPosition = new Vector3(
            roomCenter.x,
            roomCenter.y,
            transform.position.z
        );
    }
}