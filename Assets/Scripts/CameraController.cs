using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private float scrollSpeed = 10.0f;
    private int floorCount;
    private float minY;

    private void Start()
    {
        SetMinY(2);
    }

    private void Update()
    {
        ScrollControl();
    }

    // Camera scrolls vertical with the mouse scroll wheel 
    // while the cameras position get's clamped to the level
    private void ScrollControl()
    {
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");

        if (scrollAxis < 0 && transform.position.y > minY)
            transform.Translate(Vector3.up * -scrollSpeed * Time.deltaTime);
        else if (scrollAxis > 0 && transform.position.y < 1)
            transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }

    private void OnEnable()
    {
        LevelManager.OnFloorAdded += SetMinY;
    }

    private void OnDisable()
    {
        LevelManager.OnFloorAdded -= SetMinY;
    }

    // To not calculate minY every frame, minY gets updated whenever OnFloorAdded fires
    private void SetMinY(int floorCount)
    {
        minY = (floorCount - 1) * -2 + 2.2f;
    }
}
