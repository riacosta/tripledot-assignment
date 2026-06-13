using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FooterMenuView : MonoBehaviour
{
    [Header("Configuration")]

    [SerializeField] private GameObject ItemsContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Image selectionIndicator;

    private FooterMenuConfig config;
    private List<FooterMenuItemView> itemViews = new List<FooterMenuItemView>();
    private FooterMenuPresenter presenter;



    public void Initialize(FooterMenuPresenter presenter, FooterMenuConfig config)
    {
        this.presenter = presenter;
        this.config = config;

        selectionIndicator.color = config.HiddenItemColor;

    }

    public void PopulateItems(List<FooterMenuItem> items)
    {
        foreach (Transform child in ItemsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        FooterMenuItem selectedItem = null;
        foreach (var item in items)
        {
            var itemView = Instantiate(itemPrefab, ItemsContainer.transform).GetComponent<FooterMenuItemView>();

            itemView.Initialize(config);
            itemView.SetItem(item);
            itemView.Clicked += OnItemClicked;
            itemViews.Add(itemView);
            if (item.IsSelected)
            {
                selectedItem = item;
            }
        }

        SetItemSelected(selectedItem);

    }

    public void SetItemSelected(FooterMenuItem item)
    {
        FooterMenuItemView selectedItemView = null;
        foreach (var itemView in itemViews)
        {
            bool isSelected = itemView.Item == item;
            itemView.SetSelected(isSelected);
            if (isSelected)
            {
                selectedItemView = itemView;

            }

        }
        SetSelectionIndicator(selectedItemView);
    }
    
    private void SetSelectionIndicator(FooterMenuItemView selectedItemView, bool instant = false)
    {
        float animationTime = instant ? 0f : config.SelectionAnimationDuration;
        RectTransform selectionIndicatorRect = selectionIndicator.transform as RectTransform;
       
        if (selectedItemView != null)
        {

            //Follows the selected item with a smooth animation
            float initialX = selectionIndicatorRect.anchoredPosition.x;
            Tween updateTween = DOVirtual.Float(0f, 1f,
                animationTime,
               value =>
               {
                   float selectedItemViewAnchorPosX = ((RectTransform)selectedItemView.transform).anchoredPosition.x;
                   selectionIndicatorRect.anchoredPosition =
                    Vector2.Lerp(
                         new Vector2(initialX, selectionIndicatorRect.anchoredPosition.y),
                         new Vector2(selectedItemViewAnchorPosX, selectionIndicatorRect.anchoredPosition.y),
                         value
                     );
               }
                ).SetEase(config.SelectionAnimationEase);

            //Resizes the selection indicator to match the selected item.
            selectionIndicatorRect.DOSizeDelta(
                           new Vector2(config.SelectedItemSize,
                           selectionIndicator.GetComponent<RectTransform>().sizeDelta.y),
                           animationTime
                           ).SetEase(config.SelectionAnimationEase);
            
            //Moves the indicator up and down with the selected item
            selectionIndicatorRect.DOAnchorPosY(
                            config.SelectionIndicatorHeight,
                            animationTime
                            ).SetEase(config.SelectionAnimationEase);
            
            //Fades in the selection indicator
            selectionIndicator.DOColor(
                            Color.white,
                            animationTime)
                            .SetEase(config.SelectionAnimationEase);


        }
        else
        {
            selectionIndicatorRect.DOSizeDelta(
                           new Vector2(config.DeselectedItemSize,
                           selectionIndicatorRect.sizeDelta.y),
                           animationTime
                           ).SetEase(config.SelectionAnimationEase);

            selectionIndicatorRect.DOAnchorPosY(
                              -selectionIndicatorRect.sizeDelta.y,
                            animationTime
                            ).SetEase(config.SelectionAnimationEase);


            selectionIndicator.DOColor(
                           config.HiddenItemColor,
                            animationTime
                            ).SetEase(config.SelectionAnimationEase);
        }

    }

    private void OnItemClicked(FooterMenuItem item)
    {
        presenter.OnItemSelected(item);
    }

    private void OnDestroy()
    {
        foreach (var itemView in itemViews)
        {
            itemView.Clicked -= OnItemClicked;
        }
    }
}
