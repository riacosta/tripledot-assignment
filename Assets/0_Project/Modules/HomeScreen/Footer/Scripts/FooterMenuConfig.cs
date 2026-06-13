using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "FooterMenuConfig", menuName = "Scriptable Objects/FooterMenuConfig")]
public class FooterMenuConfig : ScriptableObject
{
    [Header("Menu Items")]
    public List<FooterMenuItem> MenuItems;
    public Sprite LockedIcon;
    

    [Header("Animation Settings")]
    public float SelectedItemSize = 200f;
    public float SelectedItemIconScale = 1.5f;
    public float SelectedItemIconMoveY =100f;
    public float DeselectedItemSize = 150f;
    public float SelectionAnimationDuration = 0.3f;
    public Ease SelectionAnimationEase = Ease.OutBack;
    public Color HiddenItemColor = new Color(1f, 1f, 1f, 0f);
    public float SelectionIndicatorHeight= -70f;
  

}
