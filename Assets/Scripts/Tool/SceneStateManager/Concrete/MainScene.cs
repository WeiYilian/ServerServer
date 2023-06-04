using UnityEngine.SceneManagement;

/// <summary>
/// Main场景
/// </summary>
public class MainScene : SceneState
{
    public MainScene() : base("Game") { }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void StateStart()
    {
        AudioManager.Instance.StopAudio(0);
        if (SceneManager.GetActiveScene().name != "Game"/*如果当前的场景名不为sceneName*/)
        {
            SceneManager.LoadScene("Game");//加载名为sceneName的场景
        }
        GameFacade.Instance.PanelManager.Push(new MainPanel());
        PlayerConctroller.Instance.MainPanel = GameFacade.Instance.PanelManager.MainPanel();
    }

    public override void StateEnd()
    {
        GameFacade.Instance.PanelManager.PopAll();
    }
}
