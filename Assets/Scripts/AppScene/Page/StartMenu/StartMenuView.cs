using Cysharp.Threading.Tasks;
using Jing.Data;
using Jing.Game.Loading;
using Jing.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Jing.StartMenu
{
    public class StartMenuView : UIBaseView
    {
        #region ::: Object From InitGetUI:::
        private Button btn_setup;
        private Button btn_start;
        private Button btn_privacy;
        #endregion

        #region ::: Inject :::
        private UIManager uiManager;
        private Data_Uesr data_uesr;

        [Inject]
        public virtual void Construct(UIManager uiManager, Data_Uesr data_uesr)
        {
            this.uiManager = uiManager;
            this.data_uesr = data_uesr;
        }
        #endregion
        protected override void InitGetUI()
        {
            base.InitGetUI();
            btn_setup = collector.GetUI<Button>("Button_Setup");
            btn_start = collector.GetUI<Button>("Button_Start");
            btn_privacy = collector.GetUI<Button>("Button_Privacy");
        }

        protected override void InitSet()
        {
            base.InitSet();
            btn_start.interactable = (data_uesr.Resume != null);
        }

        protected override void AddListence()
        {
            base.AddListence();
            btn_setup.onClick.AddListener(SetPreInterview);
            btn_start.onClick.AddListener(Startinterview);
            btn_privacy.onClick.AddListener(ShowPrivacy);
        }

        #region ::: Button OnClick :::
        private void SetPreInterview()
        {
            loading.Show();
            uiManager.ShowPageAndHideCurrent("PreInterviewPage").Forget();
            loading.Hide();
        }
        private void Startinterview()
        {

        }
        private void ShowPrivacy()
        {

        }
        #endregion

    }
}
