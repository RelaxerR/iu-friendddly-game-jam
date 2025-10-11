using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMPro.TextMeshProUGUI hpText;

    [Header("Deaths UI")]
    [SerializeField] private TMPro.TextMeshProUGUI deathsCount;
    [SerializeField] private GameObject deathsContainer;

    [Header("Kills UI")]
    [SerializeField] private TMPro.TextMeshProUGUI killsCount;
    [SerializeField] private GameObject killsContainer;

    private Health localHealth;
    private bool isInitialized = false;

    private void Awake()
    {
        if (hpSlider != null) hpSlider.gameObject.SetActive(false);
        if (hpText != null) hpText.gameObject.SetActive(false);
        if (deathsContainer != null) deathsContainer.SetActive(false);
        if (killsContainer != null) killsContainer.SetActive(false);
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
            if (hpSlider != null)
                hpSlider.value = localHealth.NetworkedHealth;

            if (hpText != null)
                hpText.text = Mathf.CeilToInt(localHealth.NetworkedHealth).ToString();

            if (deathsCount != null)
                deathsCount.text = localHealth.DeathCount.ToString();

            if (killsCount != null)
                killsCount.text = localHealth.KillCount.ToString();
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
                if (hpSlider != null) hpSlider.gameObject.SetActive(true);
                if (hpText != null) hpText.gameObject.SetActive(true);
                if (deathsContainer != null) deathsContainer.SetActive(true);
                if (killsContainer != null) killsContainer.SetActive(true);

                if (deathsCount != null) deathsCount.text = localHealth.DeathCount.ToString();
                if (killsCount != null) killsCount.text = localHealth.KillCount.ToString();
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
