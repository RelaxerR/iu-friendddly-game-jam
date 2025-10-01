using System;
using UnityEngine;
using Fusion;

namespace Project.Scripts.Bootstrap
{
    /// <summary>
    /// Управляет глобальным режимом игры (RedTime / GreenTime) в мультиплеерной среде через Photon Fusion.
    /// Смена режима возможна только на сервере. Все клиенты получают обновление автоматически.
    /// </summary>
    public class GameModeManager : NetworkBehaviour
    {
        public override void Spawned()
        {
            base.Spawned();
            CurrentMode = GameMode.GreenTime;
            OnGameModeChanged?.Invoke(CurrentMode);
            Debug.Log("GameModeManager.Spawned");
        }

        public enum GameMode : byte
        {
            RedTime,
            GreenTime
        }

        // Событие, вызываемое на всех клиентах (включая сервер) при смене режима
        public event Action<GameMode> OnGameModeChanged;

        // Синхронизируемое поле — Fusion автоматически рассылает изменения от сервера ко всем клиентам
        [Networked, OnChangedRender(nameof(OnCurrentModeChanged))]
        public GameMode CurrentMode { get; private set; }

        // Вызывается на всех клиентах при получении нового значения CurrentMode от сервера
        private void OnCurrentModeChanged()
        {
            OnGameModeChanged?.Invoke(CurrentMode);
        }

        /// <summary>
        /// Переключает режим игры. Может быть вызван ТОЛЬКО на сервере (state authority).
        /// </summary>
        /// <param name="mode">Новый режим игры.</param>
        private void SwitchTo(GameMode mode)
        {
            if (!Object.HasStateAuthority)
            {
                Debug.LogError("SwitchTo can only be called on the state authority (server).");
                return;
            }

            if (CurrentMode == mode)
                return;

            CurrentMode = mode;
            // OnCurrentModeChanged вызовется автоматически на всех клиентах благодаря [OnChangedRender]
        }

        /// <summary>
        /// RPC для запроса смены режима от клиента к серверу.
        /// Используйте этот метод, если клиент должен инициировать смену (например, по таймеру или событию).
        /// </summary>
        /// <param name="mode">Желаемый режим.</param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SwitchTo(GameMode mode)
        {
            // Выполняется только на сервере
            SwitchTo(mode);
        }
    }
}