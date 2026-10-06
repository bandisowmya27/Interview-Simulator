using Cysharp.Threading.Tasks;
using Jing.Data;
using Jing.Game.Loading;
using Jing.Game.Services;
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
        private IUIManager uiManager;
        private IUserDataService userDataService;

        [Inject]
        public virtual void Construct(IUIManager uiManager, IUserDataService userDataService)
        {
            this.uiManager = uiManager;
            this.userDataService = userDataService;
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
            btn_start.interactable = (!string.IsNullOrEmpty(userDataService?.UserData?.ResumeUpload?.OpenAI_FileId));
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
