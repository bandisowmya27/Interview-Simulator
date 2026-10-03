using Cysharp.Threading.Tasks;
using Jing.Game.Loading;
using Jing.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Jing.LoginAndRegister
{

    public class LoginAndRegisterView : UIBaseView
    {
        #region ::: Object From InitGetUI:::
        private TMP_InputField input_account;
        private TMP_InputField input_password;
        private Button btn_register;
        private Button btn_login;
        private Button btn_guest;
        private TMP_Text text_note;
        #endregion

        #region ::: Inject :::
        private UIManager uiManager;
        private LoginAndRegisterViewModel vm;

        [Inject]
        public virtual void Construct(UIManager uiManager, LoginAndRegisterViewModel vm)
        {
            this.uiManager = uiManager;
            this.vm = vm;
        }
        #endregion
        protected override void InitGetUI()
        {
            base.InitGetUI();
            input_account = collector.GetUI<TMP_InputField>("Input_Account");
            input_password = collector.GetUI<TMP_InputField>("Input_Password");
            btn_register = collector.GetUI<Button>("Button_Register");
            btn_login = collector.GetUI<Button>("Button_Login");
            btn_guest = collector.GetUI<Button>("Button_Guest");
            text_note = collector.GetUI<TMP_Text>("Text_Note");
        }
        protected override void InitSet()
        {
            base.InitSet();
            input_account.text = string.Empty;
            input_password.text = string.Empty;
            text_note.text = string.Empty;

        }
        protected override void AddListence()
        {
            RemoveListence();
            base.AddListence();

            btn_register.onClick.AddListener(Register);
            btn_login.onClick.AddListener(Login);
            btn_guest.onClick.AddListener(GuestUsed);

            vm.Action_ErrorMsg += Action_ErrorMsg;


        }

        protected override void RemoveListence()
        {
            base.RemoveListence();
            btn_register.onClick.RemoveListener(Register);
            btn_login.onClick.RemoveListener(Login);
            btn_guest.onClick.RemoveListener(GuestUsed);

            vm.Action_ErrorMsg += Action_ErrorMsg;

        }

        protected override void Run()
        {

        }

        #region ::: Button OnClick :::
        private async void Register()
        {
            loading.Show();

            if (input_account.text == string.Empty || input_password.text == string.Empty)
            {
                text_note.text = "Error: Username and password are required.";
                return;
            }
            bool success = await vm.Register(input_account.text, input_password.text);
            if (success)
            {
                Login();
            }
            loading.Hide();
        }
        private async void Login()
        {
            loading.Show();
            if (input_account.text == string.Empty || input_password.text == string.Empty)
            {
                text_note.text = $"Error: Username and password are required.";
                return;
            }
            bool success = await vm.Login(input_account.text, input_password.text);
            if (success)
            {
                JumpToMainMenu();
            }
            loading.Hide();
        }
        private void GuestUsed()
        {
            JumpToMainMenu();
        }
        #endregion
        #region ::: Action Function ::: 
        //If msg is string.Empty, it means registration is successful.
        private void Action_ErrorMsg(string msg)
        {
            text_note.text = msg;
        }
        #endregion

        #region ::: Private Methods :::
        private void JumpToMainMenu()
        {
            loading.Show();
            uiManager.ShowPageAndReleaseCurrent("MainMenu").Forget();
            loading.Hide();
            InitSet();
        }
        #endregion

    }
}
