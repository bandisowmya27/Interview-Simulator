using VContainer;
using VContainer.Unity;

public class APPLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<AppScene>().As<IInitializable>();

    }
}
