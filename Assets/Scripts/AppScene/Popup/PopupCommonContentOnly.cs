using System;
using Jing.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Jing.UI.Popup
{
    public class PopupCommonContentOnly : UIBaseView
    {
        #region ::: Object From InitGetUI:::
        private Button btn_close;
        private TMP_Text text_content;
        #endregion

        private Action action_close;
        public void Show(string content, Action action_close)
        {
            base.Show();
            text_content.text = content;
            this.action_close = action_close;
        }

        protected override void InitGetUI()
        {
            base.InitGetUI();
            Debug.Log(collector);
            btn_close = collector.GetUI<Button>("Button_Close");
            text_content = collector.GetUI<TMP_Text>("Text_Content");
        }

        protected override void AddListence()
        {
            base.AddListence();
            btn_close.onClick.AddListener(BtnClose);
        }

        #region ::: Button Click :::
        private void BtnClose()
        {
            action_close?.Invoke();
        }
        #endregion

    }
}
