using System;
using UnityEngine;

public enum FadeType
{
    FadeIn,
    FadeOut
}

public class TransitionScreen : MonoBehaviour
{
    private enum FadeState
    {
        Idle,
        FadeIn,
        FadeOut
    }

    private const float FADE_TIME = 1f;

    private CanvasGroup fader;
    private FadeState currentState;
    private float fadeTimer;
    private Action onFadeDone;

    private void Start()
    {
        fader = GetComponent<CanvasGroup>();
    }

    public void StartFade(FadeType type, Action onDone)
    {
        if (currentState == FadeState.Idle)
        {
            gameObject.SetActive(true);
            currentState = type == FadeType.FadeIn ? FadeState.FadeIn : FadeState.FadeOut;
            fadeTimer = FADE_TIME;
            onFadeDone = onDone;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        switch(currentState)
        {
            case FadeState.Idle:
                return;
            case FadeState.FadeIn:
                fadeTimer -= dt;
                if (fadeTimer <= 0f)
                {
                    fader.alpha = 0f;
                    gameObject.SetActive(false);
                    currentState = FadeState.Idle;
                    if (onFadeDone != null)
                    {
                        onFadeDone();
                    }
                }
                else
                {
                    float pct = fadeTimer / FADE_TIME;
                    fader.alpha = pct;
                }
                break;
            case FadeState.FadeOut:
                fadeTimer -= dt;
                if (fadeTimer <= 0f)
                {
                    fader.alpha = 1f;
                    currentState = FadeState.Idle;
                    if (onFadeDone != null)
                    {
                        onFadeDone();
                    }
                }
                else
                {
                    float pct = 1f - fadeTimer / FADE_TIME;
                    fader.alpha = pct;
                }
                break;
        }
    }
}
