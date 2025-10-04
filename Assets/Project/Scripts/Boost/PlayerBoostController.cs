using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Boost.Abstract;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Boost
{
    /// <summary>
    /// Контроллер бустов игрока.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerBoostController : MonoBehaviour, IBoostController
    {
        #region Fields

        private readonly List<IBoost> activeBoosts = new();

        #endregion

        #region IBoostController Implementation

        /// <inheritdoc />
        public void AddBoost(IBoost boost)
        {
            if (boost == null) throw new ArgumentNullException(nameof(boost));
            if (activeBoosts.Contains(boost)) return;

            boost.Apply();
            activeBoosts.Add(boost);

            // Если буст временный, запускаем отсчёт времени
            if (boost is not IBoostDurable durableBoost)
                return;
            
            var duration = durableBoost.GetDuration();
            StartCoroutine(RemoveBoostCoroutine(boost, duration));
        }

        /// <inheritdoc />
        public void RemoveBoost(IBoost boost)
        {
            if (boost == null) throw new ArgumentNullException(nameof(boost));
            if (!activeBoosts.Contains(boost)) throw new ArgumentException("Boost not found");

            boost.Remove();
            activeBoosts.Remove(boost);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Отложенное удаление буста по истечении времени.
        /// </summary>
        /// <param name="boost">Буст для удаления.</param>
        /// <param name="duration">Время до удаления.</param>
        private IEnumerator RemoveBoostCoroutine(IBoost boost, float duration)
        {
            yield return new WaitForSeconds(duration);
            RemoveBoost(boost);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Boost")) return;

            var boost = other.GetComponent<IBoost>();
            if (boost != null) AddBoost(boost);
        }

        #endregion
    }
}