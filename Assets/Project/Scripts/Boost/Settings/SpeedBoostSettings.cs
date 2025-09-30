using UnityEngine;

namespace Project.Scripts.Boost.Settings
{
    /// <summary>
    /// Настройки буста скорости.
    /// </summary>
    [CreateAssetMenu(fileName = "Speed Boost", menuName = "Boosts/Speed Boost", order = 1)]
    public class SpeedBoostSettings : ScriptableObject
    {
        /// <summary>
        /// Множитель скорости.
        /// </summary>
        public float SpeedMultiplier;

        /// <summary>
        /// Продолжительность действия буста в секундах.
        /// </summary>
        public float Duration;
    }
}
