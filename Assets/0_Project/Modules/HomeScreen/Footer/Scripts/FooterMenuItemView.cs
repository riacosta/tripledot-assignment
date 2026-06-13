using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FooterMenuItemView : MonoBehaviour, IPointerClickHandler
{
    public FooterMenuItem Item { get; private set; }
    public event Action<FooterMenuItem> Clicked;

    [SerializeField] private Image icon;
    [SerializeField] private LocalizedLabel label;
    [SerializeField] private LayoutElement layoutElement;

    private FooterMenuConfig config;
    private TextMeshProUGUI labelText;
    private float initialTextOffsetY;


    void Start()
    {
        labelText = label.GetComponentInChildren<TextMeshProUGUI>();
        initialTextOffsetY = label.transform.localPosition.y;

        if(Item != null)
        {
            SetSelected(Item.IsSelected, true);
        }
    }

    public void Initialize(FooterMenuConfig config)
    {
        this.config = config;
     
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(Item);
    }

    public void SetItem(FooterMenuItem item)
    {
        this.Item = item;
        icon.sprite = item.IsLocked ? config.LockedIcon : item.Icon;
        label.LocalizationKey = item.LocalizationKey;
           SetSelected(item.IsSelected, true);
    }

    public void SetSelected(bool isSelected,bool instant = false)
    {
        float animationTime = instant ? 0f : config.SelectionAnimationDuration;
        float targetSize = isSelected ? config.SelectedItemSize : config.DeselectedItemSize;
        Color targetColor = isSelected ? Color.white : config.HiddenItemColor;
        float targetTextOffsetY = isSelected ? initialTextOffsetY : initialTextOffsetY - 100;
        float targetIconScale = isSelected ? config.SelectedItemIconScale : 1f;
        float targetIconMoveY = isSelected ? config.SelectedItemIconMoveY : 0f;

        //Animates the element's width
        DOTween.To(
            () => layoutElement.preferredWidth,
            width =>
            {
                layoutElement.preferredWidth = width;
            },
            targetSize,
            animationTime
            ).SetEase(config.SelectionAnimationEase);

        //Text fader
        labelText.DOColor(
            targetColor,
            animationTime
            ).SetEase(config.SelectionAnimationEase);

        //Text move
        label.transform.DOLocalMoveY(
            targetTextOffsetY,
            animationTime)
            .SetEase(config.SelectionAnimationEase);

        //Icon scale
        icon.transform.DOScale(
            targetIconScale,
             animationTime)
             .SetEase(config.SelectionAnimationEase);

        //Icon move
        icon.transform.DOLocalMoveY(
            targetIconMoveY,
            animationTime)
            .SetEase(config.SelectionAnimationEase);
    }




}
