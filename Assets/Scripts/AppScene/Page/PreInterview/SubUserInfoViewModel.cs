using System;
using System.Collections.Generic;
using Jing.Data;
using Jing.Game.Services;
using UnityEngine;
using VContainer;

namespace Jing.Game.PreInterview
{
    public class SubUserInfoViewModel
    {
        #region ::: Action :::
        public event Action Action_DataChanged
        {
            add => userDataService.Action_DataChanged += value;
            remove => userDataService.Action_DataChanged -= value;
        }

        #endregion
        #region ::: Get Set :::
        public string Name
        {
            get => userDataService.UserData?.ResumeInfo.UserName ?? string.Empty;
        }
        public string Education
        {
            get => userDataService.UserData?.ResumeInfo.HighestEducation ?? string.Empty;
        }
        public string Experience
        {
            get => userDataService.UserData?.ResumeInfo.YearsOfExperience ?? string.Empty;
        }
        public string Skills
        {
            get => string.Join(", ", userDataService.UserData?.ResumeInfo.Skills ?? new List<string>());

        }
        public string ProjectHightlights
        {
            get => string.Join(", ", userDataService.UserData?.ResumeInfo.ProjectHighlights ?? new List<string>());
        }
        #endregion
        #region ::: Inject :::
        private IUserDataService userDataService;
        private IAIResume aiResume;


        [Inject]
        public void Construct(IUserDataService userDataService, IAIResume aiResume)
        {
            this.userDataService = userDataService;
            this.aiResume = aiResume;
        }

        #endregion
    }

}