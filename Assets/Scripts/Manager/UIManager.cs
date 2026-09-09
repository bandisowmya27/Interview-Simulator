using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Jing.UI
{
    public class UIManager : IUIManager
    {
        private readonly IObjectResolver resolver;

        private Canvas mainCanvas;
        private Canvas popupCanvas;

        // Prefab 設定
        private Dictionary<string, GameObject> pagePrefabs = new Dictionary<string, GameObject>();

        private Dictionary<string, GameObject> popupPrefabs = new Dictionary<string, GameObject>();

        // Runtime 建立出來的 UI
        private Dictionary<string, GameObject> pageInst = new Dictionary<string, GameObject>();

        private Dictionary<string, GameObject> popupInst = new Dictionary<string, GameObject>();

        private readonly Stack<GameObject> pageHistory =
            new Stack<GameObject>();

        private GameObject currentPopup;

        private bool initialized;

        public UIManager(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        #region Public Methods

        /// <reminder>
        /// called from main scene
        /// </reminder>
        public void Initialize(
            Canvas mainCanvas,
            Canvas popupCanvas,
            Dictionary<string, GameObject> pagePrefabs,
            Dictionary<string, GameObject> popupPrefabs)
        {
            if (mainCanvas == null)
                throw new ArgumentNullException(nameof(mainCanvas));

            if (popupCanvas == null)
                throw new ArgumentNullException(nameof(popupCanvas));

            if (initialized)
            {
                CloseAll();
            }

            this.mainCanvas = mainCanvas;
            this.popupCanvas = popupCanvas;

            // 複製一份，避免外部修改原本的 Dictionary
            this.pagePrefabs =
                pagePrefabs != null
                    ? new Dictionary<string, GameObject>(pagePrefabs)
                    : new Dictionary<string, GameObject>();

            this.popupPrefabs =
                popupPrefabs != null
                    ? new Dictionary<string, GameObject>(popupPrefabs)
                    : new Dictionary<string, GameObject>();

            initialized = true;
        }

        public void ShowPage(string pageName, bool hideCurrent = true)
        {
            EnsureInitialized();

            GameObject page = GetOrCreatePage(pageName);

            if (page == null)
            {
                Debug.LogError("Can not found page, name is" + pageName);
                return;
            }

            if (pageHistory.Count > 0 && pageHistory.Peek() == page)
            {
                OpenPage(page);
                return;
            }

            if (hideCurrent && pageHistory.Count > 0)
            {
                ClosePage(pageHistory.Peek());
            }

            OpenPage(page);
            pageHistory.Push(page);
        }

        public void ClosePage()
        {
            EnsureInitialized();

            if (pageHistory.Count <= 1) { return; }

            GameObject currentPage = pageHistory.Pop();
            ClosePage(currentPage);

            GameObject previousPage = pageHistory.Peek();
            OpenPage(previousPage);
        }



        public void OpenPopup(string popupName)
        {
            EnsureInitialized();

            GameObject popup = GetOrCreatePopup(popupName);

            if (popup == null)
            {
                Debug.LogError("Can not found popup, name is" + popupName);
                return;
            }

            if (currentPopup == popup)
            {
                OpenPage(popup);
                popup.transform.SetAsLastSibling();
                return;
            }

            ClosePopup();

            OpenPage(popup);
            popup.transform.SetAsLastSibling();
            currentPopup = popup;
        }

        public void ClosePopup()
        {
            if (currentPopup == null)
            {
                return;
            }

            ClosePage(currentPopup);
            currentPopup = null;
        }

        public void CloseAll()
        {
            //Do something when the scene changes
        }

        #endregion

        #region Create Methods

        private GameObject GetOrCreatePage(string pageName)
        {
            return GetOrCreateView(
                pageName,
                pagePrefabs,
                pageInst,
                mainCanvas.transform
            );
        }

        private GameObject GetOrCreatePopup(string popupName)
        {
            return GetOrCreateView(
                popupName,
                popupPrefabs,
                popupInst,
                popupCanvas.transform
            );
        }

        private GameObject GetOrCreateView(string viewName, Dictionary<string, GameObject> prefabDict, Dictionary<string, GameObject> instanceDict, Transform parent)
        {
            if (instanceDict.TryGetValue(viewName, out GameObject existingView))
            {
                if (existingView != null)
                {
                    return existingView;
                }
            }

            if (!prefabDict.TryGetValue(viewName, out GameObject prefab))
            {
                Debug.LogWarning($"UIManager Error! {viewName} not found ");
                return null;
            }

            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.name = viewName;
            obj.SetActive(false);

            instanceDict.Add(viewName, obj);

            return obj;
        }

        #endregion

        #region View Methods

        private void OpenPage(GameObject page)
        {
            if (page == null)
            {
                return;
            }

            page.SetActive(true);

            UIBaseView[] views = page.GetComponents<UIBaseView>();

            foreach (UIBaseView view in views)
            {
                if (view != null)
                {
                    view.Show();
                }
            }
        }

        private void ClosePage(GameObject page)
        {
            if (page == null)
            {
                return;
            }

            UIBaseView[] views = page.GetComponents<UIBaseView>();

            foreach (UIBaseView view in views)
            {
                if (view != null)
                {
                    view.Hide();
                }
            }
            page.SetActive(false);
        }

        #endregion

        private void EnsureInitialized()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("ERROR! UIManager has not been initialized.");
            }
        }
    }
}