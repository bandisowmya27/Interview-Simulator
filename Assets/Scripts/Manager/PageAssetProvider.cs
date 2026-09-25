using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jing.Tools;
using UnityEngine;
using VContainer;

public class PageAssetProvider
{
    private readonly string preload_key_lable = "Preload_UI";
    private readonly string pop_ui_key_lable = "Popup_UI";

    public Dictionary<string, GameObject> uiObj;
    public Dictionary<string, GameObject> popObj;

    #region ::: Inject :::
    private IAddressableTools addressables;

    [Inject]
    public virtual void Construct(IAddressableTools addressables)
    {
        this.addressables = addressables;
    }
    #endregion

    /// <summary>
    /// Load All Preload_UI And Popup_UI;
    /// </summary>
    public async UniTask LoadFromAddressable()
    {
        IList<GameObject> main_results = await addressables.LoadAssets<GameObject>(preload_key_lable);
        IList<GameObject> pop_results = await addressables.LoadAssets<GameObject>(pop_ui_key_lable);

        uiObj = new Dictionary<string, GameObject>();
        foreach (var prefab in main_results)
        {
            uiObj.Add(prefab.name, prefab);
        }

        popObj = new Dictionary<string, GameObject>();
        foreach (var prefab in pop_results)
        {
            popObj.Add(prefab.name, prefab);
        }
    }

    public async UniTask<GameObject> LoadFromAddressableByPageName(string name)
    {
        GameObject results = await addressables.LoadAsset<GameObject>(name);
        uiObj.Add(results.name, results);
        return results;
    }

    public void ReleaseAddressableByPageName(string name)
    {
        if (uiObj.TryGetValue(name, out GameObject prefab))
        {
            addressables.Release(name);
            uiObj.Remove(name);
        }
    }


}
