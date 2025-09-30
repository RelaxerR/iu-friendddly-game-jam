using UnityEngine;

namespace Project.Scripts.Boost.Settings
{
    /// <summary>
    /// Настройки буста здоровья.
    /// </summary>
    [CreateAssetMenu(fileName = "Health Boost", menuName = "Boosts/Health Boost", order = 0)]
    public class HealthBoostSettings : ScriptableObject
    {
        /// <summary>
        /// Количество здоровья, которое восстанавливается.
        /// </summary>
        public int HealAmount;
    }
}
