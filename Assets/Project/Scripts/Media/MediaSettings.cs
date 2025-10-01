using UnityEngine;

namespace Project.Scripts.Media
{
    /// <summary>
    /// Настройки для управления медиафайлами в зависимости от режима игры.
    /// </summary>
    [CreateAssetMenu(fileName = "Media settings", menuName = "Media", order = 0)]
    public class MediaSettings : ScriptableObject
    {
        #region Audio Clips

        /// <summary>
        /// Музыка для режима RedTime.
        /// </summary>
        public AudioClip RedTimeMusic;

        /// <summary>
        /// Музыка для режима GreenTime.
        /// </summary>
        public AudioClip GreenTimeMusic;

        #endregion

        #region Volume Settings

        /// <summary>
        /// Громкость воспроизводимой музыки.
        /// </summary>
        [Range(0f, 1f)] public float MusicVolume;

        /// <summary>
        /// Длительность плавного перехода между треками.
        /// </summary>
        public float FadeInDuration;

        #endregion
    }
}
