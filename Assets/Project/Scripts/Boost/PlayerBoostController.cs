using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Boost.Abstract;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Boost
{
  [RequireComponent(typeof(BoxCollider))]
  public class PlayerBoostController : MonoBehaviour, IBoostController
  {
    private readonly List<IBoost> activeBoosts = new();
    
    public void AddBoost(IBoost boost)
    {
      if (activeBoosts.Contains(boost)) return;
      
      boost.Apply();
      activeBoosts.Add(boost);

      var duration = boost is IBoostDurable durableBoost ? durableBoost.GetDuration() : 0;
      StartCoroutine(RemoveBoostCoroutine(boost, duration));
    }
    public void RemoveBoost(IBoost boost)
    {
      if (!activeBoosts.Contains(boost)) throw new ArgumentException("Boost not found");
      
      boost.Remove();
      activeBoosts.Remove(boost);
    }

    private IEnumerator RemoveBoostCoroutine(IBoost boost, float duration)
    {
      yield return new WaitForSeconds(duration);
      RemoveBoost(boost);
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!other.CompareTag("Boost")) return;
      
      var boost = other.GetComponent<IBoost>();
      AddBoost(boost);
    }
  }
}
