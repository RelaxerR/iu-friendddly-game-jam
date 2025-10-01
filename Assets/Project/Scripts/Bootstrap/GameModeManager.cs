using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Project.Scripts.Bootstrap
{
    /// <summary>
    /// Управляет режимами игры (RedTime, GreenTime) и обеспечивает переключение между ними.
    /// </summary>
    public class GameModeManager : MonoBehaviour
    {
        #region Singleton

        [CanBeNull]
        private static GameModeManager _instance;

        /// <summary>
        /// Возвращает текущий экземпляр GameModeManager. Если экземпляр не существует, 
        /// ищет его в сцене.
        /// </summary>
        /// <returns>Экземпляр GameModeManager.</returns>
        public static GameModeManager GetInstance()
        {
            _instance ??= FindAnyObjectByType<GameModeManager>();
            return _instance;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Событие, вызываемое при изменении режима игры.
        /// </summary>
        public event Action<GameMode> OnGameModeChanged;

        /// <summary>
        /// Перечисление возможных режимов игры.
        /// </summary>
        public enum GameMode
        {
            RedTime,
            GreenTime
        }

        /// <summary>
        /// Текущий режим игры.
        /// </summary>
        public GameMode CurrentMode { get; private set; } = GameMode.GreenTime;

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Переключает текущий режим игры на указанный.
        /// </summary>
        /// <param name="mode">Режим, на который нужно переключиться.</param>
        public void SwitchTo(GameMode mode)
        {
            if (CurrentMode == mode) return;
            CurrentMode = mode;
            OnGameModeChanged?.Invoke(CurrentMode);
        }

        #endregion
    }
}
