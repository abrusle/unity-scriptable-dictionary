using ScriptableDictionaries.Internal;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace ScriptableDictionaries.Editor
{
    [CustomEditor(typeof(ScriptableDictionaryBase), true)]
    public class ScriptableDictionaryEditor : UnityEditor.Editor
    {
        private struct Constants
        {
            public const string 
                PropNameKeys = "keys",
                PropNameValues = "values";
        }
        
        private struct Properties
        {
            public SerializedProperty
                keys,
                values;
        }

        private Properties _properties;
        private bool _cancelFollowingLinesDraw;

        private void OnEnable()
        {
            _properties = new Properties
            {
                keys = serializedObject.FindProperty(Constants.PropNameKeys),
                values = serializedObject.FindProperty(Constants.PropNameValues)
            };
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            if (_properties.keys == null || _properties.values == null)
            {
                HelpBox("Invalid ScriptableDictionary", MessageType.Error);
                GUI.enabled = false;
                DrawDefaultInspector();
                return;
            }
            
            using (new HorizontalScope())
            {
                LabelField($"Keys ({_properties.keys.arrayElementType})", EditorStyles.boldLabel);
                LabelField($"Values ({_properties.values.arrayElementType})", EditorStyles.boldLabel);
            }
            
            Separator();

            int length = _properties.keys.arraySize;
            for (int i = 0; i < length; i++)
            {
                DrawKeyValueLine(i,
                    _properties.keys.GetArrayElementAtIndex(i),
                    _properties.values.GetArrayElementAtIndex(i));
                
                if (_cancelFollowingLinesDraw)
                {
                    _cancelFollowingLinesDraw = false;
                    break;
                }
            }

            using (new HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(50)))
                {
                    _properties.keys.InsertArrayElementAtIndex(length);
                    _properties.values.InsertArrayElementAtIndex(length);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawKeyValueLine(int index, SerializedProperty keyProp, SerializedProperty valProp)
        {
            using (new HorizontalScope())
            {
                // LabelField(index.ToString(), GUILayout.Width(10));
                PropertyField(keyProp, GUIContent.none);
                PropertyField(valProp, GUIContent.none);

                if (GUILayout.Button("-"))
                {
                    _properties.keys.DeleteArrayElementAtIndex(index);
                    _properties.values.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    _cancelFollowingLinesDraw = true;
                }
            }
        }
    }
}