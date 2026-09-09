using Jing.UI;
using UnityEngine.UI;

namespace Jing.Game.PreInterview
{
    public class PreInterviewView : UIBaseView
    {
        private Button btn_save;

        protected override void InitGetUI()
        {
            base.InitGetUI();
            btn_save = collector.GetUI<Button>("Button_Save");

        }
    }
}
