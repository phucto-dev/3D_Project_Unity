using System;
using UnityEngine;

public class MenuBtn : MonoBehaviour
{
    public MenuBtnList Btn;
    public event Action<MenuBtnList> BtnClicked;
    public void OnClick()
    {
        BtnClicked?.Invoke(Btn);
    }
}
