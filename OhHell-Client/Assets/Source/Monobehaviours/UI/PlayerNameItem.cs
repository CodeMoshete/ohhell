using UnityEngine;
using UnityEngine.UI;

public class PlayerNameItem : MonoBehaviour
{
    public Text NameField;
    public Text TrickField;

    public void SetName(string name)
    {
        NameField.text = name;
    }

    public void SetNumTricks(int numTricks)
    {
        TrickField.gameObject.SetActive(numTricks > 0);
        TrickField.text = "Tricks: " + numTricks;
    }
}
