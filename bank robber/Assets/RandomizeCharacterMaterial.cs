using UnityEngine;

public class RandomizeCharacterMaterial : MonoBehaviour
{
    [Header("Material Generation")]
    [Tooltip("Material shader name. Will try fallback if not found.")]
    [SerializeField] private string shaderName = "Standard";

    [Tooltip("If true, tries URP Lit shader when Standard is missing.")]
    [SerializeField] private bool tryUrpLitFallback = true;

    [Tooltip("Random color range (HSV).")]
    [SerializeField] private Vector2 hueRange = new Vector2(0f, 1f);
    [SerializeField] private Vector2 saturationRange = new Vector2(0.5f, 1f);
    [SerializeField] private Vector2 valueRange = new Vector2(0.6f, 1f);

    [Header("Targets")]
    [Tooltip("If checked, apply to child renderers too")]
    [SerializeField] private bool includeChildren = true;

    [Tooltip("If checked, apply on Awake")]
    [SerializeField] private bool applyOnAwake = true;

    [Tooltip("If checked, include SkinnedMeshRenderer")]
    [SerializeField] private bool includeSkinnedMeshes = true;

    [Tooltip("Re-roll on R key (debug)")]
    [SerializeField] private bool debugReRollWithR = true;

    private Material chosen;

    private void Awake()
    {
        if (applyOnAwake) ApplyRandom();
    }

    private void Update()
    {
        //if (debugReRollWithR && Input.GetKeyDown(KeyCode.R))
            //ApplyRandom();
    }

    [ContextMenu("Apply Random Material")]
    public void ApplyRandom()
    {
        var shader = Shader.Find(shaderName);
        if (shader == null && tryUrpLitFallback)
            shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            Debug.LogWarning("[RandomizeCharacterMaterial] Shader not found", this);
            return;
        }

        CleanupGeneratedMaterial();

        chosen = new Material(shader)
        {
            hideFlags = HideFlags.DontSave
        };

        var color = Random.ColorHSV(
            hueRange.x, hueRange.y,
            saturationRange.x, saturationRange.y,
            valueRange.x, valueRange.y
        );

        // Try both common color property names.
        if (chosen.HasProperty("_BaseColor"))
            chosen.SetColor("_BaseColor", color);
        if (chosen.HasProperty("_Color"))
            chosen.SetColor("_Color", color);
        if (chosen.HasProperty("_Metallic"))
            chosen.SetFloat("_Metallic", 0f);
        if (chosen.HasProperty("_Smoothness"))
            chosen.SetFloat("_Smoothness", 1f);
        else if (chosen.HasProperty("_Glossiness"))
            chosen.SetFloat("_Glossiness", 1f);

        ApplyMaterialToAllRenderers(chosen);
    }

    private void ApplyMaterialToAllRenderers(Material mat)
    {
        if (mat == null) return;

        var renderers = includeChildren
            ? GetComponentsInChildren<Renderer>(true)
            : GetComponents<Renderer>();

        foreach (var r in renderers)
        {
            if (r == null) continue;

            if (!includeSkinnedMeshes && r is SkinnedMeshRenderer) continue;

            var count = r.sharedMaterials.Length;
            if (count < 1) count = 1;
            var arr = new Material[count];
            for (int i = 0; i < count; i++) arr[i] = mat;
            r.sharedMaterials = arr;
        }
    }

    private void OnDestroy()
    {
        CleanupGeneratedMaterial();
    }

    private void CleanupGeneratedMaterial()
    {
        if (chosen == null) return;
#if UNITY_EDITOR
        if (!Application.isPlaying)
            DestroyImmediate(chosen);
        else
            Destroy(chosen);
#else
        Destroy(chosen);
#endif
        chosen = null;
    }
}
