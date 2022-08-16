using UnityEditor;
using UnityEngine;


/// <summary>
/// Description : Audio Library Editor
/// @author : Louis PAKEL
/// </summary>
namespace ArchNet.Library.Audio
{
     [CustomEditor(typeof(AudioLibrary))]
    public class AudioLibraryEditor : Editor
    {
        int _listCount;
        int _lastDefaultValue;
        int _indexToDelete = -1;
        string[] _enumValues;
        bool _forceUpdate;

        SerializedProperty _audioList;
        SerializedProperty _enumPath;
        SerializedProperty _keyType;
        SerializedProperty _expandedSettings;
        SerializedProperty _forceDefaultValue;
        SerializedProperty _defaultValue;

        AudioLibrary manager;

        GUIStyle _warningInfos;

        private void OnEnable()
        {
            _warningInfos = new GUIStyle();
            _warningInfos.normal.textColor = Color.red;
            _warningInfos.fontStyle = FontStyle.Bold;

            manager = target as AudioLibrary;
        }

        private void OnDisable()
        {
            manager = null;
            _warningInfos = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _audioList = serializedObject.FindProperty("_audioList");
            _enumPath = serializedObject.FindProperty("_enumPath");
            _keyType = serializedObject.FindProperty("_keyType");
            _expandedSettings = serializedObject.FindProperty("_expandedSettings");
            _forceDefaultValue = serializedObject.FindProperty("_forceDefaultValue");
            _defaultValue = serializedObject.FindProperty("_defaultValue");

            // Key Type
            EditorGUILayout.LabelField("Key Type", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_keyType, new GUIContent(""));

            EditorGUILayout.Space(10);

            if (manager.GetKeyType() > 0 && false == manager.isKeyTypeUpToDate(_keyType.intValue))
            {
                if (EditorUtility.DisplayDialog("Warning", "This will clear set properties?\nDo you want to continue?", "Yes", "No"))
                {
                    ResetEnumPath();
                    ResetAudioList();
                }
                else
                {
                    _keyType.intValue = manager.GetKeyType();
                }
            }
            _keyType.serializedObject.ApplyModifiedProperties();

            // Enum key type
            if (_keyType.intValue == 1)
            {
                EditorGUILayout.LabelField("Full Namespace Enum Path", EditorStyles.boldLabel);

                if (_enumPath.serializedObject.hasModifiedProperties)
                {
                    _enumPath.serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.PropertyField(_enumPath, new GUIContent(""));

                EditorGUILayout.Space(10);
            }

            // Settings
            _expandedSettings.isExpanded = EditorGUILayout.Foldout(_expandedSettings.isExpanded, "Settings");
            if (_expandedSettings.isExpanded)
            {
                // Force Default Value
                _forceDefaultValue.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Force Default Value"), _forceDefaultValue.boolValue);

                _lastDefaultValue = _defaultValue.intValue;
                if (_forceDefaultValue.boolValue)
                {
                    if (_keyType.intValue == 1)
                    {
                        if (_enumValues != null && _enumValues.Length > 0)
                        {
                            _defaultValue.intValue = EditorGUILayout.Popup(_defaultValue.intValue, _enumValues);
                            if (_defaultValue.serializedObject.hasModifiedProperties)
                            {
                                SetAudioListDefaultValue();
                                _forceUpdate = true;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No enum values found", _warningInfos);
                        }
                    }
                    if (_keyType.intValue == 2)
                    {
                        EditorGUILayout.PropertyField(_defaultValue, new GUIContent(""));
                        if (_defaultValue.serializedObject.hasModifiedProperties)
                        {
                            SetAudioListDefaultValue();
                            _forceUpdate = true;
                        }
                    }
                }
                else if (_forceDefaultValue.serializedObject.hasModifiedProperties)
                {
                    _defaultValue.intValue = 0;
                    SetAudioListDefaultValue();
                    _forceUpdate = true;
                }
            }

            EditorGUILayout.Space(10);

            // Add / Remove Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add a Audio"))
            {
                if (IsConditionsOK())
                {
                    manager.AddAudio();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Audio Library", EditorStyles.boldLabel);

            if (IsConditionsOK())
            {
                HandleDatasDisplay();
                SaveAudioListIfNecessary();
            }
            else
            {
                EditorGUILayout.LabelField("Missing parameters!", _warningInfos);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetAudioListDefaultValue()
        {
            SerializedProperty audioData;
            SerializedProperty Key;

            _listCount = _audioList.arraySize;
            for (int i = 0; i < _listCount; i++)
            {
                audioData = _audioList.GetArrayElementAtIndex(i);
                Key = audioData.FindPropertyRelative("AudioKey");

                if (Key.intValue == _lastDefaultValue)
                {
                    Key.intValue = _defaultValue.intValue;
                }
            }
        }

        private void HandleDatasDisplay()
        {
            SerializedProperty audioData;
            SerializedProperty audioValue;
            SerializedProperty audioKey;

            _listCount = _audioList.arraySize;
            for (int i = 0; i < _listCount; i++)
            {
                audioData = _audioList.GetArrayElementAtIndex(i);

                audioValue = audioData.FindPropertyRelative("AudioValue");
                audioKey = audioData.FindPropertyRelative("AudioKey");

                audioData.isExpanded = EditorGUILayout.Foldout(audioData.isExpanded, new GUIContent("AudioClip " + i));
                if (audioData.isExpanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(audioValue, new GUIContent(""));
                    switch (_keyType.intValue)
                    {
                        // NONE
                        case 0:
                            break;

                        // ENUM
                        case 1:
                            audioKey.intValue = EditorGUILayout.Popup(audioKey.intValue, _enumValues, GUILayout.MaxWidth(200));
                            break;

                        // INT
                        case 2:
                            EditorGUILayout.LabelField("=>", GUILayout.MaxWidth(20));
                            EditorGUILayout.PropertyField(audioKey, new GUIContent(""), GUILayout.MaxWidth(200));
                            break;
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("Warning", "Are you sure to delete this AudioClip from the library?", "Yes", "No"))
                        {
                            audioData.isExpanded = false;
                            _audioList.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5);
                }
            }
        }

        private void ResetEnumPath()
        {
            _enumPath.stringValue = "";
            _enumPath.serializedObject.ApplyModifiedProperties();
        }

        private void ResetAudioList()
        {
            SerializedProperty audioData;
            _listCount = _audioList.arraySize;
            for (int i = 0; i < _listCount; i++)
            {
                audioData = _audioList.GetArrayElementAtIndex(i);
                audioData.isExpanded = false;
            }
            _audioList.ClearArray();
            _enumPath.serializedObject.ApplyModifiedProperties();
        }

        private bool IsConditionsOK()
        {
            if (_keyType.intValue == 0)
            {
                return false;
            }

            if (_keyType.intValue == 1)
            {
                if (string.IsNullOrEmpty(_enumPath.stringValue))
                {
                    return false;
                }

                _enumValues = manager.GetEnumValues(_enumPath.stringValue);
                if (_enumValues == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void SaveAudioListIfNecessary()
        {
            if ((_audioList.serializedObject.hasModifiedProperties || _forceUpdate) && _audioList.arraySize > 0)
            {
                manager.SaveDictionnary();
                _forceUpdate = false;
            }
            _audioList.serializedObject.ApplyModifiedProperties();
        }
    }
}
