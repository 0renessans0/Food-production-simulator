using UnityEngine;
using System.Collections.Generic;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance;
    public List<string> errors = new List<string>();

    void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[ErrorManager] Создан впервые, Instance установлен");
    }
    else
    {
        Debug.Log("[ErrorManager] Уничтожаю лишний экземпляр");
        Destroy(gameObject);
    }
}


    public void AddError(string error)
{
    if (!errors.Contains(error))
    {
        errors.Add(error);
        Debug.Log($"[ErrorManager] Добавлена ошибка: {error}");
    }
    else
    {
        Debug.Log($"[ErrorManager] Ошибка уже есть: {error}");
    }
}
    public void ClearErrors()
    {
        errors.Clear();
    }
}