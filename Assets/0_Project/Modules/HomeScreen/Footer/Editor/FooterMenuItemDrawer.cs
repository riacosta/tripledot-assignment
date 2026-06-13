#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FooterMenuItem))]
public class FooterMenuItemDrawer : PropertyDrawer
{
    private const float Padding = 10f;
    private const float SpritePreviewSize = 60f;
    private const float SpacingAfterLabel = 8f;
    private const float ColumnGap = 10f;
    private const float FieldSpacing = 5f;
    private const float PreviewBorderOffset = 2f;
    private const float PreviewLabelVerticalOffset = 8f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Main card background
        Rect cardRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
        GUI.Box(cardRect, "", new GUIStyle(GUI.skin.box));

        // Inner content area
        Rect contentRect = new Rect(cardRect.x + Padding, cardRect.y + Padding, cardRect.width - Padding * 2, cardRect.height - Padding * 2);
        float yOffset = contentRect.y;

        // Localization Key
        Rect locKeyRect = new Rect(contentRect.x, yOffset, contentRect.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty locKeyProp = property.FindPropertyRelative("LocalizationKey");
        EditorGUI.PropertyField(locKeyRect, locKeyProp);
        yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + SpacingAfterLabel;

        // Sprite preview + fields row
        float previewWidth = SpritePreviewSize;
        float rightColumnWidth = contentRect.width - previewWidth - ColumnGap;

        // Sprite preview (left side)
        Rect previewRect = new Rect(contentRect.x, yOffset, previewWidth, SpritePreviewSize);
        SerializedProperty iconProp = property.FindPropertyRelative("Icon");
        DrawSpritePreview(previewRect, iconProp.objectReferenceValue as Sprite);

        // Right column (Icon field + Flags)
        float rightColumnX = contentRect.x + previewWidth + ColumnGap;
        
        // Icon field (right side, top)
        Rect iconFieldRect = new Rect(rightColumnX, yOffset, rightColumnWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(iconFieldRect, iconProp, new GUIContent("Icon"));

        // Locked flag
        Rect lockedRect = new Rect(rightColumnX, yOffset + EditorGUIUtility.singleLineHeight + FieldSpacing, rightColumnWidth, EditorGUIUtility.singleLineHeight);
        SerializedProperty lockedProp = property.FindPropertyRelative("IsLocked");
        EditorGUI.PropertyField(lockedRect, lockedProp, new GUIContent("Locked"));

        // Selected flag
        Rect selectedRect = new Rect(rightColumnX, yOffset + EditorGUIUtility.singleLineHeight * 2 + FieldSpacing * 2, rightColumnWidth, EditorGUIUtility.singleLineHeight);
        SerializedProperty selectedProp = property.FindPropertyRelative("IsSelected");
        EditorGUI.PropertyField(selectedRect, selectedProp, new GUIContent("Selected"));

        EditorGUI.EndProperty();
    }

    private void DrawSpritePreview(Rect rect, Sprite sprite)
    {
        GUI.Box(rect, "", EditorStyles.helpBox);

        if (sprite != null)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(sprite);
            if (preview != null)
            {
                GUI.DrawTexture(
                    new Rect(rect.x + PreviewBorderOffset, rect.y + PreviewBorderOffset, rect.width - PreviewBorderOffset * 2, rect.height - PreviewBorderOffset * 2),
                    preview,
                    ScaleMode.ScaleToFit
                );
            }
        }
        else
        {
            GUI.Label(
                new Rect(rect.x + Padding, rect.y + rect.height / 2 - PreviewLabelVerticalOffset, rect.width - Padding * 2, EditorGUIUtility.singleLineHeight),
                "No sprite",
                EditorStyles.helpBox
            );
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return SpritePreviewSize + Padding * 2 + EditorGUIUtility.standardVerticalSpacing + SpacingAfterLabel + EditorGUIUtility.singleLineHeight;
    }
}
#endif
