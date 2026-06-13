using TMPro;
using UnityEngine;

public class LocalizedLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textLabel;
    private string localizationKey;
    public string LocalizationKey
    {
        get { return localizationKey; }
        set { localizationKey = value; UpdateText(); }
    }

    private void Start()
    {
        UpdateText();
    }


    private void UpdateText()
    {
        if (textLabel != null && !string.IsNullOrEmpty(LocalizationKey))
        {
            textLabel.text = LocalizationKey; // Replace with actual localization lookup
        }
        else
        {
            Debug.LogWarning("Text label is not assigned or localization key is empty.");
        }
    }
}
