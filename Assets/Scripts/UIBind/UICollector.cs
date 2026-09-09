using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jing.UI
{
    [Serializable]
    public class UIElementBinding
    {
        public string key;
        public GameObject obj;
    }

    public class UICollector : MonoBehaviour
    {
        public List<UIElementBinding> bindings =
            new List<UIElementBinding>();

        public GameObject GetUI(string key)
        {
            UIElementBinding binding = bindings.Find(item =>
                    item != null &&
                    item.key == key);

            if (binding == null)
            {
                Debug.LogError($"ERROR! UI key not found: {key}");
                return null;
            }

            if (binding.obj == null)
            {
                Debug.LogError($"ERROR! UI object is null. Key: {key}");
                return null;
            }
            return binding.obj;
        }

        public T GetUI<T>(string key)
            where T : Component
        {
            GameObject obj = GetUI(key);

            if (obj == null)
            {
                return null;
            }

            if (!obj.TryGetComponent<T>(out T component))
            {
                Debug.LogError(
                    $"UI object '{key}' does not contain " +
                    $"{typeof(T).Name}.");
                return null;
            }
            return component;
        }
    }
}