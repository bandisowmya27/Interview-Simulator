using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using TMPro;

using VContainer;

using Jing.UI;
using Jing.Data;
using Jing.UI.Popup;

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

        [Inject]
        public virtual void Construct(Data_Uesr data_uesr, IUIManager uiManager)
        {
            this.data_uesr = data_uesr;
            this.uiManager = uiManager;
        }
        #endregion
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
            btn_remove_doc.transform.parent.gameObject.SetActive(data_uesr.Resume != null);
            text_doc_name.text = (data_uesr?.Resume != null) ? data_uesr.Resume.Resume_Name : string.Empty;
        }

        protected override void AddListence()
        {
            base.AddListence();
            UnityEngine.Debug.Log("AddListence");
            btn_upload.onClick.AddListener(UploadResume);
        }

        #region ::: Button OnClick :::
        private void UploadResume()
        {
            ImplementAsync().Forget();

            async UniTask ImplementAsync()
            {
                loading.Show();
                (string file_name, byte[] bytes, string error) = await OpenFile.Open();
                if (!ConfirmOpenFile(error)) { return; }


                loading.Hide();
            }


            bool ConfirmOpenFile(string error)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    if (error != "Operation canceled: No file was selected.")
                    {
                        PopupBaseData data = new PopupBaseData()
                        {
                            Content = error
                        };
                        var confirmPopup = uiManager.OpenPopup<PopupCommonContentOnly>("Popup_Common_ContentOnly");
                        confirmPopup.Show(error, () => { uiManager.ClosePopup(); });
                    }
                    return false;
                }
                return true;
            }
        }
        #endregion

    }
}
