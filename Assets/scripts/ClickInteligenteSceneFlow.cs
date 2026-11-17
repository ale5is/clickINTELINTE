using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class ClickInteligenteSceneFlow : MonoBehaviour
{
    public enum ScreenType
    {
        Menu,
        Tutorial,
        Question,
        Feedback,
        Results
    }

    [Serializable]
    public sealed class QuestionData
    {
        public string title;
        public string advertiser;
        public string message;
        public string buttonLabel;
        public bool suspicious;
        public string explanation;
        public string signals;
        public string advice;
    }

    public ScreenType screenType;
    public int itemIndex;
    public string nextScene;
    public string previousScene;

    private const string CorrectKey = "CI_Correct";
    private const string IncorrectKey = "CI_Incorrect";
    private const string AnswerKey = "CI_LastAnswerCorrect";
    private const string SuspiciousKey = "CI_LastSuspicious";

    public static readonly QuestionData[] Questions =
    {
        new QuestionData { title = "ACTUALIZACIÓN URGENTE", advertiser = "VideoPlus HD", message = "Tu reproductor está desactualizado. Instalá esta actualización ahora o podrías perder todos tus archivos.", buttonLabel = "DESCARGAR AHORA", suspicious = true, explanation = "El mensaje intenta asustarte y crear urgencia. Además, pide instalar un archivo desde el propio anuncio.", signals = "La urgencia, la amenaza de perder archivos y la descarga directa son señales de alerta.", advice = "No descargues el archivo. Cerrá el anuncio y buscá información desde la aplicación o el sitio oficial." },
        new QuestionData { title = "¡GANASTE UN TELÉFONO!", advertiser = "Premios Flash", message = "Fuiste seleccionado entre miles de personas. Reclamá tu teléfono gratis ingresando tus datos y pagando solo el envío.", buttonLabel = "RECLAMAR PREMIO", suspicious = true, explanation = "Promete un premio inesperado y pide datos personales y un pago para reclamarlo.", signals = "Los premios inesperados, la presión para actuar y las solicitudes de datos o pagos pueden indicar un engaño.", advice = "No ingreses datos ni pagues. Consultá las promociones únicamente en canales oficiales." },
        new QuestionData { title = "RUTINA FACIAL DERMA CONTROL", advertiser = "L'Oréal Men Expert", message = "Conocé la línea Derma Control para el cuidado facial diario. Revisá los productos y sus indicaciones en los canales oficiales de la marca.", buttonLabel = "CONOCER LA RUTINA", suspicious = false, explanation = "Es una campaña de cuidado personal de una marca identificable. No solicita contraseñas, pagos inesperados ni descargas externas.", signals = "La creatividad pertenece a una campaña de marca identificable. Aun así, verificá el sitio y el vendedor antes de comprar.", advice = "Buscá la página oficial de L'Oréal y revisá los detalles del producto antes de realizar una compra." },
        new QuestionData { title = "¡SE DETECTARON 5 VIRUS!", advertiser = "Alerta de Seguridad", message = "Tu dispositivo está infectado. Instalá nuestro limpiador desconocido en los próximos 30 segundos para evitar daños.", buttonLabel = "LIMPIAR DISPOSITIVO", suspicious = true, explanation = "Un anuncio no puede confirmar por sí solo que tu dispositivo tenga virus. Intenta asustarte y forzarte a instalar una aplicación desconocida.", signals = "Las alertas alarmistas, las cuentas regresivas y las aplicaciones desconocidas son señales de riesgo.", advice = "Cerrá el anuncio. Para revisar el dispositivo, utilizá herramientas de seguridad conocidas y fuentes oficiales." },
        new QuestionData { title = "APRENDÉ A TU RITMO", advertiser = "Khan Academy", message = "Fortalecé conceptos y practicá lo que aprendés. Explorá los recursos educativos gratuitos desde la página oficial.", buttonLabel = "VER LECCIONES", suspicious = false, explanation = "La campaña promueve recursos educativos y no exige datos de pago ni descargas desde un enlace desconocido.", signals = "El anunciante es una organización educativa identificable y el mensaje invita a consultar recursos en su sitio oficial.", advice = "Escribí la dirección oficial de Khan Academy o buscá su aplicación en la tienda legítima." },
        new QuestionData { title = "DESCARGA SECRETA", advertiser = "Juegos Premium Gratis", message = "Obtené todos los juegos pagos gratis. Desactivá la protección del teléfono e instalá este archivo fuera de la tienda.", buttonLabel = "DESCARGAR APK", suspicious = true, explanation = "Pide desactivar una protección y descargar un archivo desde fuera de una tienda confiable.", signals = "Desactivar la seguridad y descargar archivos desconocidos aumenta el riesgo de instalar software malicioso.", advice = "No desactives las protecciones ni instales archivos de anuncios. Buscá aplicaciones en tiendas oficiales." },
        new QuestionData { title = "DESCUBRÍ ARGENTINA", advertiser = "Visit Argentina", message = "Explorá paisajes, destinos y experiencias para planificar tu próximo viaje desde el sitio oficial de turismo.", buttonLabel = "EXPLORAR DESTINOS", suspicious = false, explanation = "La pieza pertenece a una campaña turística identificable y dirige a información de destinos, sin exigir datos sensibles ni instalaciones externas.", signals = "El anuncio presenta un destino y una marca turística verificable, sin promesas extraordinarias ni presión para actuar.", advice = "Comprobá que el sitio sea el canal oficial de turismo antes de reservar o compartir información." },
        new QuestionData { title = "TU CUENTA SERÁ BLOQUEADA", advertiser = "Centro de Soporte", message = "Detectamos un problema. Verificá tu identidad y escribí tu contraseña para evitar que tu cuenta sea eliminada hoy.", buttonLabel = "VERIFICAR CUENTA", suspicious = true, explanation = "El anuncio imita un aviso de soporte y busca que entregues la contraseña desde un enlace externo.", signals = "Las amenazas de bloqueo, la urgencia y los pedidos de contraseña son señales claras de phishing.", advice = "No uses el enlace. Abrí la aplicación oficial por tu cuenta y revisá allí el estado de la cuenta." },
        new QuestionData { title = "¡GANASTE BENEFICIOS PREMIUM!", advertiser = "Beneficios Plus", message = "Tu cuenta puede recibir funciones premium sin costo. Iniciá sesión con tu correo y contraseña para activar el beneficio antes de que termine la promoción.", buttonLabel = "ACTIVAR BENEFICIO", suspicious = true, explanation = "El anuncio ofrece un beneficio gratuito a cambio de iniciar sesión desde su propia promoción. Podría ser un intento de robar credenciales.", signals = "Los beneficios inesperados y los formularios de inicio de sesión abiertos desde anuncios son señales de riesgo.", advice = "No ingreses tu contraseña desde el anuncio. Abrí la aplicación oficial y comprobá allí si existe alguna promoción." }
    };

    private void Start()
    {
        WireButton("StartButton", StartGame);
        WireButton("ContinueButton", Continue);
        WireButton("PreviousButton", Previous);
        WireButton("SafeButton", AnswerSafe);
        WireButton("ReportButton", AnswerSuspicious);
        WireButton("NextButton", Next);
        WireButton("RestartButton", Restart);
        RefreshSceneText();
    }

    private void WireButton(string objectName, UnityEngine.Events.UnityAction action)
    {
        GameObject target = GameObject.Find(objectName);
        if (target == null)
            return;
        Button button = target.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(action);
    }

    private Transform FindChild(Transform parent, string objectName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == objectName)
                return child;
            Transform found = FindChild(child, objectName);
            if (found != null)
                return found;
        }
        return null;
    }

    private void RefreshSceneText()
    {
        if (screenType == ScreenType.Question && itemIndex >= 0 && itemIndex < Questions.Length)
        {
            QuestionData data = Questions[itemIndex];
            SetText("QuestionTitle", data.title);
            SetText("Advertiser", data.advertiser);
            SetText("AdMessage", data.message);
            SetText("FakeAdButtonLabel", data.buttonLabel);
            SetText("Progress", "ANUNCIO " + (itemIndex + 1) + " / " + Questions.Length);
        }
        else if (screenType == ScreenType.Feedback && itemIndex >= 0 && itemIndex < Questions.Length)
        {
            QuestionData data = Questions[itemIndex];
            bool correct = PlayerPrefs.GetInt(AnswerKey, 0) == 1;
            bool suspicious = PlayerPrefs.GetInt(SuspiciousKey, 0) == 1;
            SetText("FeedbackTitle", correct ? "¡Muy bien!" : "Repasemos esta decisión");
            SetText("FeedbackStatus", correct ? "Tu clasificación fue correcta." : "La clasificación correcta era: " + (suspicious ? "REPORTAR COMO SOSPECHOSO" : "PARECE SEGURO"));
            SetText("Explanation", data.explanation);
            SetText("Signals", "Señales para recordar: " + data.signals);
            SetText("Advice", "Qué hacer: " + data.advice);
            SetText("Score", "Correctas: " + PlayerPrefs.GetInt(CorrectKey, 0) + "     Incorrectas: " + PlayerPrefs.GetInt(IncorrectKey, 0));
        }
        else if (screenType == ScreenType.Results)
        {
            int correct = PlayerPrefs.GetInt(CorrectKey, 0);
            int incorrect = PlayerPrefs.GetInt(IncorrectKey, 0);
            int total = correct + incorrect;
            int percentage = total == 0 ? 0 : Mathf.RoundToInt((float)correct / total * 100f);
            SetText("FinalScore", percentage + "%");
            SetText("FinalDetails", "Correctas: " + correct + "   Incorrectas: " + incorrect + "   Total: " + total);
            SetText("FinalMessage", percentage >= 80 ? "¡Excelente trabajo! Reconociste la mayoría de las señales de riesgo." : percentage >= 60 ? "¡Buen avance! Repasá las explicaciones para reconocer más señales." : "Seguí practicando. Recordá frenar, verificar la fuente y evitar descargas desde anuncios.");
        }
    }

    private void SetText(string objectName, string value)
    {
        GameObject target = GameObject.Find(objectName);
        if (target == null)
            return;
        TMP_Text text = target.GetComponent<TMP_Text>();
        if (text != null)
            text.text = value;
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt(CorrectKey, 0);
        PlayerPrefs.SetInt(IncorrectKey, 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Tutorial01");
    }

    public void Continue()
    {
        if (screenType == ScreenType.Tutorial)
        {
            if (itemIndex < 3)
                SceneManager.LoadScene("Tutorial0" + (itemIndex + 1));
            else
                SceneManager.LoadScene("Anuncio01");
        }
    }

    public void Previous()
    {
        if (screenType == ScreenType.Tutorial && itemIndex > 1)
            SceneManager.LoadScene("Tutorial0" + (itemIndex - 1));
        else if (screenType == ScreenType.Tutorial)
            SceneManager.LoadScene("Menu");
    }

    public void AnswerSafe() => RecordAnswer(false);
    public void AnswerSuspicious() => RecordAnswer(true);

    private void RecordAnswer(bool reportedSuspicious)
    {
        if (screenType != ScreenType.Question || itemIndex < 0 || itemIndex >= Questions.Length)
            return;
        QuestionData data = Questions[itemIndex];
        bool correct = reportedSuspicious == data.suspicious;
        PlayerPrefs.SetInt(AnswerKey, correct ? 1 : 0);
        PlayerPrefs.SetInt(SuspiciousKey, data.suspicious ? 1 : 0);
        PlayerPrefs.SetInt(CorrectKey, PlayerPrefs.GetInt(CorrectKey, 0) + (correct ? 1 : 0));
        PlayerPrefs.SetInt(IncorrectKey, PlayerPrefs.GetInt(IncorrectKey, 0) + (correct ? 0 : 1));
        PlayerPrefs.Save();
        SceneManager.LoadScene("Explicacion" + (itemIndex + 1).ToString("00"));
    }

    public void Next()
    {
        if (screenType != ScreenType.Feedback)
            return;
        if (itemIndex + 1 < Questions.Length)
            SceneManager.LoadScene("Anuncio" + (itemIndex + 2).ToString("00"));
        else
            SceneManager.LoadScene("Resultados");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Menu");
    }
}
