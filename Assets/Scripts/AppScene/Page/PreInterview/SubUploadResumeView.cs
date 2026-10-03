using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using TMPro;

using VContainer;

using Jing.UI;
using Jing.Data;
using Jing.UI.Popup;
using System;
using static ManagerAI.OPEN_AI;
using UnityEngine.PlayerLoop;

namespace Jing.Game.PreInterview
{
    public class SubUploadResumeView : UIBaseView
    {
        #region ::: Object From InitGetUI:::
        private Button btn_upload;
        private Button btn_remove_doc;
        private TMP_Text text_doc_name;
        #endregion

        #region ::: Inject :::
        private Data_Uesr data_uesr;
        private IUIManager uiManager;

        private SubUploadResumeViewModel vm;

        [Inject]
        public virtual void Construct(Data_Uesr data_uesr, IUIManager uiManager, SubUploadResumeViewModel vm)
        {
            this.data_uesr = data_uesr;
            this.uiManager = uiManager;
            this.vm = vm;
        }
        #endregion

        #region ::: Override :::

        protected override void InitGetUI()
        {
            base.InitGetUI();
            btn_upload = collector.GetUI<Button>("Button_Upload");
            btn_remove_doc = collector.GetUI<Button>("Button_Remove");
            text_doc_name = collector.GetUI<TMP_Text>("Text_DoctName");
        }

        protected override void InitSet()
        {
            base.InitSet();
            btn_remove_doc.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(data_uesr?.Resume?.OpenAI_FileId));
            text_doc_name.text = string.Empty;
        }

        protected override void AddListence()
        {
            base.AddListence();
            RemoveListence();
            btn_upload.onClick.AddListener(UploadResume);
            btn_remove_doc.onClick.AddListener(RemoveResume);
            vm.Action_OpenPopup += OpenPopup;
            vm.Action_UpdateFileName += ShowFileName;
        }

        protected override void Run()
        {
            base.Run();
            vm.InitGet();
            string file_name = vm.FileName;
            if (!string.IsNullOrEmpty(file_name))
            {
                ShowFileName(file_name);
            }
        }

        protected override void RemoveListence()
        {
            base.RemoveListence();
            btn_upload.onClick.RemoveListener(UploadResume);

            vm.Action_OpenPopup -= OpenPopup;

        }
        #endregion
        #region ::: Button OnClick :::
        private async void UploadResume()
        {
            loading.Show();
            await vm.UploadResume();
            loading.Hide();
        }

        private async void RemoveResume()
        {
            loading.Show();
            await vm.RemoveResume();
            loading.Hide();
        }

        #endregion

        #region ::: Private Methods :::
        private void ShowFileName(string name)
        {

            btn_remove_doc.transform.parent.gameObject.SetActive(true);
            text_doc_name.text = name;
        }
        #endregion
        #region ::: Popup :::

        private void OpenPopup(string content)
        {

            PopupBaseData data = new PopupBaseData()
            {
                Content = content
            };
            var confirmPopup = uiManager.OpenPopup<PopupCommonContentOnly>("Popup_Common_ContentOnly");
            confirmPopup.Show(content, () => { uiManager.ClosePopup(); });
        }
        #endregion
    }
}
