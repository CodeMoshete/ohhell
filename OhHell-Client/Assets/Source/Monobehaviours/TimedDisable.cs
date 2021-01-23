using UnityEngine;

public class TimedDisable : MonoBehaviour
{
    public float DisableTime;
    private float timeLeft;

    private void OnEnable()
    {
        timeLeft = DisableTime;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            gameObject.SetActive(false);
            timeLeft = 0f;
        }
    }
}
