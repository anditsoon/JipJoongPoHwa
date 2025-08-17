using System.Collections;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    // 모델 바디 컬러
    private Renderer[] renderers;
    private Color originalColor;
    [SerializeField] private Color hitColor = new Color(1f, 0f, 0f, 0.5f);
    private float colorChangeDuration = 0.5f;

    void Start()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // 기존 컬러 저장
        if (renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
        }
    }

    public IEnumerator ChangeColorTemporarily()
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material.color = hitColor;
            }
        }

        yield return new WaitForSeconds(colorChangeDuration);

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material.color = originalColor;
            }
        }
    }
}
