using UnityEngine;
using UnityEngine.SceneManagement;

public class cambiarEscena : MonoBehaviour
{
    [Header("Escenas disponibles")]
    public string[] escenas;

    public void IrAEscenaAleatoria()
    {
        if (escenas == null || escenas.Length == 0)
        {
            Debug.LogWarning("No hay escenas asignadas.");
            return;
        }

        int indice = Random.Range(0, escenas.Length);
        SceneManager.LoadScene(escenas[indice]);
    }
}