using VContainer;
using VContainer.Unity;


namespace Jing.VContainerSetting
{
    public class GlobalLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<KeepAlive>();

        }
    }
}