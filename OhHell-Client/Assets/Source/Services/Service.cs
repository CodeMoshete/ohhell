
public static class Service
{
    private static EventManager eventManager;
    public static EventManager EventManager
    {
        get
        {
            if (eventManager == null)
            {
                eventManager = new EventManager();
            }

            return eventManager;
        }
    }

    private static TimerManager timerMananager;
    public static TimerManager TimerManager
    {
        get
        {
            if (timerMananager == null)
            {
                timerMananager = new TimerManager();
            }

            return timerMananager;
        }
    }

    private static WebRequestService webRequests;
    public static WebRequestService WebRequests
    {
        get
        {
            if (webRequests == null)
            {
                webRequests = new WebRequestService();
            }
            return webRequests;
        }
    }

    // Manually set services
    public static UpdateManager UpdateManager
    {
        get
        {
            return UpdateManager.Instance;
        }
    }

    public static LocalPreferences LocalPreferences
    {
        get
        {
            return LocalPreferences.Instance;
        }
    }
}
