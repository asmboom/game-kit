using UnityEngine;
using System;

namespace Beetle23
{
    public interface IStorage
    {
        string GetString(string key, string defaultValue = "");
        void SetString(string key, string value);
        float GetFloat(string key, float defaultValue = 0);
        void SetFloat(string key, float value);
        int GetInt(string key, int defaultValue = 0);
        void SetInt(string key, int value);
        bool GetBool(string key, bool defaultValue = false);
        void SetBool(string key, bool value);
    }
}
