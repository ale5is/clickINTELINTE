using System.Collections.Generic;
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
            Debug.LogWarning("No hay escenas asignadas.", this);
            return;
        }

        List<string> escenasValidas = new List<string>(escenas.Length);

        foreach (string escena in escenas)
        {
            if (string.IsNullOrWhiteSpace(escena))
                continue;

            string nombreEscena = escena.Trim();

            if (!Application.CanStreamedLevelBeLoaded(nombreEscena))
            {
                Debug.LogWarning($"La escena '{nombreEscena}' no está disponible en Build Settings.", this);
                continue;
            }

            if (!escenasValidas.Contains(nombreEscena))
                escenasValidas.Add(nombreEscena);
        }

        if (escenasValidas.Count == 0)
        {
            Debug.LogError("No hay escenas válidas para cargar. Revisá los nombres y Build Settings.", this);
            return;
        }

        string escenaActual = SceneManager.GetActiveScene().name;

        if (escenasValidas.Count > 1)
            escenasValidas.Remove(escenaActual);

        int indice = Random.Range(0, escenasValidas.Count);
        SceneManager.LoadScene(escenasValidas[indice]);
    }
}