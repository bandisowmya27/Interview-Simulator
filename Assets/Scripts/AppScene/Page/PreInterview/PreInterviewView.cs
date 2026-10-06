using System.Collections.Generic;
using System.Linq;
using Jing.Game.Services;
using Jing.UI;
using UnityEngine.UI;
using VContainer;

namespace Jing.Game.PreInterview
{
    public class PreInterviewView : UIBaseView
    {
        #region ::: Object From InitGetUI:::
        private Button btn_save;
        private SubUserInfoView subpage_userInfo;

        #endregion

        #region ::: Inject :::
        private IUserDataService userDataService;
        private IUIManager uiManager;

        [Inject]
        public virtual void Construct(IUserDataService userDataService, IUIManager uiManager)
        {
            this.userDataService = userDataService;
            this.uiManager = uiManager;
        }
        #endregion

        #region ::: Override :::
        protected override void InitGetUI()
        {
            base.InitGetUI();
            btn_save = collector.GetUI<Button>("Button_Save");
            subpage_userInfo = collector.GetUI<SubUserInfoView>("Sub_UserInfo");
        }
        protected override void AddListence()
        {
            btn_save.onClick.AddListener(BtnSave);
        }
        #endregion

        #region ::: Button :::
        private void BtnSave()
        {
            userDataService.UpdateResumeContentInfo(new Data.Data_ResumeInfomation()
            {
                UserName = subpage_userInfo.input_name.text,
                HighestEducation = subpage_userInfo.input_education.text,
                YearsOfExperience = subpage_userInfo.input_experience.text,
                Skills = subpage_userInfo.input_skills.text.Split(',').ToList(),
                ProjectHighlights = subpage_userInfo.input_project_hightlights.text.Split(',').ToList()
            });
            uiManager.ShowPageAndHideCurrent("MainMenu");
        }
        #endregion


    }
}
