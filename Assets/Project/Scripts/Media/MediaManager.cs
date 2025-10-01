using System;
using System.Collections;
using JetBrains.Annotations;
using Project.Scripts.Bootstrap;
using UnityEngine;

namespace Project.Scripts.Media
{
    /// <summary>
    /// Управляет воспроизведением аудио в зависимости от текущего режима игры.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MediaManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private MediaSettings _settings;

        [CanBeNull] private static MediaManager _instance;

        private AudioSource _audioSourceA;
        private AudioSource _audioSourceB;
        private AudioSource _currentSource;
        private AudioSource _nextSource;

        #endregion

        #region Singleton

        /// <summary>
        /// Возвращает текущий экземпляр MediaManager. Если экземпляр не существует, 
        /// ищет его в сцене.
        /// </summary>
        /// <returns>Экземпляр MediaManager.</returns>
        public static MediaManager GetInstance()
        {
            if (!_instance)
            {
                _instance = FindAnyObjectByType<MediaManager>();
            }
            return _instance;
        }

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // Создаем два AudioSource
            _audioSourceA = gameObject.AddComponent<AudioSource>();
            _audioSourceB = gameObject.AddComponent<AudioSource>();

            _audioSourceA.playOnAwake = false;
            _audioSourceB.playOnAwake = false;

            _audioSourceA.volume = _settings.MusicVolume;
            _audioSourceB.volume = 0f;

            _currentSource = _audioSourceA;
            _nextSource = _audioSourceB;

            // Подписываемся на событие
            GameModeManager.GetInstance().OnGameModeChanged += OnGameModeChanged;

            // Начинаем с текущего режима
            PlayCurrentMode();
        }

        private void OnDestroy()
        {
            GameModeManager.GetInstance().OnGameModeChanged -= OnGameModeChanged;
        }

        #endregion

        #region Event Handlers

        private void OnGameModeChanged(GameModeManager.GameMode mode)
        {
            StartCoroutine(FadeBetweenTracks(mode));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Воспроизводит музыку для текущего режима.
        /// </summary>
        private void PlayCurrentMode()
        {
            var clip = GetCurrentClip();
            if (!clip)
                return;
            
            _currentSource.clip = clip;
            _currentSource.loop = true;
            _currentSource.Play();
        }

        /// <summary>
        /// Возвращает аудиоклип, соответствующий текущему режиму игры.
        /// </summary>
        /// <returns>Аудиоклип для текущего режима.</returns>
        private AudioClip GetCurrentClip()
        {
            var mode = GameModeManager.GetInstance().CurrentMode;
            return mode == GameModeManager.GameMode.RedTime ? _settings.RedTimeMusic : _settings.GreenTimeMusic;
        }

        /// <summary>
        /// Плавно переключает между аудиотреками при смене режима.
        /// </summary>
        /// <param name="newMode">Новый режим игры.</param>
        /// <returns>Корутина для плавного переключения.</returns>
        private IEnumerator FadeBetweenTracks(GameModeManager.GameMode newMode)
        {
            // Назначаем новый клип на следующий источник
            var newClip = newMode == GameModeManager.GameMode.RedTime ? _settings.RedTimeMusic : _settings.GreenTimeMusic;

            if (newClip)
            {
                _nextSource.clip = newClip;
                _nextSource.loop = true;
                _nextSource.volume = 0f;
                _nextSource.Play();
            }

            var elapsed = 0f;
            var duration = _settings.FadeInDuration;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;

                _currentSource.volume = Mathf.Lerp(_settings.MusicVolume, 0f, t);
                _nextSource.volume = Mathf.Lerp(0f, _settings.MusicVolume, t);

                yield return null;
            }

            // После фейда меняем местами источники
            (_currentSource, _nextSource) = (_nextSource, _currentSource);
        }

        #endregion
    }
}