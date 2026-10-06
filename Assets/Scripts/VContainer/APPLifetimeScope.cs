using Jing.Game.PreInterview;
using Jing.LoginAndRegister;
using Jing.UI;
using VContainer;
using VContainer.Unity;

public class APPLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<AppScene>().As<IInitializable>();
        Singleton(builder);

        builder.RegisterComponentInHierarchy<TestUse>();


    }

    private void Singleton(IContainerBuilder builder)
    {
        builder.Register<UIManager>(Lifetime.Singleton).As<IUIManager>().AsSelf(); ;
        builder.Register<AIResume>(Lifetime.Singleton).As<IAIResume>();
        builder.Register<LoginAndRegisterViewModel>(Lifetime.Singleton);
        builder.Register<SubUploadResumeViewModel>(Lifetime.Singleton);
        builder.Register<SubUserInfoViewModel>(Lifetime.Singleton);


    }
}
