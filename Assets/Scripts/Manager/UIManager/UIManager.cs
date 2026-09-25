using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Jing.UI
{
    public class UIManager : IUIManager
    {

        private Canvas mainCanvas;
        private Canvas popupCanvas;

        private Dictionary<string, GameObject> pageInst = new Dictionary<string, GameObject>(); // Instantiate

        private Dictionary<string, GameObject> popupInst = new Dictionary<string, GameObject>();    //Instantiate

        private readonly Stack<GameObject> pageHistory =
            new Stack<GameObject>();

        private GameObject currentPopup;

        private bool initialized;


        #region ::: Inject :::
        private IObjectResolver resolver;
        private PageAssetProvider pageAssetProvider;

        [Inject]
        public virtual void Construct(IObjectResolver resolver, PageAssetProvider pageAssetProvider)
        {
            this.resolver = resolver;
            this.pageAssetProvider = pageAssetProvider;
        }
        #endregion

        #region Public Methods

        /// <reminder>
        /// called from main scene
        /// </reminder>
        public async UniTask Init(Canvas mainCanvas, Canvas popupCanvas)
        {
            if (mainCanvas == null || popupCanvas == null)
            {
                Debug.LogError("mainCanvas and popupCanvas must not be null.");
            }

            if (initialized) { CloseAll(); }

            this.mainCanvas = mainCanvas;
            this.popupCanvas = popupCanvas;
            await pageAssetProvider.LoadFromAddressable();

            initialized = true;
        }

        public async UniTask ShowPage(string pageName)
        {
            EnsureInitialized();

            GameObject page = await GetOrCreatePage(pageName);

            if (page == null)
            {
                Debug.LogError("Can not found page, name is" + pageName);
                return;
            }

            OpenPage(page);
            pageHistory.Push(page);

        }

        public async UniTask ShowPageAndHideCurrent(string pageName)
        {
            EnsureInitialized();

            GameObject page = await GetOrCreatePage(pageName);

            if (page == null)
            {
                Debug.LogError("Can not found page, name is" + pageName);
                return;
            }
            if (pageHistory.Count > 0)
            {
                ClosePage(pageHistory.Pop());
            }

            OpenPage(page);
            pageHistory.Push(page);

        }

        public async UniTask ShowPageAndReleaseCurrent(string pageName)
        {
            EnsureInitialized();

            GameObject page = await GetOrCreatePage(pageName);

            if (page == null)
            {
                Debug.LogError("Can not found page, name is" + pageName);
                return;
            }
            if (pageHistory.Count > 0)
            {
                GameObject current = pageHistory.Pop();
                ClosePage(current);
                pageAssetProvider.ReleaseAddressableByPageName(current.name);
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

        public T OpenPopup<T>(string popupName) where T : UIBaseView
        {
            EnsureInitialized();

            GameObject popup = GetOrCreatePopup(popupName);

            if (popup == null)
            {
                Debug.LogError("Can not found popup, name is: " + popupName);
                return null;
            }

            if (currentPopup != popup)
            {
                ClosePopup();
                currentPopup = popup;
            }

            OpenPage(popup);
            popup.transform.SetAsLastSibling();

            return popup.GetComponent<T>();
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
            //Only Page
            //Do something when the scene changes
            foreach (var obj in pageInst)
            {
                if (obj.Value != null)
                {
                    ClosePage(obj.Value);
                }
            }
            pageInst.Clear();

        }

        #endregion

        #region Create Methods

        private async UniTask<GameObject> GetOrCreatePage(string pageName)
        {
            if (pageInst.TryGetValue(pageName, out GameObject existingView))
            {
                if (existingView != null)
                {
                    return existingView;
                }
            }
            if (!pageAssetProvider.uiObj.TryGetValue(pageName, out GameObject prefab))
            {
                prefab = await pageAssetProvider.LoadFromAddressableByPageName(pageName);
                if (prefab == null)
                {
                    return null;
                }
            }

            GameObject obj = GameObject.Instantiate(prefab, mainCanvas.transform);
            obj.name = pageName;
            obj.SetActive(false);
            resolver.InjectGameObject(obj);
            pageInst.Add(pageName, obj);
            return obj;
        }

        private GameObject GetOrCreatePopup(string popupName)
        {
            if (popupInst.TryGetValue(popupName, out GameObject existingView))
            {
                if (existingView != null)
                {
                    return existingView;
                }
            }
            if (!pageAssetProvider.popObj.TryGetValue(popupName, out GameObject prefab))
            {
                Debug.LogError($"UIManager Error : Popup Object cannot found, name is {popupName}");
                return null;
            }

            GameObject obj = GameObject.Instantiate(prefab, popupCanvas.transform);
            obj.name = popupName;
            obj.SetActive(false);
            resolver.InjectGameObject(obj);
            popupInst.Add(popupName, obj);
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

            UIBaseView[] views = page.GetComponentsInChildren<UIBaseView>(true);

            foreach (UIBaseView view in views)
            {
                Debug.Log(view.name);
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