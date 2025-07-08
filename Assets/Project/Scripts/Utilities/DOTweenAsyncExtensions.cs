using System.Threading;
using DG.Tweening;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// Provides extension methods for converting DOTween animations to UniTask.
    /// </summary>
    public static class DOTweenAsyncExtensions
    {
        /// <summary>
        /// Converts a Tween to a UniTask.
        /// </summary>
        /// <param name="tween">The Tween to convert.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UniTask that completes when the Tween completes.</returns>
        public static UniTask ToUniTask(this Tween tween, CancellationToken cancellationToken = default)
        {
            if (!tween.IsActive())
            {
                return UniTask.CompletedTask;
            }

            var tcs = new UniTaskCompletionSource();
            
            // Register a callback for when the tween completes.
            tween.OnComplete(() => tcs.TrySetResult());
            
            // Register a callback for when the tween is killed.
            tween.OnKill(() => tcs.TrySetCanceled());

            // If a cancellation token is provided, register a callback to kill the tween.
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    tween.Kill();
                });
            }

            return tcs.Task;
        }
    }
}