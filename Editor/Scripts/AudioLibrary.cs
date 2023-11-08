using System;
using System.Collections.Generic;
using Flow.Scripts.Element_Service.Enums;
using UnityEngine;

namespace Library
{
    /// <summary>
    /// AudioLibrary is a library that stores common AudioCLips 
    /// </summary>
    [CreateAssetMenu(menuName = "Catalyst/Libraries/AudioLibrary", fileName = nameof(AudioLibrary) + ".asset")]
    public class AudioLibrary : Library<AudioClip>
    {
        protected override AudioClip DefaultValue() => null;
        public override ISet<Type> AuthorizedEnums()
        {
            return new HashSet<Type>
            {
                typeof(Music),
                typeof(Kingdom)
            };
        }
    }
}