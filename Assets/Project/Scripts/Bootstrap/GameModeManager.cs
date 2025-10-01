using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Project.Scripts.Bootstrap
{
  public class GameModeManager : MonoBehaviour
  {
    [CanBeNull]
    private static GameModeManager _instance;
    public static GameModeManager GetInstance()
    {
      _instance ??= FindAnyObjectByType<GameModeManager>();
      return _instance;
    }

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }

    public event Action<GameMode> OnGameModeChanged;

    public enum GameMode { RedTime, GreenTime }

    public GameMode CurrentMode { get; private set; } = GameMode.GreenTime;

    public void SwitchTo(GameMode mode)
    {
      if (CurrentMode == mode) return;
      CurrentMode = mode;
      OnGameModeChanged?.Invoke(CurrentMode);
    }
  }
}
