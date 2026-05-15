using System.Collections.Generic;
using UnityEngine;

public enum MenuPageType
{
    Character,
    Combat
}
public class MainMenuController : MonoBehaviour
{
    [SerializeField] List<MenuPage> pages;

    private MenuPage _currentPage;

    private void Start()
    {
        foreach (MenuPage page in pages)
        {
            if (page.PageType == MenuPageType.Character)
            {
                _currentPage = page;
                OpenPage(_currentPage.PageType);
            }
        }
    }

    public void OpenPage(MenuPageType pageType)
    {
        foreach (MenuPage page in pages)
        {
            bool isTarget = page.PageType == pageType;

            page.gameObject.SetActive(isTarget);

            if (isTarget)
            {
                _currentPage = page;
            }
        }
    }
}
