using UnityEngine;
using UnityEditor;

namespace Beetle23
{
    public static class GameKitEditorDrawUtil
    {
        static GameKitEditorDrawUtil()
        {
            TitleStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            TitleStyle.alignment = TextAnchor.MiddleCenter;
            TitleStyle.fontStyle = FontStyle.Bold;

            ItemCenterLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            ItemCenterLabelStyle.alignment = TextAnchor.MiddleCenter;

            ItemLeftLabelStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
            ItemLeftLabelStyle.normal.textColor = TitleStyle.normal.textColor;
            ItemLeftLabelStyle.alignment = TextAnchor.MiddleLeft;

            ItemSelectedCenterStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
            ItemSelectedCenterStyle.normal.textColor = new Color(0, 0.5f, 0);

            ItemSelectedLeftStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
            ItemSelectedLeftStyle.normal.textColor = ItemSelectedCenterStyle.normal.textColor;
            ItemSelectedLeftStyle.alignment = TextAnchor.MiddleLeft;

            FoldoutStyle = new GUIStyle(EditorStyles.foldout);
            FoldoutStyle.fixedHeight = FoldoutStyle.fixedWidth = 0;
        }

        public static GUIStyle TitleStyle { get; private set; }
        public static GUIStyle ItemCenterLabelStyle { get; private set; }
        public static GUIStyle ItemLeftLabelStyle { get; private set; }
        public static GUIStyle ItemSelectedCenterStyle { get; private set; }
        public static GUIStyle ItemSelectedLeftStyle { get; private set; }
        public static GUIStyle FoldoutStyle { get; private set; }

        public static bool KeyPressed<T>(this T s, string controlName, KeyCode key, out T fieldValue)
        {
            fieldValue = s;
            if (GUI.GetNameOfFocusedControl() == controlName)
            {
                if ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == key))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
