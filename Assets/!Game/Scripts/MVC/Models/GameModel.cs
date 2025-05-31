using System;

public class GameModel : Model
{
    private LevelData _data;
    private int _currentSteps;
    private int _currentScore = 0;
    private int _playerStep = 0;
    
    public LevelData Data => _data;
    public int CurrentSteps
    {
        get => _currentSteps;
        private set
        {
            _currentSteps = value;
            StepsChanged?.Invoke(_currentSteps, StepCost);
        }
    }

    public bool CanMakeStep => CurrentSteps >= StepCost;

    public int CurrentScore
    {
        get =>_currentScore;
        private set
        {
            _currentScore = value;
            ScoreChanged?.Invoke(_currentScore);
        }
    }
    
    public int StepCost => _data.StepCost + _playerStep;
    private event Action<int> ScoreChanged;
    private event Action<int, int> StepsChanged;
    
    public GameModel(GameView view, LevelData data) : base(view)
    {
        _data = data;
        CurrentSteps = _data.StartSteps;
        CurrentScore = 0;
        
        ScoreChanged += view.OnScoreChange;
        StepsChanged += view.OnStepsChange;
    }

    // Добавляем очки и ходы
    public void ChangeScore(int count)
    {
        CurrentSteps += count;
        CurrentScore += count;
    }
    
    // Тратим очки игрока на ход
    public void MakeStep()
    {
        CurrentSteps -= StepCost;
        _playerStep += 1;
    }

    public void UpdateUI()
    {
        StepsChanged?.Invoke(_currentSteps, StepCost);
        ScoreChanged?.Invoke(_currentScore);
    }
}
