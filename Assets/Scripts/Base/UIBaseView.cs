using Jing.Game.Loading;
using UnityEngine;
using VContainer;

namespace Jing.UI
{
    [RequireComponent(typeof(UICollector))]
    public abstract class UIBaseView : MonoBehaviour
    {
        protected UICollector collector;

        #region ::: Inject :::
        protected ILoadingService loading;

        [Inject]
        public virtual void Construct(ILoadingService loading)
        {
            this.loading = loading;
        }
        #endregion
        public virtual void Show()
        {
            loading.Show();
            InitGetUI();
            InitSet();
            AddListence();
            Run();
            gameObject.SetActive(true);

            loading.Hide();
        }

        public virtual void Hide()
        {
            RemoveListence();
            gameObject.SetActive(false);
        }

        protected virtual void InitGetUI()
        {
            collector = GetComponent<UICollector>();
        }

        protected virtual void InitSet() { }

        protected virtual void AddListence() { }

        protected virtual void RemoveListence() { }

        protected virtual void Run() { }

    }
}