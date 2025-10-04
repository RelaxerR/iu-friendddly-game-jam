using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMPro.TextMeshProUGUI hpText;
    [SerializeField] private TMPro.TextMeshProUGUI deathsCount;
    [SerializeField] private GameObject deathsContainer;

    private Health localHealth;
    private bool isInitialized = false;

    private void Awake()
    {
        hpSlider.gameObject.SetActive(false);
        if (hpText != null)
            hpText.gameObject.SetActive(false);

        if (deathsContainer != null)
            deathsContainer.SetActive(false);
    }

    private void Update()
    {
        if (!isInitialized)
        {
            TryInitialize();
            return;
        }

        if (localHealth != null)
        {
            deathsCount.text = localHealth.DeathCount.ToString();

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

                hpSlider.gameObject.SetActive(true);
                if (hpText != null)
                    hpText.gameObject.SetActive(true);

                if (deathsContainer != null)
                    deathsContainer.SetActive(true);

                deathsCount.text = localHealth.DeathCount.ToString();
                break;
            }
        }
    }

    private void OnPlayerDeath(Health victim, PlayerRef killer)
    {
        deathsCount.text = victim.DeathCount.ToString();

        hpSlider.value = 0;
        if (hpText != null)
            hpText.text = "0";
    }
}
