// ArchNet · Audio Library
// Stores audio clips under a key (an enum value such as Music.Menu, or an int), so code
// plays sounds by name and a clip can be replaced in one place.
// Create one with  Assets > Create > ArchNet > Audio Library.
// Docs: https://archnet-plateform.fr/#AudioLibrary

using System;
using UnityEngine;

namespace ArchNet.Library
{
    [CreateAssetMenu(fileName = "NewAudioLibrary", menuName = "ArchNet/Audio Library")]
    public class AudioLibrary : KeyedLibrary<AudioClip>
    {
        public AudioClip GetClip(int key) => Get(key);
        public AudioClip GetClip(Enum key) => Get(key);

        /// <summary>Plays the clip stored for this key once on the source (overlapping sounds allowed).</summary>
        public void PlayOneShot(AudioSource source, int key, float volume = 1f)
        {
            if (source != null && TryGet(key, out AudioClip clip) && clip != null) source.PlayOneShot(clip, volume);
        }

        public void PlayOneShot(AudioSource source, Enum key, float volume = 1f) =>
            PlayOneShot(source, Convert.ToInt32(key), volume);
    }
}
