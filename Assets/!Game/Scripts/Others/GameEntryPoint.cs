using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private LevelData _levelData;
    [SerializeField] private GameView _gameView;
    [SerializeField] private GameFieldView _gameFieldView;
    
    private GameFieldController _fieldController;
    private GameController _gameController;
    
    private void Start()
    {
        Init();
        GameView.Restart += Restart;
    }

    private void OnDestroy()
    {
        GameView.Restart -= Restart;
    }

    private void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    private void Init()
    {
        GameProcess gameProcess = new GameProcess();
        
        GameFieldModel model = new GameFieldModel(_gameFieldView, _levelData);
        _fieldController = new GameFieldController(model, _gameFieldView, _levelData, gameProcess);

        GameModel gameModel = new GameModel(_gameView, _levelData);
        _gameController = new GameController(gameModel, _gameView, gameProcess);
    }
}
