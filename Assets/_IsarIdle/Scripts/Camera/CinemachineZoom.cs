using UnityEngine;
using Unity.Cinemachine;

public class CinemachineZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private CinemachinePositionComposer composer;

    private void Reset()
    {
        vcam = GetComponent<CinemachineCamera>();
        composer = GetComponent<CinemachinePositionComposer>();
    }

    private void Update()
    {
        if (composer == null) return;

        float scrollInput = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            float currentDistance = composer.CameraDistance;
            float newDistance = Mathf.Clamp(currentDistance - scrollInput * zoomSpeed, minZoom, maxZoom);
            composer.CameraDistance = newDistance;
        }
    }
}