using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameView : View
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _steps;

    public event Action<int> ChangeScene;
    
    public void OnScoreChange(int count) => _score.text = count.ToString();

    public void OnStepsChange(int count, int price) => _steps.text = count.ToString() + $"/<color=\"red\">{price}";
    
    public void CallChangeScene(int index) => ChangeScene?.Invoke(index);
    
    public void RestartGame() => GameEntryPoint.Restart?.Invoke();
}
