using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Graphic))]
public sealed class UIShaderAnimator : MonoBehaviour, IMaterialModifier
{
    [Header("Shader property")]

    [Tooltip("Shader Graph Reference name of the property to animate")]
    [SerializeField]
    private string propertyName = "";

    [Tooltip("Value animated by the Animation or Animator component")]
    [SerializeField]
    private float value;

    private Graphic graphic;

    private Material materialInstance;
    private Material currentBaseMaterial;

    private string cachedPropertyName;
    private int propertyId;

    private bool propertyIsValid;
    private bool warningLogged;

    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            ApplyAndRefresh();
        }
    }

    private void OnEnable()
    {
        graphic = GetComponent<Graphic>();

        CacheProperty();
        graphic.SetMaterialDirty();
    }

    private void OnDisable()
    {
        ReleaseMaterial();

        if (graphic != null)
            graphic.SetMaterialDirty();
    }

    private void OnDestroy()
    {
        ReleaseMaterial();
    }

    private void OnValidate()
    {
        if (graphic == null)
            graphic = GetComponent<Graphic>();

        CacheProperty();
        ApplyAndRefresh();
    }

    private void OnDidApplyAnimationProperties()
    {
        ApplyAndRefresh();
    }

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        if (!isActiveAndEnabled || baseMaterial == null)
            return baseMaterial;

        CacheProperty();

        bool needsNewInstance =
            materialInstance == null ||
            currentBaseMaterial != baseMaterial ||
            materialInstance.shader != baseMaterial.shader;

        if (needsNewInstance)
        {
            ReleaseMaterial();

            currentBaseMaterial = baseMaterial;

            materialInstance = new Material(baseMaterial)
            {
                name = $"{baseMaterial.name} ({name} ShaderAnimator)",
                hideFlags = HideFlags.HideAndDontSave
            };

            propertyIsValid = false;
            warningLogged = false;
        }
        else
        {
         
             // Keep any changes made by other UI material modifiers,          
            materialInstance.CopyPropertiesFromMaterial(baseMaterial);
        }

        ApplyToMaterial(materialInstance);

        return materialInstance;
    }

    private void ApplyAndRefresh()
    {
        CacheProperty();

        if (materialInstance != null)
            ApplyToMaterial(materialInstance);

        if (graphic != null)
            graphic.SetMaterialDirty();
    }

    private void ApplyToMaterial(Material material)
    {
        if (!ValidateProperty(material))
            return;

        material.SetFloat(propertyId, value);
    }

    private void CacheProperty()
    {
        if (cachedPropertyName == propertyName)
            return;

        cachedPropertyName = propertyName;

        propertyId = string.IsNullOrWhiteSpace(propertyName)
            ? 0
            : Shader.PropertyToID(propertyName);

        propertyIsValid = false;
        warningLogged = false;
    }

    private bool ValidateProperty(Material material)
    {
        if (propertyIsValid)
            return true;

        if (material == null || material.shader == null)
            return false;

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            LogWarningOnce("Shader property name is empty.");
            return false;
        }

        Shader shader = material.shader;
        int propertyIndex = shader.FindPropertyIndex(propertyName);

        if (propertyIndex < 0)
        {
            LogWarningOnce(
                $"Shader '{shader.name}' does not contain " +
                $"a property named '{propertyName}'.");

            return false;
        }

        ShaderPropertyType propertyType =
            shader.GetPropertyType(propertyIndex);

        if (propertyType != ShaderPropertyType.Float &&
            propertyType != ShaderPropertyType.Range)
        {
            LogWarningOnce(
                $"Property '{propertyName}' is of type " +
                $"{propertyType}, not Float or Range.");

            return false;
        }

        propertyIsValid = true;
        return true;
    }

    private void LogWarningOnce(string message)
    {
        if (warningLogged)
            return;

        warningLogged = true;

        Debug.LogWarning(
            $"{nameof(UIShaderAnimator)} on '{name}': {message}",
            this);
    }

    private void ReleaseMaterial()
    {
        if (materialInstance == null)
            return;

        if (Application.isPlaying)
            Destroy(materialInstance);
        else
            DestroyImmediate(materialInstance);

        materialInstance = null;
        currentBaseMaterial = null;
        propertyIsValid = false;
    }
}