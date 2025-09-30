using Project.Scripts.Boost.Abstract;
using Project.Scripts.Boost.Settings;
using UnityEngine;

namespace Project.Scripts.Boost.Boosts
{
    /// <summary>
    /// Контроллер буста скорости.
    /// </summary>
    public class SpeedBoostController : MonoBehaviour, IBoostDurable
    {
        #region Fields

        [SerializeField] private SpeedBoostSettings speedBoostSettings;

        #endregion

        #region IBoost Implementation

        /// <inheritdoc />
        public void Apply()
        {
            // TODO: добавить логику увеличения Скорости
            var speedMultiplier = speedBoostSettings.SpeedMultiplier; // Скорость увеличивается в это кол-во раз
            Debug.LogWarning("Speed boost logic is not implemented");
            Destroy(gameObject);
        }

        /// <inheritdoc />
        public void Remove()
        {
            // TODO: добавить логику отмены увеличения Скорости
            Debug.LogWarning("Speed boost removal logic is not implemented");
        }

        #endregion

        #region IBoostDurable Implementation

        /// <inheritdoc />
        public float GetDuration() => speedBoostSettings.Duration;

        #endregion
    }
}
