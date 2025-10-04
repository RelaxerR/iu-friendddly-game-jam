using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private Vector2 size = new Vector2(64, 64);

    private Image _crosshairImage;

    private void Awake()
    {
        var go = new GameObject("Crosshair", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(transform, false);

        _crosshairImage = go.GetComponent<Image>();
        _crosshairImage.sprite = crosshairSprite;
        _crosshairImage.color = Color.white;
        _crosshairImage.raycastTarget = false;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
    }
}
