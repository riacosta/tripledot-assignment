using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FooterMenuConfig", menuName = "Scriptable Objects/FooterMenuConfig")]
public class FooterMenuConfig : ScriptableObject
{
    public List<FooterMenuItem> MenuItems;
    public Sprite LockedIcon;
}
