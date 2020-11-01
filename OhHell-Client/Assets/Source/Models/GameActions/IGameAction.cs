public interface IGameAction
{
    string ActionType { get; }
    void ExecuteAction();
}
