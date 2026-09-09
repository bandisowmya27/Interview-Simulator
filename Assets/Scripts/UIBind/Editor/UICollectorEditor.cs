using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jing.UI
{
    [CustomEditor(typeof(UICollector))]
    public class UICollectorEditor : Editor
    {
        #region ::: Public Methods :::
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UICollector panel = (UICollector)target;

            if (GUILayout.Button("Auto Binding"))
            {
                AutoBinding(panel);
                Repaint();
            }
        }
        #endregion

        #region ::: Private Methods :::
        private void AutoBinding(UICollector panel)
        {
            Undo.RecordObject(
                panel,
                "Auto Binding UI"
            );

            panel.bindings.Clear();

            HashSet<string> usedKeys =
                new HashSet<string>();

            UIBind[] binds =
                panel.GetComponentsInChildren<UIBind>(true);

            foreach (UIBind bind in binds)
            {
                if (bind.transform == panel.transform)
                {
                    continue;
                }

                if (FindOwnerCollector(bind.transform) != panel)
                {
                    continue;
                }

                AddBinding(
                    panel,
                    bind.gameObject,
                    bind,
                    usedKeys
                );
            }

            UICollector[] childCollectors =
                panel.GetComponentsInChildren<UICollector>(true);

            foreach (UICollector childCollector in childCollectors)
            {
                if (childCollector == panel)
                {
                    continue;
                }

                if (FindOwnerCollector(
                        childCollector.transform.parent) != panel)
                {
                    continue;
                }

                UIBind bind =
                    childCollector.GetComponent<UIBind>();

                AddBinding(
                    panel,
                    childCollector.gameObject,
                    bind,
                    usedKeys
                );
            }

            EditorUtility.SetDirty(panel);

            if (PrefabUtility.IsPartOfPrefabInstance(panel))
            {
                PrefabUtility
                    .RecordPrefabInstancePropertyModifications(
                        panel
                    );
            }
        }

        private UICollector FindOwnerCollector(Transform current)
        {
            while (current != null)
            {
                UICollector collector =
                    current.GetComponent<UICollector>();

                if (collector != null)
                    return collector;

                current = current.parent;
            }

            return null;
        }

        private void AddBinding(UICollector panel, GameObject obj, UIBind bind, HashSet<string> usedKeys)
        {
            string key;

            if (bind != null &&
                !string.IsNullOrEmpty(bind.KeyName))
            {
                key = bind.KeyName;
            }
            else
            {
                key = obj.name;
            }

            if (!usedKeys.Add(key))
            {
                Debug.LogWarning(
                    $"Duplicate UI key: {key}",
                    obj
                );

                return;
            }

            panel.bindings.Add(
                new UIElementBinding
                {
                    key = key,
                    obj = obj
                }
            );
        }
        #endregion
    }
}