using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private Camera _camera;

    [Header("Scaling")]
    public float scaleFactor = 0.2f;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!enabled) return;

        transform.LookAt(
            transform.position + _camera.transform.rotation * Vector3.back,
            _camera.transform.rotation * Vector3.up
        );

        float distance = Vector3.Distance(transform.position, _camera.transform.position);

        transform.localScale = Vector3.one * distance * scaleFactor;
    }
}
