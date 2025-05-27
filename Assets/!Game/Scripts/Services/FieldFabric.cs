using UnityEngine;

//Фабрика для генерации игровых объектов
public static class FieldFabric
{
    public static T Create<T>(T prefab, Transform parent) where T : View
    {
        T obj = GameObject.Instantiate(prefab, parent);
        return obj;
    }
}
