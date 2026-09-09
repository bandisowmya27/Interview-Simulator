using UnityEngine;

namespace Jing.UI
{
    [RequireComponent(typeof(UICollector))]
    public abstract class UIBaseView : MonoBehaviour
    {
        protected UICollector collector;

        public virtual void Show()
        {
            InitGetUI();
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {

            gameObject.SetActive(false);
        }

        protected virtual void InitGetUI()
        {
            collector = GetComponent<UICollector>();
        }


    }
}