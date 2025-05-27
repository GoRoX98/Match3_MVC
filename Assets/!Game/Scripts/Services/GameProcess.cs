using System;

/// <summary>
/// Сервис для связывания процессов GameFieldController и GameController
/// </summary>
public class GameProcess
{
    public event Func<bool> GetStep;
    public event Action<int> EndStep;

    public bool StartStep() => GetStep?.Invoke() ?? false;
    
    public void FinishStep(int score) => EndStep?.Invoke(score);
}
