using Project.Scripts.Boost.Abstract;
using Project.Scripts.Boost.Settings;
using UnityEngine;

namespace Project.Scripts.Boost.Boosts
{
  public class SpeedBoostController : MonoBehaviour, IBoostDurable
  {
    [SerializeField]
    private SpeedBoostSettings speedBoostSettings;
    
    public void Apply()
    {
      Destroy(gameObject);
      //TODO: добавить логику увеличения Скорости
      throw new System.NotImplementedException("Speed boost logic is not implemented");
    }
    public void Remove()
    {
      //TODO: добавить логику отмены увеличения Скорости
      throw new System.NotImplementedException("Speed boost logic is not implemented");
    }
    public float GetDuration() => speedBoostSettings.Duration;
  }
}
