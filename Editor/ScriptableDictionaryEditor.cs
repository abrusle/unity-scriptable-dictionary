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

            public const float
                LineIconWidth = 17;
        }
        
        private struct Properties
        {
            public SerializedProperty
                keys,
                values;
        }

        private struct Contents
        {
            public static GUIContent 
                AddButton,
                DeleteButton,
                IconDuplicateKey,
                IconNullKey;
            
            public static void Load()
            {
                AddButton = new GUIContent("+", "Add an new item");
                DeleteButton = new GUIContent("x", "Delete this item");
                Texture warnIcon = EditorGUIUtility.IconContent("console.warnicon.sml").image;
                IconDuplicateKey = new GUIContent(warnIcon, "Conflict: please remove/change duplicate key.");
                IconNullKey = new GUIContent(warnIcon, "Conflict: null is an invalid value for a key.");
            }
        }

        private class Conflict
        {
            public static Conflict DuplicateKey => new Conflict(Contents.IconDuplicateKey, new Color(1f, 0.85f, 0.46f));
            public static Conflict NullKey => new Conflict(Contents.IconNullKey, new Color(1f, 0.85f, 0.46f));
            public static readonly Conflict None = new Conflict(GUIContent.none, Color.white);
            
            public readonly GUIContent content;
            public readonly Color color;

            public Conflict(GUIContent content, Color color)
            {
                this.content = content;
                this.color = color;
            }
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
            
            Contents.Load();
        }

        /// <inheritdoc />
        public override bool UseDefaultMargins() => false;

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
                Space(Constants.LineIconWidth);
                LabelField($"Keys ({_properties.keys.arrayElementType})", EditorStyles.boldLabel);
                LabelField($"Values ({_properties.values.arrayElementType})", EditorStyles.boldLabel);
            }
            
            Separator();
            
            EditorGUI.BeginChangeCheck();
            int length = _properties.keys.arraySize;
            for (int i = 0; i < length; i++)
            {
                DrawKeyValueLine(i,
                    _properties.keys.GetArrayElementAtIndex(i),
                    _properties.values.GetArrayElementAtIndex(i), Conflict.None);
                
                if (_cancelFollowingLinesDraw)
                {
                    _cancelFollowingLinesDraw = false;
                    break;
                }
            }

            GUI.color = Color.white;

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            using (new HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Contents.AddButton, GUILayout.Width(50)))
                {
                    _properties.keys.InsertArrayElementAtIndex(length);
                    _properties.values.InsertArrayElementAtIndex(length);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawKeyValueLine(int index, SerializedProperty keyProp, SerializedProperty valProp, Conflict conflict)
        {
            using (new HorizontalScope())
            {
                GUI.color = conflict.color;
                LabelField(conflict.content, GUILayout.MaxWidth(Constants.LineIconWidth));
                PropertyField(keyProp, GUIContent.none);
                PropertyField(valProp, GUIContent.none);

                if (GUILayout.Button(Contents.DeleteButton))
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