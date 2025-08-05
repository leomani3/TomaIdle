using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private CinemachineCamera m_cinemachineCamera;
    [SerializeField] private float m_scrollSpeed = 10f;
    [SerializeField] private Vector2 m_minMaxScroll;
    
    public Camera Camera => m_camera;
    public CinemachineCamera CinemachineCamera => m_cinemachineCamera;

    public void SetTarget(Transform _target)
    {
        m_cinemachineCamera.Follow = _target;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            m_cinemachineCamera.Lens.OrthographicSize -= Input.mouseScrollDelta.y * m_scrollSpeed * Time.deltaTime;
            m_cinemachineCamera.Lens.OrthographicSize = Mathf.Clamp(m_cinemachineCamera.Lens.OrthographicSize, m_minMaxScroll.x, m_minMaxScroll.y);
        }
    }
}