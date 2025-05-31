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
    
    /// <summary>
    /// Инициализируем контроллер, прокидываем событие для реагирования на клик, инициализируем вьюху, прокидываем элементы по линиям
    /// </summary>
    /// <param name="model">Модель игрового поля</param>
    /// <param name="view"> View игрового поля</param>
    /// <param name="data"> Данные уровня</param>
    /// <param name="gameProcess">Сервис игрового процесса</param>
    public GameFieldController(GameFieldModel model, GameFieldView view, LevelData data, GameProcess gameProcess) : base(model, view)
    {
        _gameProcess = gameProcess;
        _gfView = view;
        _gfModel = model;
        _data = data;

        OnElementClick += ElementClick;
        _gfView.Init(_data, OnElementClick);
        
        _analyzer = new AnalyzeMatch();
        GameEntryPoint.Restart += RestartGame;
        view.ElementViewCreate += NewElement;
        view.ElementViewDestroy += DestroyElement;
        view.LineViewCreate += CreateLine;
    }

    private void RestartGame()
    {
        throw new NotImplementedException();
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
            _gfView.ElementsPool.Release(GetElement(line, row));
            _gfModel.Elements[line][row] = null;
        }
        
        _gameProcess.FinishStep(match.Count);
        
        List<Sequence> sequence = SortField(lineIndexes);
        
        foreach (var position in positions)
            GetNewElement((int)position.x);
        for (int i = 0; i < lineIndexes.Count; i++)
            _gfView.Lines[lineIndexes[i]].AnimateNewElements(sequence[i]);
    }

    // Забираем элемент из линии
    private ElementView GetElement(int line, int index)
    {
        var element = _gfView.Lines[line].Elements[index];
        _gfView.Lines[line].Elements[index] = null;
        return element;
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

    #region WorkWithModel

    /// <summary>
    /// Создаем модель линии при необходимости
    /// </summary>
    /// <param name="line">View линии</param>
    private void CreateLine(LineView line)
    {
        int id = _gfModel.Lines.Count;
        _gfModel.Lines.Add(new LineModel(line, id));
    }
    
    /// <summary>
    /// Удаление элементов на случай переполнения пула ElementView, которые после возвращения в пул будут удаляться
    /// </summary>
    /// <param name="id">Id элемента</param>
    private void DestroyElement(int id) => _gfModel.ElementsPool.Remove(id);
    
    /// <summary>
    /// Создаем модель элемента при необходимости
    /// </summary>
    /// <param name="view">View элемента</param>
    private void NewElement(ElementView view) => _gfModel.ElementsPool.Add(view.Id, new ElementModel(view, _data.GetRandomElement(), view.Id));

    #endregion
}
