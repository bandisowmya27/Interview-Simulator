using System.Diagnostics;
using Jing.Data;
using Jing.Game.Loading;
using Jing.Game.PlayFab;
using Jing.Game.Services;
using Jing.Tools;
using Jing.UI;
using VContainer;
using VContainer.Unity;


namespace Jing.VContainerSetting
{
    public class GlobalLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<KeepAlive>();
            Singleton(builder);
            RegisterData(builder);
            RegisterServices(builder);
            RegisterObj(builder);

        }

        private void Singleton(IContainerBuilder builder)
        {
            builder.Register<AddressableTools>(Lifetime.Singleton).As<IAddressableTools>();
            builder.Register<PageAssetProvider>(Lifetime.Singleton);
            builder.Register<LoadingService>(Lifetime.Singleton).As<ILoadingService>();
        }
        private void RegisterData(IContainerBuilder builder)
        {
            builder.Register<Data_Uesr>(Lifetime.Singleton);

        }
        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<UserDataService>(Lifetime.Singleton).As<IUserDataService>();
            builder.Register<PlayFabService>(Lifetime.Singleton).As<IPlayFabService>();

        }
        private void RegisterObj(IContainerBuilder builder)
        {
            LoadingView loadingView = transform.parent.GetComponentInChildren<LoadingView>(true);
            if (loadingView != null)
            {

                builder.RegisterComponent(loadingView).As<IInitializable>();

            }
            else
            {
                UnityEngine.Debug.Log("NULL");
            }
        }
    }
}