/// <summary>
/// Контроллер отвечающий за состояние игры
/// </summary>
public class GameController : Controller
{
    private GameModel _gameModel;
    private GameProcess _gameProcess;
    
    public GameController(GameModel model, GameView view, GameProcess gameProcess) : base(model, view)
    {
        _gameModel = model;
        _gameProcess = gameProcess;

        _gameModel.UpdateUI();
        _gameProcess.GetStep += MakeStep;
        _gameProcess.EndStep += AddScore;
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
