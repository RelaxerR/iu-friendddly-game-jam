using UnityEngine;

namespace Project.Scripts.Boost.Settings
{
  [CreateAssetMenu(fileName = "Health boost", menuName = "Boosts/Health boost", order = 0)]
  public class HealthBoostSettings : ScriptableObject
  {
    public int HealAmount;
  }
}
