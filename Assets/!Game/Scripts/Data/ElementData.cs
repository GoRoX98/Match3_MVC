using UnityEngine;

[CreateAssetMenu(fileName = "New Element", menuName = "ScriptableObjects/Element")]
public class ElementData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    
    public string Name => _name;
    public Sprite Sprite => _sprite;
}
