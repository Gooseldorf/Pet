using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class TweenUtilities
{
    public static Tweener RotateAroundZ(Transform target, float angle, Vector3 pivot, float duration)
    {
        float currentAngle = 0f;

        return DOTween.To(
            () => currentAngle,
            x =>
            {
                float delta = x - currentAngle;
                currentAngle = x;

                // крутим объект вокруг pivot по оси Z
                target.RotateAround(pivot, Vector3.forward, delta);
            },
            angle,        // конечный угол
            duration    // время анимации
        );
    }

    public static Tweener DOScaleInOut(this Transform target, Vector3 initialScale, float scaleMultiplier, float duration, Ease easing = Ease.Linear)
    {
        return target.DOScale(initialScale * scaleMultiplier, duration / 2).SetEase(easing)
            .OnComplete(() => target.DOScale(initialScale, duration / 2).SetEase(easing));
    }

    public static UniTask WaitForCompletion(this Tweener tween)
    {
        /*if (tween == null) return UniTask.CompletedTask;*/
        if (!tween.IsActive() || tween.IsComplete()) 
            return UniTask.CompletedTask;

        return UniTask.WaitWhile(() => tween.IsActive() && !tween.IsComplete());
    }
}
