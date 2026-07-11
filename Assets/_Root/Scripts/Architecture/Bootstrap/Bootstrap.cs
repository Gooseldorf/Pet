using System.Threading;
using Architecture.SceneLoading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Architecture.Bootstrap
{
    public class Bootstrap : IAsyncStartable
    {
        private readonly SceneLoader sceneLoader;

        public Bootstrap(SceneLoader sceneLoader)
        {
            this.sceneLoader = sceneLoader;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            await sceneLoader.LoadAdditiveAsync("MainMenu", cancellation);
        }
    }
}
