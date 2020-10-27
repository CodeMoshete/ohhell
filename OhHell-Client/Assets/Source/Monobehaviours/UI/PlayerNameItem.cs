using UnityEngine;
using UnityEngine.UI;

public class PlayerNameItem : MonoBehaviour
{
    public Text NameField;

    public void SetName(string name)
    {
        NameField.text = name;
    }
}
