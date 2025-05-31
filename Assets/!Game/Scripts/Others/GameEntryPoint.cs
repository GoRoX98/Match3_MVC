using System;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private LevelData _levelData;
    [SerializeField] private GameView _gameView;
    [SerializeField] private GameFieldView _gameFieldView;
    
    private GameFieldController _fieldController;
    private GameController _gameController;
    
    public static Action Restart;
    
    private void Start()
    {
        Init();
    }
    
    private void Init()
    {
        GameProcess gameProcess = new GameProcess();
        
        GameFieldModel model = new GameFieldModel(_gameFieldView, _levelData);
        _fieldController = new GameFieldController(model, _gameFieldView, _levelData, gameProcess);

        GameModel gameModel = new GameModel(_gameView, _levelData);
        _gameController = new GameController(gameModel, _gameView, gameProcess);
    }
}
