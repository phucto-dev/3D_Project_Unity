using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    private PlayerInput _inputSystem;
    private InputAction _pauseAction;

    // Biến theo dõi trạng thái hiện tại
    private bool _isCursorLocked = true;

    private void Awake()
    {
        // Lấy PlayerInput (Đảm bảo GameManager cũng có gắn PlayerInput, 
        // hoặc bạn kéo tham chiếu từ Player sang)
        _inputSystem = GetComponent<PlayerInput>();
        if (_inputSystem != null)
        {
            _pauseAction = _inputSystem.actions["ToggleMenu"];
        }
    }

    private void OnEnable()
    {
        if (_pauseAction != null)
            _pauseAction.performed += HandlePauseInput;
    }

    private void OnDisable()
    {
        if (_pauseAction != null)
            _pauseAction.performed -= HandlePauseInput;
    }

    private void Start()
    {
        // Vừa vào game là tự động khóa chuột ngay
        LockCursor();
    }

    private void HandlePauseInput(InputAction.CallbackContext ctx)
    {
        // Khi người chơi bấm ESC, đổi trạng thái
        ToggleCursor();
    }

    private void ToggleCursor()
    {
        _isCursorLocked = !_isCursorLocked;
        Debug.Log("Toggle neee " + _isCursorLocked);

        if (_isCursorLocked)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    private void LockCursor()
    {
        // Khóa chết chuột ở giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
        // Ẩn hình ảnh con trỏ đi
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        // Thả tự do cho chuột
        Cursor.lockState = CursorLockMode.None;
        // Hiện con trỏ lên lại để bấm UI
        Cursor.visible = true;
    }
}
