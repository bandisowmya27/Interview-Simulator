using Jing.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.Game.PreInterview
{
    public class SubUploadResumeView : UIBaseView
    {
        private Button btn_upload;
        private Transform parent;

        protected override void InitGetUI()
        {
            base.InitGetUI();
            btn_upload = collector.GetUI<Button>("Button_Upload");
            parent = collector.GetUI<Transform>("DoctParent");
        }
    }
}
