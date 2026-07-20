using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pet
{
    public interface ISceneEntryPoint
    {
        UniTask InitializeAsync(CancellationToken cancellation);
    }
}
