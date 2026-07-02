using UnityEngine;
using UnityEditor;

using Inspector;

namespace Assets.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Save the previous GUI enabled state
            bool previousGUIState = GUI.enabled;
    
            // Disable the GUI, making the field non-interactive
            GUI.enabled = false;
    
            // Draw the property field normally, but it will be read-only
            EditorGUI.PropertyField(position, property, label);
    
            // Restore the previous GUI enabled state for subsequent fields
            GUI.enabled = previousGUIState;
        }
    }
}
