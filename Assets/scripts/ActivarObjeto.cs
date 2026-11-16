using UnityEngine;

public class ActivarObjeto : MonoBehaviour
{
    public GameObject objeto;

    public void Activar()
    {
        if (objeto == null)
        {
            Debug.LogWarning("No se puede activar el objeto: asigná una referencia en el Inspector.", this);
            return;
        }

        objeto.SetActive(true);
    }
}