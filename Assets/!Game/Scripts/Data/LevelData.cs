using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Main Settings")] 
    [SerializeField] private int _startSteps = 10;
    [SerializeField] private int _stepCost = 3;
    [Header("Field Settings")]
    [SerializeField] private List<ElementData> elements = new List<ElementData>();
    [SerializeField] private int _columnsCount = 10;
    [SerializeField] private int _rowsCount = 12;
    [SerializeField][Min(0.1f)] private float _animationTime = 0.15f;
    
    public int StartSteps => _startSteps;
    public int StepCost => _stepCost;
    public List<ElementData> Elements => elements;
    public int ColumnsCount => _columnsCount;
    public int RowsCount => _rowsCount;
    public int FieldSize => _columnsCount * _rowsCount;
    public float AnimationTime => _animationTime;
    
    public ElementData GetRandomElement() => elements[Random.Range(0, elements.Count)];
}
