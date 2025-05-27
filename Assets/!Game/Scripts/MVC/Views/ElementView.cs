using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementView : View, IPointerDownHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;
    private int _id;
    private bool _isInit = false;
    
    public RectTransform RectTransform => _rectTransform;
    public int Id => _id;
    //Передаем ID Element
    private event Action<int> OnClick;


    public void Init(int id, Action<int> onClick)
    {
        if (_isInit) return;
        
        _isInit = true;
        _id = id;
        OnClick = onClick;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(_id);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    //Обновляем спрайт элемента
    public void SetSprite(Sprite sprite) => _image.sprite = sprite;

}
