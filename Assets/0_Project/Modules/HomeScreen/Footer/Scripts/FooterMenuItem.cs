
using System;
using UnityEngine;

[Serializable]
public class FooterMenuItem 
{
    public string LocalizationKey;
    public Sprite Icon;
    public bool IsSelected;
    public bool IsLocked;


    public FooterMenuItem(FooterMenuItem other)
    {
        Icon = other.Icon;
        IsLocked = other.IsLocked;
        IsSelected = other.IsSelected;
        LocalizationKey = other.LocalizationKey;
    }
}
