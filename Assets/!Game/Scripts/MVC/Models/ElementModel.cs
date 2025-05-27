using System;
using UnityEngine;

public class ElementModel : Model
{
    private int _id;
    private ElementData _data;
    
    public ElementData Data => _data;
    public int Id => _id;
    public string Name => _data.Name;

    private event Action<Sprite> SpriteUpdate; 
    
    public ElementModel(ElementView view, ElementData data, int id) : base(view)
    {
        _id = id;
        _data = data;
        view.SetSprite(data.Sprite);
        SpriteUpdate += view.SetSprite;
    }

    public void SetData(ElementData data)
    {
        _data = data;
        SpriteUpdate?.Invoke(data.Sprite);
    }
}
