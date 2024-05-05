using RobbysUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteColorGroup : MonoBehaviour
{
    [SerializeField] SpriteColorGroupRunMode runMode;
    [SerializeField] Color color = Color.white;
    [Range(0, 1)]
    [SerializeField] float alpha;

    [SerializeField] SpriteRenderer[] cachedSRs;

    void OnValidate()
    {
        if (runMode != SpriteColorGroupRunMode.PlayerOnly)
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        else
        {
            foreach (var spriteRenderer in cachedSRs)
            {
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
    }

    private void Start()
    {
        if (runMode != SpriteColorGroupRunMode.EditorOnly)
            RefreshCachedSpriteRenderers();
    }

    public void ToggleSpriteVisibility(bool visible, float alphaLevelIfVisible = 0.176f)
    {
        if (runMode == SpriteColorGroupRunMode.EditorOnly)
        {
            Debug.LogError("Cannot Toggle Sprite Visibility when in Editor Only Mode");
            return;
        }

        if (visible)
        {
            alpha = alphaLevelIfVisible;
            foreach (var spriteRenderer in cachedSRs)
            {
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        else
        {
            alpha = 0;
            foreach (var spriteRenderer in cachedSRs)
            {
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
    }

    public void RefreshCachedSpriteRenderers()
    {
        cachedSRs = cachedSRs.Clear();
        cachedSRs = GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in cachedSRs.ToArray())
        {
            if (!spriteRenderer.gameObject.activeSelf)
                cachedSRs = cachedSRs.RemoveItem(spriteRenderer);
        }
    }

    public void SetColor(Color color)
    {
        if (this.color == color) return;

        this.color = color;
        this.alpha = color.a;
        OnValidate();
    }
}

public enum SpriteColorGroupRunMode
{
    EditorOnly,
    PlayerAndEditor,
    PlayerOnly,
}
