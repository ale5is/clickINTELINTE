using UnityEngine;

public class ActivarObjeto : MonoBehaviour
{
    public GameObject objeto;

    public void Activar()
    {
        if (objeto != null)
            objeto.SetActive(true);
    }
}