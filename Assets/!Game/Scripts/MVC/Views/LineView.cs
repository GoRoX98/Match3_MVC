using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LineView : View
{
    [SerializeField] private Transform _startPosition;
    [SerializeField] private RectTransform _rect;
    // Время анимации элемента
    [SerializeField] private float _animationTime = 0.5f;
    public List<ElementView> Elements;
    
    private Vector2 _elementSize;
    private List<Vector2> _elementPositions;
    private int _elementsCount;
    private int _id;
    private List<ElementView> _newElements = new List<ElementView>();

    private bool _isInit = false;
    
    public void Init(int elementsCount, int id, LevelData data)
    {
        _elementsCount = elementsCount;
        Elements = new List<ElementView>(_elementsCount);
        _elementPositions = new List<Vector2>(_elementsCount);
        _animationTime = data.AnimationTime;
        _id = id;
    }

    // По завершению наполнения линии в первый раз - сортируем и анимируем ее
    public IEnumerator AnimateInit()
    {
        if (_isInit) yield break;
        
        yield return new WaitForSeconds(0.1f);
        
        _elementSize = new Vector2(_rect.sizeDelta.x, _rect.sizeDelta.y / _elementsCount);
        for (int i = 0; i < _elementsCount; i++)
        {
            float posY = _elementPositions.Count == 0 ? _elementSize.y * 1.25f : _elementPositions[i - 1].y + _elementSize.y;
            _elementPositions.Add(new Vector2(_rect.position.x, posY));
        }
        
        Elements = new List<ElementView>(_newElements);
        foreach (var element in Elements)
            element.RectTransform.sizeDelta = _elementSize;
        
        _newElements.Clear();

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < Elements.Count; i++)
        {
            Elements[i].transform.position = _startPosition.position;
            sequence.Append(Animate(Elements[i], i)).AppendInterval(0.1f);
        }

        _isInit = true;
    }
    
    // Анимация сортировки после удаления элемента(ов)
    public Sequence AnimateSort(List<ElementModel> newOrder) 
    {
        var elementMap = Elements.Where(e => e != null)
            .ToDictionary(e => e.Id);
    
        Sequence sequence = DOTween.Sequence()
            .AppendInterval(0.1f);

        for (int i = 0; i < newOrder.Count; i++)
        {
            if (newOrder[i] == null) continue;

            if (elementMap.TryGetValue(newOrder[i].Id, out var element))
                if ((Vector2)element.RectTransform.position != _elementPositions[i])
                    sequence.Append(Animate(element, i));
        }
        
        return sequence;
    }
    
    // Анимация новых элементов
    public void AnimateNewElements(Sequence sequence)
    {
        int nullCount = Elements.FindAll(x => x == null).Count;
        
        for (int i = 0, b = Elements.Count - nullCount; i < nullCount; i++, b++)
        {
            Elements[b] = _newElements[i];
            sequence.Append(Animate(Elements[b], b));
        }
        
        _newElements.Clear();
    }

    // Анимация падения элемента
    private Tween Animate(ElementView element, int indexPos)
    {
        return element.RectTransform.DOMove(_elementPositions[indexPos], _animationTime);
    }
    
    // Добавляем новый элемент
    public void AddNewElement(ElementView element)
    {
        element.transform.SetParent(transform);
        element.transform.position = _startPosition.position;
        element.RectTransform.sizeDelta = _elementSize;
        _newElements.Add(element);
    }
}
