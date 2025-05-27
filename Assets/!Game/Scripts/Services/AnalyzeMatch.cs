using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnalyzeMatch
{
    /// <summary>
    /// Передаем текущее поле и id нажатого элемента, чтобы найти совпадения
    /// </summary>
    /// <param name="elements">Текущее поле</param>
    /// <param name="id">Id нажатого элемента</param>
    /// <returns>Возвращает список соседей совпадающих с нажатым элементом, их координаты и индексы линий</returns>
    public (bool, List<ElementModel>, List<Vector2>, List<int> lines) FindMatch(List<List<ElementModel>> elements, int id)
    {
        //Ищем нажатый элемент
        var targetElement = elements
            .SelectMany((list, line) => list.Select((elementModel, row) => (e: elementModel, line, row)))
            .FirstOrDefault(x => x.e.Id == id);

        if (targetElement.e == null) return (false, null, null, null);

        (ElementModel element, int line, int row) = targetElement;
        string elementName = element.Name;
    
        var matches = new List<ElementModel> { element };
        var positions = new List<Vector2> { new(line, row) };
        List<int> lines = new List<int> { line };

        // Поиск соседних элементов
        foreach (var (column, lineRow) in new[] { (-1,0), (1,0), (0,-1), (0,1) })
        {
            int l = line + column;
            int r = row + lineRow;
        
            if (l >= 0 && l < elements.Count && r >= 0 && r < elements[l].Count && elements[l][r].Name == elementName)
            {
                if (matches.Contains(elements[l][r])) continue;
                
                matches.Add(elements[l][r]);
                if (r < row)
                {
                    int index = positions.FindIndex(x => x.x == l && x.y == row);
                    positions[index] = new Vector2(l, r);
                    positions.Add(new(line, row));
                }
                else
                    positions.Add(new(l, r));
                
                if (!lines.Contains(l))
                    lines.Add(l);
            }
        }
        
        return (true, matches, positions, lines);
    }
}
