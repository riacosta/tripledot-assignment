using UnityEngine;

public class FooterMenuPresenter
{
    public FooterMenuItem SelectedItem { get; private set; }


    private FooterMenuView view;
    private FooterMenuConfig config;


    public void Initialize(FooterMenuView view, FooterMenuConfig config)
    {
        this.view = view;
        this.config = config;
        view.Initialize(this);
        view.PopulateItems(config.MenuItems);
    }

    public void OnItemSelected(FooterMenuItem item)
    {
        if (!item.IsLocked)
        {
            SelectedItem = item;
            view.SetItemSelected(item);
        }

    }
}
