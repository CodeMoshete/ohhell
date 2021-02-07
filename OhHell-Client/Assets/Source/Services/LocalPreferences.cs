public class LocalPreferences
{
    private static LocalPreferences instance;
    public static LocalPreferences Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LocalPreferences();
            }
            return instance;
        }
    }

    public bool DisableTurnNotification = false;
    public bool EnableAdvancedCardControls = false;
}
