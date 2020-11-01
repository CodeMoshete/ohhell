using System;

public interface IGameAction
{
    string ActionType { get; }
    void ExecuteAction(Action onDone);
    void PopulateFromJson(string json);
}
