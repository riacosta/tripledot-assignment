using System.Collections.Generic;
using UnityEngine;

public class FooterMenuPresenter
{
    public FooterMenuItem SelectedItem { get; private set; }

    private List<FooterMenuItem> menuItems;
    private FooterMenuView view;


    public void Initialize(FooterMenuView view, FooterMenuConfig config)
    {
        this.view = view;
        view.Initialize(this, config);
        this.menuItems = config.MenuItems.ConvertAll(item => new FooterMenuItem(item));
        view.PopulateItems(menuItems);
    }

    public void OnItemSelected(FooterMenuItem item)
    {

        //if the item is already selected or locked, deselect it.
        if (item.IsSelected || item.IsLocked)
        {
            SelectedItem = null;
            item.IsSelected = false;
        }

        //if the item is not locked, select it and deselect all other items.
       else if (!item.IsLocked)
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.IsSelected = item == menuItem;
            }

            SelectedItem = item;
        }
        view.SetItemSelected(SelectedItem);
    }
}
