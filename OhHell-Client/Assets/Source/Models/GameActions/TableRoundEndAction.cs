using System;

[Serializable]
public class TableRoundEndAction : IGameAction
{
    public string ActionType
    {
        get
        {
            return "TableRoundEndAction";
        }
    }

    public bool IsRoundEnded;

    public void ExecuteAction()
    {

    }
}
