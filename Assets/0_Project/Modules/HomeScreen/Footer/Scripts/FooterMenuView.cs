using System;
using System.Collections.Generic;
using UnityEngine;

public class FooterMenuView : MonoBehaviour
{
    [Header("Configuration")]

    [SerializeField] private GameObject ItemsContainer;
    [SerializeField] private GameObject itemPrefab;

    private FooterMenuConfig config;
    private List<FooterMenuItemView> itemViews = new List<FooterMenuItemView>();
    private FooterMenuPresenter presenter;

    public void Initialize(FooterMenuPresenter presenter)
    {
        this.presenter = presenter;
    }

    public void PopulateItems(List<FooterMenuItem> items)
    {
        foreach (Transform child in ItemsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            var itemView = Instantiate(itemPrefab, ItemsContainer.transform).GetComponent<FooterMenuItemView>();
            
            itemView.Initialize(config);
            itemView.SetItem(item);
            itemView.Clicked += OnItemClicked;
            itemViews.Add(itemView);
        }
    }

    public void SetItemSelected(FooterMenuItem item)
    {
        foreach (var itemView in itemViews)
        {
            itemView.SetSelected(itemView.Item == presenter.SelectedItem);
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
