# Deep-Dive Refactor

## Selected Feature: Footer Menu

I selected the **Footer Menu** for the hands-on refactor because it is one of the main interaction points in the UI and contains several of the issues identified during the audit.

The original implementation uses two main scripts:

* `MenuFooterController`
* `ButtonFooterController`

The new implementation uses:

* `FooterMenuPresenter`
* `FooterMenuView`
* `FooterMenuItemView`
* `FooterMenuItem`
* `FooterMenuConfig`

## Problems in the Original Implementation

The original footer works, but its implementation mixes state management, animations and direct UI references.

`MenuFooterController` is responsible for:

* Tracking the selected button
* Selecting and deselecting every item
* Showing and hiding the indicator
* Moving the indicator
* Listening to button events

Each `ButtonFooterController` also controls its own `Animator` using the `Selected` and `Locked` parameters.

At the same time, the menu controller uses DOTween to move the indicator. This means the footer relies on two different animation systems controlling related visual states. This makes the component harder to manage and is likely the cause of the flickering seen when changing selection.

The buttons must also be manually added to a list in the Inspector:

```csharp
[SerializeField]
private ButtonFooterController startSelected;

[SerializeField]
private List<ButtonFooterController> footerButtons;
```

Adding, removing or reordering a menu item requires changing the scene hierarchy and manually updating this list.

The indicator animation also contains hardcoded values:

```csharp
indicator.transform
    .DOMoveX(_currentSlot.transform.position.x, 0.25f)
    .SetEase(Ease.OutSine);
```

The duration and easing cannot be adjusted from the Inspector without editing the code.

For a single static footer, a controller combined with item views would probably be sufficient. I chose a more data-driven implementation because the brief emphasises extensibility, localisation and reuse across future screens. Without those requirements, I would favour the smaller implementation.

## Refactor Goals

The main goals of the refactor were:

* Separate menu state from visual behaviour
* Use DOTween as the only animation system for the footer
* Generate menu items from data
* Move animation values into a shared configuration
* Improve localisation support
* Make the footer easier to extend and reuse

## New Structure

### `FooterMenuItem`

`FooterMenuItem` contains the configuration and runtime state of each menu option:

```csharp
[Serializable]
public class FooterMenuItem
{
    public string LocalizationKey;
    public Sprite Icon;
    public bool IsSelected;
    public bool IsLocked;
}
```

The item no longer depends on a specific `GameObject` or button component.

It also includes a copy constructor. This allows the presenter to create runtime copies of the configured items instead of modifying the original `ScriptableObject` asset.

### `FooterMenuConfig`

`FooterMenuConfig` is a `ScriptableObject` containing the menu items and shared visual settings.

It includes:

* Menu icons
* Locked icon
* Selected and deselected sizes
* Animation duration
* Animation easing
* Icon scale and position
* Indicator position

This removes most hardcoded visual values from the scripts and allows artists or designers to adjust the footer directly from the Inspector.

It also allows the same footer implementation to be reused with different configurations.

### `FooterMenuPresenter`

The presenter handles the selection logic:

```csharp
public void OnItemSelected(FooterMenuItem item)
{
    if (item.IsSelected || item.IsLocked)
    {
        SelectedItem = null;
        item.IsSelected = false;
    }
    else
    {
        foreach (var menuItem in menuItems)
        {
            menuItem.IsSelected = item == menuItem;
        }

        SelectedItem = item;
    }

    view.SetItemSelected(SelectedItem);
}
```

The presenter contains no animation code and has no direct references to individual UI components.

Its responsibility is limited to deciding which item is selected and asking the view to refresh.

This makes the selection logic easier to understand, maintain and test.

### `FooterMenuView`

`FooterMenuView` creates the menu item views from the provided configuration:

```csharp
foreach (var item in items)
{
    var itemView = Instantiate(
        itemPrefab,
        ItemsContainer.transform
    ).GetComponent<FooterMenuItemView>();

    itemView.Initialize(config);
    itemView.SetItem(item);
    itemView.Clicked += OnItemClicked;
}
```

The footer no longer requires a manually maintained list of button references.

Items can be added, removed or reordered in the `ScriptableObject` without changing the menu code or manually reconnecting scene references.

The view also controls the shared selection indicator. It uses `RectTransform.anchoredPosition` instead of world position, which is safer and more predictable for responsive UI layouts.

### `FooterMenuItemView`

Each item view is responsible for:

* Displaying the correct icon
* Applying the localisation key
* Detecting clicks
* Animating its selected and unselected states

```csharp
public void SetItem(FooterMenuItem item)
{
    Item = item;

    icon.sprite = item.IsLocked
        ? config.LockedIcon
        : item.Icon;

    label.LocalizationKey = item.LocalizationKey;

    SetSelected(item.IsSelected, true);
}
```

The item view sends the selected data object to the presenter instead of passing a reference to the button component itself.

## Before and After

### Before

The original controller directly compares scene components and changes their state:

```csharp
_buttonSelected = buttonClicked;

foreach (var btn in footerButtons)
{
    btn.SetSelect(_buttonSelected == btn);
}

MoveIndicator();
```

### After

The presenter updates the menu state first:

```csharp
foreach (var menuItem in menuItems)
{
    menuItem.IsSelected = item == menuItem;
}

SelectedItem = item;

view.SetItemSelected(SelectedItem);
```

The view then updates the visuals based on that state.

This creates a clearer separation of responsibilities:

| Component             | Responsibility                              |
| --------------------- | ------------------------------------------- |
| `FooterMenuPresenter` | Selection logic                             |
| `FooterMenuView`      | Menu creation and rendering                 |
| `FooterMenuItemView`  | Individual item presentation and animations |
| `FooterMenuConfig`    | Editable visual and animation settings      |
| `FooterMenuItem`      | Menu data and runtime state                 |

## Main Improvements

The refactored version improves the footer in several areas:

* `Animator` and DOTween no longer control related states in the same component.
* Menu items are generated from configuration data.
* Animation values are exposed outside the scripts.
* Selection logic is separated from visual behaviour.
* Localisation keys are included in the menu data.
* Runtime state does not modify the original `ScriptableObject` asset.
* The component is easier to reuse, maintain and extend.
* Responsive positioning uses UI-local coordinates instead of world-space coordinates.

## Result

The refactor changes the footer from a scene-driven implementation into a data-driven UI component.

The new structure is easier to maintain because state, presentation and configuration have clearer responsibilities.

It also resolves the main animation ownership problem by keeping all footer transitions under DOTween instead of combining DOTween with `Animator`.
