using System;
using UnityEngine;
using UnityEditor;

public class SingleInputDialog : EditorWindow
{
    public delegate bool CheckErrorDelegate(string text);

    public static void Show(string textCaption, string buttonCaption,
        Action<string> OnButtonClick)
    {
        Show(textCaption, buttonCaption, OnButtonClick, null);
    }

    public static void Show(string textCaption, string buttonCaption,
        Action<string> OnButtonClick, CheckErrorDelegate checkError)
    {
        SingleInputDialog dialog = EditorWindow.GetWindow<SingleInputDialog>("Input Dialog");
        dialog._textCaption = textCaption;
        dialog._buttonCaption = buttonCaption;
        dialog._onButtonClick = OnButtonClick;
        dialog._checkError = checkError;
        dialog.Show();
    }

    private void OnGUI()
    {
        _text = EditorGUILayout.TextField(_textCaption, _text);

        if (GUILayout.Button(_buttonCaption))
        {
            OnClickButton();
        }
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