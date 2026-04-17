using UnityEngine;
using UnityEngine.InputSystem;

public class TestShaderChangeByButton : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color color2Change;
    [SerializeField] private float duration = 0.2f;

    private Material mat;
    private bool _isChanged = false;
    private Color _baseColor;
    private float _lastTime;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            mat = meshRenderer.material;
            _baseColor = Color.red;
        }
    }

    private void Update()
    {
        if (Keyboard.current.cKey.isPressed)
        {
            if (Time.time - _lastTime < duration) return;
            if (_isChanged)
            {
                ChangeColor(color2Change);
            }
            else
            {
                ChangeColor(_baseColor);
            }
        }
    }

    private void ChangeColor(Color color)
    {
        mat.SetColor("_BaseColor", color);
        _isChanged = !_isChanged;
        _lastTime = Time.time;
    }
}
