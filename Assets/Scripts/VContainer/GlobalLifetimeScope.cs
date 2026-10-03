using System.Diagnostics;
using Jing.Data;
using Jing.Game.Loading;
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
            RegisterObj(builder);

        }

        private void Singleton(IContainerBuilder builder)
        {

            builder.Register<AddressableTools>(Lifetime.Singleton).As<IAddressableTools>();
            builder.Register<PageAssetProvider>(Lifetime.Singleton);
            builder.Register<LoadingService>(Lifetime.Singleton).As<ILoadingService>();

            builder.Register<Data_Uesr>(Lifetime.Singleton);



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