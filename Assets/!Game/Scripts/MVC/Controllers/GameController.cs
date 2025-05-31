using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер отвечающий за состояние игры
/// </summary>
public class GameController : Controller
{
    private GameView _gameView;
    private GameModel _gameModel;
    private GameProcess _gameProcess;
    
    public GameController(GameModel model, GameView view, GameProcess gameProcess) : base(model, view)
    {
        _gameView = view;
        _gameModel = model;
        _gameProcess = gameProcess;

        _gameModel.UpdateUI();
        _gameView.ChangeScene += ChangeScene;
        _gameProcess.GetStep += MakeStep;
        _gameProcess.EndStep += AddScore;
        GameEntryPoint.Restart += RestartLevel;
    }

    private void ChangeScene(int index) => SceneManager.LoadScene(index);

    private void RestartLevel()
    {
        LevelData data = _gameModel.Data;
        _gameModel = new GameModel(_gameView, data);
        _gameView.OnScoreChange(_gameModel.CurrentScore);
        _gameView.OnStepsChange(_gameModel.CurrentSteps, _gameModel.StepCost);
    }

    private void AddScore(int count) => _gameModel.ChangeScore(count);

    private bool MakeStep()
    {
        if (!_gameModel.CanMakeStep)
            return false;
        
        _gameModel.MakeStep();
        return true;
    }
}
