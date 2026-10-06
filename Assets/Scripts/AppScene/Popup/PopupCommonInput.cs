using System;
using Jing.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCommonInput : UIBaseView
{
    #region ::: Object From InitGetUI:::
    private Button btn_close;
    private Button btn_save;
    private TMP_Text text_title;
    private TMP_InputField input_content;
    #endregion

    private Action<string> OnAction_Save;
    private Action OnAction_Close;
    public void Show(string title, string content, Action<string> onSave, Action onClose)
    {
        base.Show();
        text_title.text = title;
        input_content.text = content;
        OnAction_Save = onSave;
        OnAction_Close = onClose;

    }
    #region ::: Override :::
    protected override void InitGetUI()
    {
        base.InitGetUI();

        btn_close = collector.GetUI<Button>("Button_Close");
        btn_save = collector.GetUI<Button>("Button_Save");
        text_title = collector.GetUI<TMP_Text>("Text_Title");
        input_content = collector.GetUI<TMP_InputField>("Input_Content");
    }
    protected override void InitSet()
    {
        text_title.text = string.Empty;
        input_content.text = string.Empty;
    }

    protected override void AddListence()
    {
        base.AddListence();
        btn_close.onClick.AddListener(BtnClose);
        btn_save.onClick.AddListener(BtnSave);
    }

    protected override void RemoveListence()
    {
        base.RemoveListence();
        btn_close.onClick.RemoveListener(BtnClose);
        btn_save.onClick.RemoveListener(BtnSave);
    }
    #endregion

    #region ::: Button Click:::
    private void BtnClose()
    {
        OnAction_Close?.Invoke();
    }

    private void BtnSave()
    {
        OnAction_Save?.Invoke(input_content.text);
    }
    #endregion


}
