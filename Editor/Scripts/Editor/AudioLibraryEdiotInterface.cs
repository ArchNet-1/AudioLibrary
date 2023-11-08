using UnityEditor;
using UnityEngine;

namespace Library.Editor
{
    [CustomEditor(typeof(AudioLibrary))]
    public class AudioLibraryEditorInterface : LibraryEditorInterface<AudioLibrary, AudioClip>
    {
        protected override bool AreDifferent(AudioClip a, AudioClip b) => a != b;

        protected override AudioClip DisplayGuiAndSelect(string label, AudioClip currentValue)
        {
            return EditorGUILayout.ObjectField(
                label,
                currentValue,
                typeof(AudioClip),
                false
            ) as AudioClip;
        }
    }
}