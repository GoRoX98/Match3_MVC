using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Контроллер отвечающий за игровое поле
/// </summary>
public class GameFieldController : Controller
{
    private GameFieldView _gfView;
    private GameFieldModel _gfModel;
    private GameProcess _gameProcess;
    
    private LevelData _data;
    private AnalyzeMatch _analyzer;
    
    public event Action<int> OnElementClick;
    
    // Инициализируем контроллер, прокидывает событие для реагирования на клик, инициализируем вьюху, прокидываем элементы по линиям
    // Инициализируем сервисы
    public GameFieldController(GameFieldModel model, GameFieldView view, LevelData data, GameProcess gameProcess) : base(model, view)
    {
        _gameProcess = gameProcess;
        _gfView = view;
        _gfModel = model;
        _data = data;

        OnElementClick += ElementClick;
        _gfView.Init(_data, OnElementClick);
        
        _analyzer = new AnalyzeMatch();
    }
    
    // Обработка нажатия на элемента
    private void ElementClick(int id)
    {
        // Проверка можно ли сделать ход
        if (!_gameProcess.StartStep()) return;
        
        (bool found, List<ElementModel> match, List<Vector2> positions, List<int> lineIndexes) =_analyzer.FindMatch(_gfModel.Elements, id);
        if (!found) return;

        // Удаляем нажатый элемент и совпавшие с ним
        foreach (Vector2 position in positions)
        {
            int line = (int)position.x;
            int row = (int)position.y;
            _gfView.ElementsPool.Release(_gfView.Lines[line].GetElement(row));
            _gfModel.Elements[line][row] = null;
        }
        
        _gameProcess.FinishStep(match.Count);
        
        List<Sequence> sequence = SortField(lineIndexes);
        
        foreach (var position in positions)
            GetNewElement((int)position.x);
        for (int i = 0; i < lineIndexes.Count; i++)
            _gfView.Lines[lineIndexes[i]].AnimateNewElements(sequence[i]);
    }

    // Берем новый элемент для линии и передаем его
    private void GetNewElement(int lineIndex)
    {
        var view = _gfView.ElementsPool.Get();
        var model = _gfModel.ElementsPool[view.Id];
        model.SetData(_data.GetRandomElement());
        _gfModel.SetModel(model, lineIndex);
        _gfView.Lines[lineIndex].AddNewElement(view);
        view.Activate();
    }
    
    // Сортируем линию, пустые слоты перемещаем в начало
    private List<Sequence> SortField(List<int> lineIndexes)
    {
        var sequences = new List<Sequence>();
        foreach (var lineIndex in lineIndexes)
        {
            var line = _gfModel.Elements[lineIndex];
            var lineView = _gfView.Lines[lineIndex].Elements;
    
            // Собираем элементы с сохранением порядка без null
            var nonNullElements = line.Where(x => x != null).ToList();
            var nonNullViews = lineView.Where(x => x != null).ToList();
    
            // Добавляем null в конец для обеих коллекций
            nonNullElements.AddRange(Enumerable.Repeat<ElementModel>(null, line.Count - nonNullElements.Count));
            nonNullViews.AddRange(Enumerable.Repeat<ElementView>(null, lineView.Count - nonNullViews.Count));

            // Заменяем исходные коллекции
            _gfModel.Elements[lineIndex] = nonNullElements;
            _gfView.Lines[lineIndex].Elements = nonNullViews;

            // Запускаем анимацию сортировки
            sequences.Add(_gfView.Lines[lineIndex].AnimateSort(_gfModel.Elements[lineIndex]));
        }
        return sequences;
    }
}
