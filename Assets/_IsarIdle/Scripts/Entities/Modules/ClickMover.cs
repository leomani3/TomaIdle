using UnityEngine;

public class ClickMover : AEntityModule
{
    [SerializeField] private LayerMask m_groundLayer;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray _ray = CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out RaycastHit _raycastHit, 100, m_groundLayer))
            {
                if (m_linkedEntity.TryGetModule(out MovementEM _movementEM))
                {
                    _movementEM.SetDestination(_raycastHit.point);
                }
            }
        }
    }
}