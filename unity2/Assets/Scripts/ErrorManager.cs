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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddError(string error)
    {
        errors.Add(error);
        Debug.Log($"Ошибка добавлена: {error}");
    }

    public void ClearErrors()
    {
        errors.Clear();
    }
}