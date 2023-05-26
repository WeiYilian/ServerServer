using UnityEngine.SceneManagement;

public class Game2Scene : SceneState
{
    public Game2Scene() : base("Game2") { }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void StateStart()
    {
        if (SceneManager.GetActiveScene().name != "Game2"/*如果当前的场景名不为sceneName*/)
        {
            SceneManager.LoadScene("Game2");//加载名为sceneName的场景
        }
        GameFacade.Instance.PanelManager.Push(new MainPanel());
    }

    public override void StateEnd()
    {
        GameFacade.Instance.PanelManager.PopAll();
    }
}
