using Game.Controllers;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private FlowController gameFlow;
    private void Start()
    {
        gameFlow = new FlowController();
        gameFlow.StartGame();
    }
}
