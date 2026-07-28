using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform _mainCameraTransform;
    private void LateUpdate()
    {
        if (_mainCameraTransform == null || !_mainCameraTransform.gameObject.activeInHierarchy)
        {
            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }
            else
            {
                return;
            }
        }
        transform.rotation = _mainCameraTransform.rotation;
    }
}
