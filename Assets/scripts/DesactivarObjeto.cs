using UnityEngine;

public class DesactivarObjeto : MonoBehaviour
{
    public GameObject objeto;

    public void Desactivar()
    {
        if (objeto != null)
            objeto.SetActive(false);
    }
}