using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FooterMenuItemView : MonoBehaviour, IPointerClickHandler
{
    public FooterMenuItem Item { get; private set; } 


    [SerializeField] private Image icon;
    [SerializeField] private LocalizedLabel label;
    private Sprite lockedIcon;

    public event Action<FooterMenuItem> Clicked;

public void Initialize(FooterMenuConfig config)
    {
        this.lockedIcon = config.LockedIcon;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(Item);
    }

    public void SetItem(FooterMenuItem item)
    {
        this.Item = item;
        icon.sprite = item.IsLocked ? lockedIcon : item.Icon;
        label.LocalizationKey = item.LocalizationKey;
    }

    public void SetSelected(bool isSelected)
    {
        icon.color = isSelected ? Color.green : Color.white; // Example: Change color based on selection
        //TODO: Update the visual state of the item based on selection
    }




}
