using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMPro.TextMeshProUGUI hpText;

    private Health localHealth;

    private bool isInitialized = false;

    private void Update()
    {
        if (!isInitialized)
        {
            TryInitialize();
            return;
        }

        if (localHealth != null)
        {
            hpSlider.value = localHealth.NetworkedHealth;
            if (hpText != null)
                hpText.text = Mathf.CeilToInt(localHealth.NetworkedHealth).ToString();
        }
    }

    private void TryInitialize()
    {
        foreach (var h in FindObjectsOfType<Health>())
        {
            if (h.Object.HasInputAuthority)
            {
                localHealth = h;
                localHealth.OnDeath += OnPlayerDeath;
                isInitialized = true;
                break;
            }
        }
    }

    private void OnPlayerDeath(Health victim, PlayerRef killer)
    {
        hpSlider.value = 0;
        if (hpText != null) hpText.text = "0";
    }
}
