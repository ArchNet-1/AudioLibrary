using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Description : Audio Library is a link between Audio and enum or integer
/// @author : Louis PAKEL
/// </summary>
namespace ArchNet.Library.Audio
{
    [Serializable]
    public class AudioData
    {
        public int AudioKey;
        public AudioClip AudioValue;
        
        public AudioData(int audioKey, AudioClip audioValue)
        {
            AudioKey = audioKey;
            AudioValue = audioValue;
        }
    }

    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "ArchNet/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
          public enum KeyType
        {
            NONE,
            ENUM,
            INT
        }

        #region SerializeField

        [SerializeField] private string _enumPath;
        [SerializeField] private List<AudioData> _audioList;
        [SerializeField] bool _expandedSettings = true;
        [SerializeField] bool _forceDefaultValue;
        [SerializeField] private int _defaultValue;
        [SerializeField] KeyType _keyType = KeyType.NONE;

        #endregion

        #region Private Properties
        
        private Dictionary<int, AudioClip> _audioDict;
        private string[] _enumValues;
        private Type _enumType;

        #endregion

        #region Public Methods

        public AudioClip GetAudio(int pKeyValue)
        {
            if (CheckExistingMaterial(pKeyValue))
            {
                return _audioDict[pKeyValue];
            }

            //TODO: RETURN ERROR
            return null;
        }

        public AudioClip GetAudio(Enum pEnumValue)
        {
            Type actualEnumType = GetEnumType(_enumPath);

            if (_keyType != KeyType.ENUM)
            {
                Debug.LogWarning("[" + name + "] It seems that you're trying to get an Audio with an enum within a library not set for enum key");
            }
            else if (pEnumValue.GetType() != actualEnumType)
            {
                Debug.LogWarning("[" + name + "] It seems that you're trying to get an Audio with a different enum that the one defined to be the key in the library");
            }

            int value = Convert.ToInt32(pEnumValue);
            if (CheckExistingMaterial(value))
            {
                return _audioDict[value];
            }

            // TODO: RETURN ERROR
            return null;
        }

        #endregion

        #region Private Methods

        private bool CheckExistingMaterial(int enumValue)
        {
            if (_audioDict == null)
            {
                if (_audioList != null)
                {
                    SaveDictionnary();
                }

                if (_audioList.Count == 0)
                {
                    Debug.LogWarning("Audio Library \'" + name + "\' doesn't contain anything");
                    return false;
                }
            }
            if (false == _audioDict.ContainsKey(enumValue))
            {
                Debug.LogWarning("Library do not contain a Audio for value: " + enumValue);
                return false;
            }

            if (_audioDict.ContainsKey(enumValue))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Editor Methods

        
        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public string[] GetEnumValues(string enumName)
        {
            Type type = GetEnumType(enumName);
            if (type != null)
            {
                _enumType = type;
                _enumPath = enumName;
                _enumValues = Enum.GetNames(type);
                return _enumValues;
            }

            return null;
        }
        
        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public Type GetEnumType(string enumName)
        {
            if (false == string.IsNullOrEmpty(enumName))
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetType(enumName);
                    if (type == null)
                        continue;
                    if (type.IsEnum)
                        return type;
                }
            }
            return null;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public int GetKeyType()
        {
            return (int)_keyType;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public bool isKeyTypeUpToDate(int pKeyValue)
        {
            if ((int)_keyType == pKeyValue)
            { return true; }

            return false;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public void SaveDictionnary()
        {
            if (_audioList == null)
            { return; }

            if (_audioList.Count == 0)
            {
                Debug.LogWarning("The Audio List is empty");
            }
            _audioDict = new Dictionary<int, AudioClip>();
            for (int i = 0; i < _audioList.Count; i++)
            {
                if (_audioList[i].AudioKey != _defaultValue)
                {
                    if (_audioDict.ContainsKey(_audioList[i].AudioKey))
                    {
                        if (_audioDict[_audioList[i].AudioKey] != _audioList[i].AudioValue)
                        {
                            if (_keyType == KeyType.ENUM)
                            {
                                Debug.LogWarning("Audio \'" + _audioDict[_audioList[i].AudioKey] + "\' is already defined for \'" + _enumValues[_audioList[i].AudioKey] + "\'\nCannot define \'" + _audioList[i].AudioValue + "\' as well.");
                            }
                            else
                            {
                                Debug.LogWarning("Audio \'" + _audioDict[_audioList[i].AudioKey] + "\' is already defined for \'" + _audioList[i].AudioKey + "\'\nCannot define \'" + _audioList[i].AudioValue + "\' as well.");
                            }
                        }
                    }
                    else
                    {
                        _audioDict.Add(_audioList[i].AudioKey, _audioList[i].AudioValue);
                    }
                }
            }
        }
        
        public void AddAudio()
        {
            _audioList.Add(new AudioData(_defaultValue, null));
        }
        
        #endregion
    }
}

