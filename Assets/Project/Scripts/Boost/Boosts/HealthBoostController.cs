using Project.Scripts.Boost.Abstract;
using Project.Scripts.Boost.Settings;
using UnityEngine;

namespace Project.Scripts.Boost.Boosts
{
    /// <summary>
    /// Контроллер буста здоровья.
    /// </summary>
    public class HealthBoostController : MonoBehaviour, IBoost
    {
        #region Fields

        [SerializeField] private HealthBoostSettings healthBoostSettings;

        #endregion

        #region IBoost Implementation

        /// <inheritdoc />
        public void Apply()
        {
            // TODO: добавить логику увеличения ХП
            var healAmount = healthBoostSettings.HealAmount;
            Debug.LogWarning("Health boost logic is not implemented");
            Destroy(gameObject);
        }

        /// <inheritdoc />
        public void Remove()
        {
            // Буст здоровья не требует отмены.
        }

        #endregion
    }
}
