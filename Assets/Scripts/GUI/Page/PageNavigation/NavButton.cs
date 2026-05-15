using Unity.VisualScripting;
using UnityEngine;

public class NavButton : MonoBehaviour
{
    [SerializeField] private MenuPageType targetPage;
    private MainMenuController menuController;

    private void Start()
    {
        menuController = transform.root.GetComponentInChildren<MainMenuController>();
    }
    public void OnClick()
    {
        menuController.OpenPage(targetPage);
    }
}
