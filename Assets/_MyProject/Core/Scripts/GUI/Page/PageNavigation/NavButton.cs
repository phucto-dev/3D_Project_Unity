using Unity.VisualScripting;
using UnityEngine;

public class NavButton : MonoBehaviour
{
    [SerializeField] private MenuPageType targetPage;

    public void OnClick()
    {
        MainMenuController.Instance.OpenPage(targetPage);
    }
}
