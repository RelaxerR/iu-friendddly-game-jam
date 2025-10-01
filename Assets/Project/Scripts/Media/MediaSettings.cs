using UnityEngine;

namespace Project.Scripts.Media
{
    [CreateAssetMenu(fileName = "Media settings", menuName = "Media", order = 0)]
    public class MediaSettings : ScriptableObject
    {
        public AudioClip GreenTimeMusic;
        public AudioClip RedTimeMusic;

        public float MusicVolume;
        public float FadeInDuration;
    }
}
