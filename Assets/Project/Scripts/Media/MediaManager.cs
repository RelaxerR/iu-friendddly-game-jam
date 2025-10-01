using System;
using System.Collections;
using UnityEngine;
using Fusion;
using Project.Scripts.Bootstrap;

namespace Project.Scripts.Media
{
    /// <summary>
    /// Управляет воспроизведением аудио в зависимости от текущего режима игры.
    /// Работает ТОЛЬКО на клиенте (хост или remote client).
    /// </summary>
    public class MediaManager : MonoBehaviour
    {
        [SerializeField] private MediaSettings _settings;

        private AudioSource _audioSourceA;
        private AudioSource _audioSourceB;
        private AudioSource _currentSource;
        private AudioSource _nextSource;

        private GameModeManager _gameModeManager;

        private void Start()
        {
            _audioSourceA = gameObject.AddComponent<AudioSource>();
            _audioSourceB = gameObject.AddComponent<AudioSource>();

            _audioSourceA.playOnAwake = false;
            _audioSourceB.playOnAwake = false;

            _audioSourceA.volume = _settings.MusicVolume;
            _audioSourceB.volume = 0f;

            _currentSource = _audioSourceA;
            _nextSource = _audioSourceB;

            // Найти GameModeManager (он должен быть Scene Object или spawned)
            _gameModeManager = FindAnyObjectByType<GameModeManager>();
            if (!_gameModeManager)
            {
                Debug.LogError("MediaManager: GameModeManager not found in scene!");
                enabled = false;
                return;
            }

            // Подписываемся на событие (оно вызывается на всех клиентах)
            _gameModeManager.OnGameModeChanged += OnGameModeChanged;

            // Применяем начальное состояние
            OnGameModeChanged(_gameModeManager.CurrentMode);
        }

        private void OnDestroy()
        {
            if (_gameModeManager)
            {
                _gameModeManager.OnGameModeChanged -= OnGameModeChanged;
            }
        }

        private void OnGameModeChanged(GameModeManager.GameMode mode)
        {
            StartCoroutine(FadeBetweenTracks(mode));
        }

        private IEnumerator FadeBetweenTracks(GameModeManager.GameMode newMode)
        {
            var newClip = newMode == GameModeManager.GameMode.RedTime 
                ? _settings.RedTimeMusic 
                : _settings.GreenTimeMusic;

            if (!newClip)
            {
                yield break;
            }

            _nextSource.clip = newClip;
            _nextSource.loop = true;
            _nextSource.volume = 0f;
            _nextSource.Play();

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

            // Меняем источники местами
            (_currentSource, _nextSource) = (_nextSource, _currentSource);
        }
    }
}