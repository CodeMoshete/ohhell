using Game.Controllers;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private FlowController gameFlow;
    private void Start()
    {
        // Initialize our managers that use the update manager early.
        // This prevents them from being created during an update loop.
        TimerManager manager = Service.TimerManager;

        gameFlow = new FlowController();
        gameFlow.StartGame();
    }
}
