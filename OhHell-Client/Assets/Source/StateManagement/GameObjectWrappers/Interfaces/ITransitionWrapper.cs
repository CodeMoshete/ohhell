using System;

public interface ITransitionWrapper
{
    void PlayTransitionIn(float duration, Action onAnimationOver);
    void PlayTransitionOut(float duration, Action onAnimationOver);
    void SetActive(bool active);
}
