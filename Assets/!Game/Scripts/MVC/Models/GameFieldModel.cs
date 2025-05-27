using System.Collections.Generic;

public class GameFieldModel : Model
{
    private List<List<ElementModel>> _elements;
    private List<LineModel> _lines;
    private LevelData _data;
    private Dictionary<int, ElementModel> _elementsPool;
    
    public LevelData Data => _data;
    public List<List<ElementModel>> Elements => _elements;
    public Dictionary<int, ElementModel> ElementsPool => _elementsPool;
    public List<LineModel> Lines => _lines;
    
    public GameFieldModel(GameFieldView view, LevelData data) : base(view)
    {
        _data = data;
        
        // Заранее выделяем память на коллекции
        _elements = new List<List<ElementModel>>(data.ColumnsCount);
        for (int i = 0; i < _data.ColumnsCount; i++) 
            _elements.Add(new List<ElementModel>(_data.RowsCount));
        _lines = new List<LineModel>(data.ColumnsCount);
        _elementsPool = new Dictionary<int, ElementModel>(data.FieldSize);
        
        view.ElementViewCreate += NewElement;
        view.ElementViewDestroy += DestroyElement;
        view.LineViewCreate += CreateLine;
        view.ElementViewGet += SetModel;
    }

    // Заполняем поле элементами
    public void SetModel(ElementView view)
    {
        foreach (var list in _elements)
        {
            if (list.Count < _data.RowsCount)
            {
                list.Add(_elementsPool[view.Id]);
                break;
            }
        }
    }
    
    // Вставляем элемент в линию
    public void SetModel(ElementModel model, int listIndex)
    {
        int emptySlot = _elements[listIndex].FindIndex(x => x == null);
        _elements[listIndex][emptySlot] = model;
    }
    
    // Создаем модель линии при необходимости
    private void CreateLine(LineView line)
    {
        int id = _lines.Count;
        _lines.Add(new LineModel(line, id));
    }

    // Создаем модель элемента при необходимости
    private void NewElement(ElementView view) =>
        _elementsPool.Add(view.Id, new ElementModel(view, _data.GetRandomElement(), view.Id));

    // Удаление элементов на случай переполнения пула ElementView, которые после возвращения в пул будут удаляться
    private void DestroyElement(int id) => _elementsPool.Remove(id);
}
