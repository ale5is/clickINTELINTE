using UnityEngine;

public class DesactivarObjeto : MonoBehaviour
{
    public GameObject objeto;

    public void Desactivar()
    {
        if (objeto == null)
        {
            Debug.LogWarning("No se puede desactivar el objeto: asigná una referencia en el Inspector.", this);
            return;
        }

        objeto.SetActive(false);
    }
}