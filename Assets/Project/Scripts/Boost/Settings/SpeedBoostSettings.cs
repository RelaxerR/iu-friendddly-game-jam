using UnityEngine;

namespace Project.Scripts.Boost.Settings
{
  [CreateAssetMenu(fileName = "Speed boost", menuName = "Boosts/Speed boost", order = 1)]
  public class SpeedBoostSettings : ScriptableObject
  {
    public float SpeedMultiplier;
    public float Duration;
  }
}
