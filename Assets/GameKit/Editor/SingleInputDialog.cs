using System;
using UnityEngine;
using UnityEditor;

public class SingleInputDialog : EditorWindow
{
    public delegate bool CheckErrorDelegate(string text);

    public static void Show(string textCaption, string defaultText, string buttonCaption,
        Action<string> OnButtonClick)
    {
        Show(textCaption, defaultText, buttonCaption, OnButtonClick, null);
    }

    public static void Show(string textCaption, string defaultText, string buttonCaption,
        Action<string> OnButtonClick, CheckErrorDelegate checkError)
    {
        SingleInputDialog dialog = ScriptableObject.CreateInstance<SingleInputDialog>();
        dialog._textCaption = textCaption;
        dialog._text = defaultText;
        dialog._buttonCaption = buttonCaption;
        dialog._onButtonClick = OnButtonClick;
        dialog._checkError = checkError;
        dialog.Show(true);
    }

    private void OnGUI()
    {
        _text = EditorGUILayout.TextField(_textCaption, _text);
        GUI.enabled = !string.IsNullOrEmpty(_text);
        if (GUILayout.Button(_buttonCaption))
        {
            OnClickButton();
        }
        GUI.enabled = true;
    }

    private void OnLostFocus()
    {
        Focus();
    }

    private void OnClickButton()
    {
        _text = _text.Trim();

        if (_checkError == null || !_checkError(_text))
        {
            if (string.IsNullOrEmpty(_text))
            {
                EditorUtility.DisplayDialog("Error", "Please specify a valid text.", "Close");
            }
            else
            {

                if (_onButtonClick != null)
                {
                    _onButtonClick(_text);
                }
            }
        }

        Close();
        GUIUtility.ExitGUI();
    }

    private string _textCaption = "Default";
    private string _buttonCaption = "OK";
    private Action<string> _onButtonClick;
    private CheckErrorDelegate _checkError;

    private string _text;
}