using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pet.UI
{
    public interface IBackHandler
    {
        UniTask<bool> TryHandleBackAsync(CancellationToken cancellation);
    }
}
