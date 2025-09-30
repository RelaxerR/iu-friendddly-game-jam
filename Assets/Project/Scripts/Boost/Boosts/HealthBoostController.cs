using Project.Scripts.Boost.Abstract;
using Project.Scripts.Boost.Settings;
using UnityEngine;

namespace Project.Scripts.Boost.Boosts
{
  public class HealthBoostController : MonoBehaviour, IBoost
  {
    [SerializeField]
    private HealthBoostSettings healthBoostSettings;
    
    public void Apply()
    {
      Destroy(gameObject);
      //TODO: добавить логику увеличения ХП
      throw new System.NotImplementedException("Health boost logic is not implemented");
    }
    public void Remove() { }
  }
}
