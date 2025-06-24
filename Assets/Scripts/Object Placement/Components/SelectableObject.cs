using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for objects that can be selected and visually highlighted.
/// </summary>
public class SelectableObject : MonoBehaviour
{
    /// <summary>
    /// The color to apply to the object when it is selected.
    /// </summary>
    [Tooltip("The color to apply to the object when it is selected.")]
    [SerializeField] private Color selectionColour = Color.yellow;
    /// <summary>
    /// The material template to use for highlighting the object upon selection.
    /// </summary>
    [Tooltip("The material template to use for highlighting the object upon selection.")]
    [SerializeField] private Material selectionMaterialTemplate;

    // A list to store all the renderers of this object and its children.
    private readonly List<Renderer> renderers = new();
    // A list to store the original materials of each renderer.
    private readonly List<Material[]> originalMaterials = new();
    // The instantiated material used for selection highlighting.
    private Material selectionMaterial;

    /// <summary>
    /// Event triggered when this object is selected.
    /// </summary>
    public event Action<SelectableObject> OnSelected;
    /// <summary>
    /// Event triggered when this object is deselected.
    /// </summary>
    public event Action<SelectableObject> OnDeselected;

    /// <summary>
    /// Initializes the renderer list, original materials, and creates the selection material.
    /// </summary>
    private void Start()
    {
        UpdateRenderers();
        CreateSelectionMaterial();
    }

    // Creates an instance of the selection material from the template.
    private void CreateSelectionMaterial()
    {
        if (selectionMaterialTemplate == null)
        {
            Debug.LogError("Selection material template is not assigned.");
            return;
        }

        selectionMaterial = new Material(selectionMaterialTemplate);
    }

    /// <summary>
    /// Selects the object, visually highlighting it and invoking the OnSelected event.
    /// </summary>
    public void Select()
    {
        SetColour(selectionColour);
        OnSelected?.Invoke(this);
    }

    /// <summary>
    /// Deselects the object, removing the highlight and invoking the OnDeselected event.
    /// </summary>
    public void Deselect()
    {
        ClearColour();
        OnDeselected?.Invoke(this);
    }

    /// <summary>
    /// Clears the selection highlight by restoring the object's original materials.
    /// </summary>
    public void ClearColour()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
    }

    /// <summary>
    /// Sets the color of the object's materials to the specified color, used for selection highlighting.
    /// </summary>
    /// <param name="colour">The color to apply to the object's materials.</param>
    public void SetColour(Color colour)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                Material[] selectionMaterials = new Material[renderer.materials.Length];
                selectionMaterial.color = colour;
                for (int i = 0; i < selectionMaterials.Length; i++)
                {
                    selectionMaterials[i] = selectionMaterial;
                }
                renderer.materials = selectionMaterials;
            }
        }
    }

    /// <summary>
    /// Sets the active state of the GameObject, deselecting it if it's being deactivated.
    /// </summary>
    /// <param name="active">The new active state of the GameObject.</param>
    public void SetActive(bool active)
    {
        if (!active)
        {
            Deselect();
        }
        gameObject.SetActive(active);
    }

    /// <summary>
    /// Updates the list of renderers and their original materials. Called during initialization and can be called manually if the object's children change.
    /// </summary>
    protected void UpdateRenderers()
    {
        renderers.Clear();
        originalMaterials.Clear();

        Renderer[] childRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in childRenderers)
        {
            if (renderer == null) continue;

            renderers.Add(renderer);
            originalMaterials.Add(renderer.sharedMaterials);
        }
    }

    // Destroys the selection material instance when this object is destroyed.
    private void OnDestroy()
    {
        if (selectionMaterial != null)
        {
            Destroy(selectionMaterial);
        }
    }
}