using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Обновление размера ячеек поля
/// </summary>
public class ScaleField : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _group;
    [SerializeField] private RectTransform _rect;

    private bool _isInit = false;
    
    public void Init(LevelData data)
    {
        if (_isInit) return;
        
        _isInit = true;
        _group.cellSize = new Vector2(_rect.rect.width / data.ColumnsCount, _rect.rect.height / data.RowsCount);
    }
}
