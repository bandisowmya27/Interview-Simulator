using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jing.Data;
using Jing.Game.Loading;
using Jing.UI;
using Jing.UI.Popup;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AppScene : MonoBehaviour, IInitializable
{

    #region ::: Inject :::
    protected IObjectResolver resolver;
    protected UIManager uiManager;
    ILoadingService loading;

    [Inject]
    public virtual void Construct(IObjectResolver resolver, UIManager uiManager, ILoadingService loading)
    {
        this.resolver = resolver;
        this.uiManager = uiManager;
        this.loading = loading;
    }
    #endregion
    public void Initialize()
    {
        ImplementAsync().Forget();
    }

    private async UniTask ImplementAsync()
    {
        await Init();

        loading.Show();
        uiManager.ShowPage("StartApp").Forget();
        uiManager.ShowPage("LoginAndRegister").Forget();
        // uiManager.ShowPage("MainMenu", false).Forget();
        loading.Hide();
    }

    private async UniTask Init()
    {
        Canvas uiCanvas = transform.Find("UICanvas").GetComponent<Canvas>();
        Canvas popCanvas = transform.Find("PopupCanvas").GetComponent<Canvas>();
        foreach (Transform child in uiCanvas.transform)
        {
            Destroy(child.gameObject);
        }
        await uiManager.Init(uiCanvas, popCanvas);
    }

}
