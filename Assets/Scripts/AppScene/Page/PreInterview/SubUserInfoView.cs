using System;
using Jing.Data;
using Jing.Game.Services;
using Jing.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Jing.Game.PreInterview
{
    public class SubUserInfoView : UIBaseView
    {
        #region ::: Object From InitGetUI:::
        [HideInInspector] public TMP_InputField input_name;
        [HideInInspector] public TMP_InputField input_education;
        [HideInInspector] public TMP_InputField input_experience;
        [HideInInspector] public TMP_InputField input_skills;
        [HideInInspector] public TMP_InputField input_project_hightlights;

        private Button button_editName;
        private Button button_editEducation;
        private Button button_editExperience;
        private Button button_editSkills;
        private Button button_editProjectHightlights;
        #endregion

        #region ::: Inject :::
        private IUserDataService userDataService;
        private IUIManager uiManager;

        private SubUserInfoViewModel vm;

        [Inject]
        public virtual void Construct(IUserDataService userDataService, IUIManager uiManager, SubUserInfoViewModel vm)
        {
            this.userDataService = userDataService;
            this.uiManager = uiManager;
            this.vm = vm;
        }
        #endregion

        #region ::: Override :::

        protected override void InitGetUI()
        {
            base.InitGetUI();
            input_name = collector.GetUI<TMP_InputField>("Input_Name");
            input_education = collector.GetUI<TMP_InputField>("Input_Education");
            input_experience = collector.GetUI<TMP_InputField>("Input_Experience");
            input_skills = collector.GetUI<TMP_InputField>("Input_Skills");
            input_project_hightlights = collector.GetUI<TMP_InputField>("Input_ProjectHightlights ");

            button_editName = collector.GetUI<Button>("Button_EditName");
            button_editEducation = collector.GetUI<Button>("Button_EditEducation");
            button_editExperience = collector.GetUI<Button>("Button_EditExperience");
            button_editSkills = collector.GetUI<Button>("Button_EditSkills");
            button_editProjectHightlights = collector.GetUI<Button>("Button_EditProjectHightlights");
        }

        protected override void InitSet()
        {
            base.InitSet();
            UpdateData();
        }

        protected override void AddListence()
        {
            RemoveListence();
            base.AddListence();
            vm.Action_DataChanged += DataChanged;
            button_editName.onClick.AddListener(BtnEditName);
            button_editEducation.onClick.AddListener(BtnEditEducation);
            button_editExperience.onClick.AddListener(BtnEditExperience);
            button_editSkills.onClick.AddListener(BtnEditSkills);
            button_editProjectHightlights.onClick.AddListener(BtnEditProjectHightlights);
        }

        protected override void Run()
        {
            base.Run();
        }

        protected override void RemoveListence()
        {
            base.RemoveListence();
            vm.Action_DataChanged -= DataChanged;
            button_editName.onClick.RemoveListener(BtnEditName);
            button_editEducation.onClick.RemoveListener(BtnEditEducation);
            button_editExperience.onClick.RemoveListener(BtnEditExperience);
            button_editSkills.onClick.RemoveListener(BtnEditSkills);
            button_editProjectHightlights.onClick.RemoveListener(BtnEditProjectHightlights);
        }
        #endregion
        #region ::: Action Function :::
        public void DataChanged()
        {
            UpdateData();
        }
        #endregion
        #region ::: Button OnClick :::
        private void BtnEditName()
        {
            OpenPopup_EditContent("", vm.Name, (text) => { input_name.name = text; });
        }
        private void BtnEditEducation()
        {
            OpenPopup_EditContent("", vm.Education, (text) => { input_education.name = text; });
        }
        private void BtnEditExperience()
        {
            OpenPopup_EditContent("", vm.Experience, (text) => { input_experience.name = text; });
        }
        private void BtnEditSkills()
        {
            OpenPopup_EditContent("", vm.Skills, (text) => { input_skills.name = text; });
        }
        private void BtnEditProjectHightlights()
        {
            OpenPopup_EditContent("", vm.ProjectHightlights, (text) => { input_project_hightlights.name = text; });
        }
        #endregion
        #region ::: Private Methods :::
        private void UpdateData()
        {
            input_name.text = vm.Name;
            input_education.text = vm.Education;
            input_experience.text = vm.Experience;
            input_skills.text = vm.Skills;
            input_project_hightlights.text = vm.ProjectHightlights;
        }

        #endregion

        #region ::: Popup :::

        private void OpenPopup_EditContent(string title, string content, Action<string> onSave)
        {

            PopupBaseData data = new PopupBaseData()
            {
                Content = content
            };
            var confirmPopup = uiManager.OpenPopup<PopupCommonInput>("Popup_Input");
            confirmPopup.Show(
                title,
                content,
                onSave,
                () => { uiManager.ClosePopup(); });

        }

        #endregion


    }
}