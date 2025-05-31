using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameFieldView : View
{
    [SerializeField] private LineView _linePrefab;
    [SerializeField] private ElementView _elementPrefab;
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private Transform _pool;
    [SerializeField] private List<LineView> _lines;
    [SerializeField] private ScaleField _scaleField;
    
    private ObjectPool<ElementView> _elementsPool;
    private bool _isInit = false;
    
    public List<LineView> Lines => _lines;
    public ObjectPool<ElementView> ElementsPool => _elementsPool;

    
    // Событие уведомляет о создании экземпляра LineView
    public event Action<LineView> LineViewCreate;
    // Событие уведомляет о создании экземпляра ElementView чтобы была создана модель для него
    public event Action<ElementView> ElementViewCreate;
    // Событие уведомляет об активации экземпляра ElementView
    public event Action<ElementView> ElementViewGet; 
    // Событие уведомляет об уничтожении экземпляра ElementView
    public event Action<int> ElementViewDestroy;
    
    /// <summary>
    /// Инициализируем вьюху и пул ElementView. Инициализируем линии, передаем им элементы
    /// </summary>
    /// <param name="data"></param>
    /// <param name="OnElementClick"></param>
    public void Init(LevelData data, Action<int> OnElementClick)
    {
        if (_isInit) return;
        
        _scaleField.Init(data);
        
        for (int i = 0; i < data.ColumnsCount; i++)
        {
            var line = Instantiate(_linePrefab, _lineTransform);
            line.Init(data.RowsCount, i, data);
            _lines.Add(line);
            LineViewCreate?.Invoke(line);
        }
        
        // Инициализируем ObjectPool элементов
        _elementsPool = new ObjectPool<ElementView>(
            () =>
            {
                var element = FieldFabric.Create(_elementPrefab, _pool);
                element.Init(_elementsPool.CountAll, OnElementClick);
                element.Deactivate();
                ElementViewCreate?.Invoke(element);
                return element;
            }, 
            OnElementGet,
            OnReleaseElement, 
            OnElementDestroy, 
            true, data.FieldSize + 10, data.FieldSize + 20
            );

        foreach (var line in _lines)
        {
            for (int i = 0; i < data.RowsCount; i++)
            {
                var view = _elementsPool.Get(); 
                line.AddNewElement(view);
                ElementViewGet?.Invoke(view);
            }
            StartCoroutine(line.AnimateInit());
        }
        
        _isInit = true;
    }
    
    #region Pool
    private void OnElementGet(ElementView element)
    {
        element.Activate();
    }

    private void OnReleaseElement(ElementView element)
    {
        element.transform.SetParent(_pool);
        element.Deactivate();
    }

    private void OnElementDestroy(ElementView element)
    {
        ElementViewDestroy?.Invoke(element.Id);
        Destroy(element.gameObject);
    }
    #endregion
}
