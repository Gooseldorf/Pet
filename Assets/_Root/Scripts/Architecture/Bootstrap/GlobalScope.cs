using Architecture.Bootstrap;
using Architecture.SceneLoading;
using VContainer;
using VContainer.Unity;

public class GlobalScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SceneLoader>(Lifetime.Singleton);
        
        builder.RegisterEntryPoint<Bootstrap>();
    }
}
