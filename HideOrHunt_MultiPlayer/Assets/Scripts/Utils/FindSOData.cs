using UnityEngine;

public class FindSOData : MonoBehaviour
{
    protected TransformDataSO data;
    protected string nameToFind = "inclinacion";

    protected virtual void Awake()
    {
        data = FindScriptableObject(nameToFind);
        if (data == null)
        {
            Debug.LogError("No se encontró ningún ScriptableObject con el nombre: " + nameToFind);
            return;
        }
    }

    protected TransformDataSO FindScriptableObject(string nombre)
    {
        return Resources.Load<TransformDataSO>(nombre);
    }
}
