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
        [SerializeField] private GameModeManager gameModeManager;

        private void Awake()
        {
            // Подписываемся как можно раньше
            if (gameModeManager)
            {
                gameModeManager.OnGameModeChanged += OnGameModeChanged;
            }
        }

        private void OnGameModeChanged(GameModeManager.GameMode mode)
        {
            // Здесь безопасно использовать mode
            ApplyGameMode(mode);
        }

        private void ApplyGameMode(GameModeManager.GameMode mode)
        {
            Debug.Log($"Запуск трека для режима [{mode}]");
            StartCoroutine(FadeBetweenTracks(mode));
        }
        
        [SerializeField] private MediaSettings _settings;

        private AudioSource _audioSourceA;
        private AudioSource _audioSourceB;
        private AudioSource _currentSource;
        private AudioSource _nextSource;

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
            if (!gameModeManager)
            {
                Debug.LogError("MediaManager: GameModeManager not found in scene!");
                enabled = false;
                return;
            }

            // Подписываемся на событие (оно вызывается на всех клиентах)
            gameModeManager.OnGameModeChanged += OnGameModeChanged;
        }

        private void OnDestroy()
        {
            if (gameModeManager)
            {
                gameModeManager.OnGameModeChanged -= OnGameModeChanged;
            }
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