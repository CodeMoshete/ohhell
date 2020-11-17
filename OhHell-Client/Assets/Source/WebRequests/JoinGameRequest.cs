using System;

[Serializable]
public class JoinGameRequest
{
    public string GameName;
    public string PlayerName;
    
    public JoinGameRequest(string gameName, string playerName)
    {
        GameName = gameName;
        PlayerName = playerName;
    }
}
