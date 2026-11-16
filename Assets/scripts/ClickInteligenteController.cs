using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

[DisallowMultipleComponent]
public sealed class ClickInteligenteController : MonoBehaviour
{
    [Header("Material educativo")]
    [SerializeField] private VideoClip[] videosTutoriales;
    [SerializeField, Range(5, 10)] private int preguntasPorPartida = 9;

    private Canvas canvas;
    private RectTransform screenRoot;
    private VideoPlayer videoPlayer;
    private RenderTexture videoTexture;
    private readonly List<AdScenario> round = new List<AdScenario>();
    private int tutorialPage;
    private int questionIndex;
    private int correctAnswers;
    private int incorrectAnswers;
    private bool answered;
    private bool lastAnswerWasCorrect;

    private static readonly AdScenario[] ScenarioBank =
    {
        new AdScenario("ACTUALIZACIÓN URGENTE", "VideoPlus HD", "Tu reproductor está desactualizado. Instalá esta actualización ahora o podrías perder todos tus archivos.", "DESCARGAR AHORA", true,
            "El mensaje intenta asustarte y crear urgencia. Además, pide instalar un archivo desde el propio anuncio.",
            "La urgencia, la amenaza de perder archivos y la descarga directa son señales de alerta.",
            "No descargues el archivo. Cerrá el anuncio y buscá información desde la aplicación o el sitio oficial."),
        new AdScenario("¡GANASTE UN TELÉFONO!", "Premios Flash", "Fuiste seleccionado entre miles de personas. Reclamá tu teléfono gratis ingresando tus datos y pagando solo el envío.", "RECLAMAR PREMIO", true,
            "Promete un premio inesperado y pide datos personales y un pago para reclamarlo.",
            "Los premios inesperados, la presión para actuar y las solicitudes de datos o pagos pueden indicar un engaño.",
            "No ingreses datos ni pagues. Consultá las promociones únicamente en canales oficiales."),
        new AdScenario("CLASES GRATUITAS DE DIBUJO", "Centro Cultural", "Conocé los horarios y actividades del taller gratuito. La información está disponible en los canales oficiales del centro cultural.", "VER INFORMACIÓN", false,
            "El mensaje no promete un premio extraordinario, no amenaza al usuario ni solicita instalar archivos o entregar datos para continuar.",
            "No aparecen señales evidentes de descarga forzada o presión. Aun así, conviene verificar que el anunciante sea realmente el centro cultural.",
            "Comprobá la identidad del anunciante y escribí la dirección oficial por tu cuenta antes de compartir información."),
        new AdScenario("¡SE DETECTARON 5 VIRUS!", "Alerta de Seguridad", "Tu dispositivo está infectado. Instalá nuestro limpiador desconocido en los próximos 30 segundos para evitar daños.", "LIMPIAR DISPOSITIVO", true,
            "Un anuncio no puede confirmar por sí solo que tu dispositivo tenga virus. Intenta asustarte y forzarte a instalar una aplicación desconocida.",
            "Las alertas alarmistas, las cuentas regresivas y las aplicaciones desconocidas son señales de riesgo.",
            "Cerrá el anuncio. Para revisar el dispositivo, utilizá las herramientas de seguridad que ya conocés y fuentes oficiales."),
        new AdScenario("LECTURA PARA LA ESCUELA", "Biblioteca Digital", "Explorá recomendaciones de lectura y recursos gratuitos. Podés consultar el catálogo y verificar quién publica el contenido.", "CONOCER EL CATÁLOGO", false,
            "La propuesta ofrece información sin exigir una descarga inmediata, un pago ni datos personales para reclamar algo.",
            "El mensaje no presenta señales claras de engaño, pero siempre hay que comprobar la identidad del servicio y del desarrollador.",
            "Buscá el nombre del servicio en una fuente oficial y revisá quién lo publica antes de instalar cualquier aplicación."),
        new AdScenario("DESCARGA SECRETA", "Juegos Premium Gratis", "Obtené todos los juegos pagos gratis. Desactivá la protección del teléfono e instalá este archivo fuera de la tienda.", "DESCARGAR APK", true,
            "Pide desactivar una protección y descargar un archivo desde fuera de una tienda confiable.",
            "Desactivar la seguridad y descargar archivos desconocidos aumenta el riesgo de instalar software malicioso.",
            "No desactives las protecciones ni instales archivos de anuncios. Buscá aplicaciones en tiendas oficiales."),
        new AdScenario("NUEVO TALLER DE CIENCIA", "Club de Ciencias", "Conocé experimentos y actividades para estudiantes. Consultá fechas, requisitos y responsables en la página oficial de la institución.", "VER ACTIVIDADES", false,
            "Presenta información concreta y propone consultar los canales oficiales, sin pedir datos sensibles ni instalar archivos.",
            "No se observan señales evidentes de presión o descarga sospechosa. La identidad del anunciante debe verificarse igualmente.",
            "Comprobá que la página pertenezca a la institución antes de registrarte o compartir información."),
        new AdScenario("TU CUENTA SERÁ BLOQUEADA", "Soporte de Cuenta", "Detectamos un problema. Descargá este archivo y escribí tu contraseña para evitar que tu cuenta sea eliminada hoy.", "SOLUCIONAR PROBLEMA", true,
            "Amenaza con bloquear una cuenta, solicita una contraseña y propone descargar un archivo.",
            "Las amenazas, la solicitud de contraseñas y las descargas inesperadas son señales muy importantes de alerta.",
            "No compartas contraseñas ni descargues el archivo. Entrá a tu cuenta desde la aplicación o dirección oficial."),
        new AdScenario("ACTIVÁ TU CUENTA PREMIUM GRATIS", "Beneficios Plus", "Tu cuenta puede recibir funciones premium sin costo. Iniciá sesión con tu correo y contraseña para activar el beneficio antes de que termine la promoción.", "ACTIVAR BENEFICIO", true,
            "El anuncio ofrece un beneficio gratuito a cambio de iniciar sesión desde su propia promoción. Podría ser un intento de robar credenciales.",
            "Los beneficios inesperados y los formularios de inicio de sesión abiertos desde anuncios son señales de riesgo.",
            "No ingreses tu contraseña desde el anuncio. Abrí la aplicación oficial y comprobá allí si existe alguna promoción.")
    };

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Click Inteligente necesita estar agregado a un objeto que tenga un Canvas.", this);
            enabled = false;
            return;
        }

        EnsureEventSystem();
        canvas.transform.SetAsLastSibling();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        BuildMenu();
    }

    private void OnDestroy()
    {
        StopTutorialVideo();
    }

    private void EnsureEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(eventSystemObject);
            return;
        }

        if (eventSystem.GetComponent<BaseInputModule>() == null)
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
    }

    private void ClearScreen()
    {
        StopTutorialVideo();

        if (screenRoot != null)
            Destroy(screenRoot.gameObject);

        GameObject root = new GameObject("ClickInteligenteScreen", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        screenRoot = root.GetComponent<RectTransform>();
        screenRoot.SetParent(canvas.transform, false);
        Stretch(screenRoot);
        root.transform.SetAsLastSibling();

        Image background = root.GetComponent<Image>();
        background.color = Hex("#101827");
        background.raycastTarget = true;
    }

    private void BuildMenu()
    {
        ClearScreen();
        AddText(screenRoot, "Brand", "CLICK INTELIGENTE", new Vector2(0.06f, 0.89f), new Vector2(0.94f, 0.98f), Vector2.zero, Vector2.zero, 28, Hex("#70D6FF"), TextAlignmentOptions.Left);
        AddText(screenRoot, "Title", "Antes de hacer clic, pensá", new Vector2(0.08f, 0.63f), new Vector2(0.92f, 0.83f), Vector2.zero, Vector2.zero, 58, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "Intro", "Aprendé a reconocer anuncios engañosos que intentan que descargues aplicaciones o archivos peligrosos. Vas a practicar con ejemplos ficticios y recibir explicaciones después de cada decisión.", new Vector2(0.16f, 0.40f), new Vector2(0.84f, 0.61f), Vector2.zero, Vector2.zero, 27, Hex("#D6E2F0"), TextAlignmentOptions.Center);
        AddText(screenRoot, "Audience", "Actividad educativa para chicos y chicas de 10 a 16 años", new Vector2(0.12f, 0.28f), new Vector2(0.88f, 0.36f), Vector2.zero, Vector2.zero, 21, Hex("#A8B8CC"), TextAlignmentOptions.Center);
        AddButton(screenRoot, "Comenzar tutorial", new Vector2(0.27f, 0.12f), new Vector2(0.73f, 0.23f), Hex("#147D92"), () =>
        {
            tutorialPage = 0;
            BuildTutorial();
        });
        AddText(screenRoot, "Safety", "Todos los anuncios y ejemplos son simulados. No se descargan archivos ni se abren enlaces.", new Vector2(0.10f, 0.035f), new Vector2(0.90f, 0.085f), Vector2.zero, Vector2.zero, 17, Hex("#9FB0C5"), TextAlignmentOptions.Center);
    }

    private void BuildTutorial()
    {
        ClearScreen();
        string[] titles =
        {
            "1. Frená y observá",
            "2. Buscá señales de alerta",
            "3. Elegí cómo protegerte"
        };
        string[] descriptions =
        {
            "Antes de tocar un anuncio, leé el mensaje completo. No actúes por apuro y no descargues archivos porque una publicidad te lo pida.",
            "Desconfiá de los premios inesperados, las ofertas increíbles, las amenazas de virus, las cuentas regresivas y las solicitudes de instalar aplicaciones desconocidas.",
            "No compartas contraseñas ni datos personales desde un anuncio. Verificá al anunciante por canales oficiales y buscá aplicaciones desde tiendas confiables."
        };

        AddHeader("TUTORIAL", titles[tutorialPage], "Leé cada consejo. Después vas a ponerlo en práctica.");
        CreateCard(screenRoot, "LessonCard", new Vector2(0.08f, 0.25f), new Vector2(0.92f, 0.76f), Hex("#1B2A3D"));
        AddText(screenRoot, "LessonText", descriptions[tutorialPage], new Vector2(0.13f, 0.48f), new Vector2(0.87f, 0.70f), Vector2.zero, Vector2.zero, 30, Color.white, TextAlignmentOptions.Center);

        string[] labels = tutorialPage == 0
            ? new[] { "No hagas clic por impulso", "No descargues desde anuncios", "Verificá quién publica" }
            : tutorialPage == 1
                ? new[] { "Urgencia o amenazas", "Premios demasiado buenos", "Archivos o apps desconocidas" }
                : new[] { "No compartas contraseñas", "Usá canales oficiales", "Pedí ayuda si tenés dudas" };

        for (int i = 0; i < labels.Length; i++)
        {
            float x0 = 0.12f + i * 0.255f;
            CreateCard(screenRoot, "Tip" + i, new Vector2(x0, 0.31f), new Vector2(x0 + 0.235f, 0.45f), Hex("#243A50"));
            AddText(screenRoot, "TipText" + i, labels[i], new Vector2(x0 + 0.01f, 0.325f), new Vector2(x0 + 0.225f, 0.435f), Vector2.zero, Vector2.zero, 20, Hex("#BCEBFF"), TextAlignmentOptions.Center);
        }

        if (tutorialPage == 2)
        {
            VideoClip clip = GetTutorialVideo();
            if (clip != null)
                BuildVideoPanel(clip);
            else
            {
                CreateCard(screenRoot, "VideoPlaceholder", new Vector2(0.20f, 0.12f), new Vector2(0.80f, 0.23f), Hex("#243A50"));
                AddText(screenRoot, "VideoPlaceholderText", "VIDEO INFORMATIVO: espacio preparado para incorporar el clip educativo desde el Inspector.", new Vector2(0.22f, 0.13f), new Vector2(0.78f, 0.22f), Vector2.zero, Vector2.zero, 17, Hex("#C8D5E5"), TextAlignmentOptions.Center);
            }
        }

        if (tutorialPage > 0)
            AddButton(screenRoot, "Anterior", new Vector2(0.08f, 0.045f), new Vector2(0.27f, 0.13f), Hex("#34465D"), () =>
            {
                tutorialPage--;
                BuildTutorial();
            });

        string nextLabel = tutorialPage == 2 ? "Comenzar práctica" : "Continuar";
        AddButton(screenRoot, nextLabel, new Vector2(0.70f, 0.045f), new Vector2(0.92f, 0.13f), Hex("#147D92"), () =>
        {
            if (tutorialPage < 2)
            {
                tutorialPage++;
                BuildTutorial();
            }
            else
            {
                StartPractice();
            }
        });
    }

    private VideoClip GetTutorialVideo()
    {
        if (videosTutoriales == null || tutorialPage >= videosTutoriales.Length)
            return null;

        return videosTutoriales[tutorialPage];
    }

    private void BuildVideoPanel(VideoClip clip)
    {
        CreateCard(screenRoot, "VideoPanel", new Vector2(0.30f, 0.10f), new Vector2(0.70f, 0.24f), Hex("#080D16"));
        GameObject imageObject = new GameObject("TutorialVideo", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.SetParent(screenRoot, false);
        imageRect.anchorMin = new Vector2(0.32f, 0.115f);
        imageRect.anchorMax = new Vector2(0.68f, 0.225f);
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        videoTexture = new RenderTexture(640, 360, 0);
        videoTexture.Create();
        imageObject.GetComponent<RawImage>().texture = videoTexture;
        imageObject.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        imageObject.GetComponent<AspectRatioFitter>().aspectRatio = 16f / 9f;

        GameObject playerObject = new GameObject("TutorialVideoPlayer");
        playerObject.transform.SetParent(screenRoot, false);
        videoPlayer = playerObject.AddComponent<VideoPlayer>();
        AudioSource audioSource = playerObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = clip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.isLooping = false;
        videoPlayer.prepareCompleted += player => player.Play();
        videoPlayer.Prepare();
    }

    private void StartPractice()
    {
        round.Clear();
        round.AddRange(ScenarioBank);
        for (int i = round.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            AdScenario temp = round[i];
            round[i] = round[j];
            round[j] = temp;
        }

        int count = Mathf.Clamp(preguntasPorPartida, 5, ScenarioBank.Length);
        if (round.Count > count)
            round.RemoveRange(count, round.Count - count);

        questionIndex = 0;
        correctAnswers = 0;
        incorrectAnswers = 0;
        answered = false;
        BuildPracticeQuestion();
    }

    private void BuildPracticeQuestion()
    {
        ClearScreen();
        answered = false;
        AdScenario scenario = round[questionIndex];
        AddText(screenRoot, "PracticeBrand", "CLICK INTELIGENTE  ·  APRENDÉ A NAVEGAR SEGURO", new Vector2(0.025f, 0.925f), new Vector2(0.70f, 0.985f), Vector2.zero, Vector2.zero, 22, Hex("#70D6FF"), TextAlignmentOptions.Left);
        AddText(screenRoot, "PracticeProgress", "ANUNCIO " + (questionIndex + 1) + " / " + round.Count, new Vector2(0.78f, 0.925f), new Vector2(0.975f, 0.985f), Vector2.zero, Vector2.zero, 19, Color.white, TextAlignmentOptions.Right);
        BuildPlatformMockup(questionIndex % 3, scenario, questionIndex / 3);
        AddButton(screenRoot, "Parece seguro", new Vector2(0.12f, 0.085f), new Vector2(0.47f, 0.18f), Hex("#237A58"), () => Answer(false));
        AddButton(screenRoot, "Reportar como sospechoso", new Vector2(0.53f, 0.085f), new Vector2(0.88f, 0.18f), Hex("#A74646"), () => Answer(true));
        AddText(screenRoot, "PracticeSafety", "CONSEJO: frená, revisá quién publica y no descargues archivos desde anuncios.", new Vector2(0.06f, 0.015f), new Vector2(0.94f, 0.065f), Vector2.zero, Vector2.zero, 16, Hex("#BCEBFF"), TextAlignmentOptions.Center);
    }

    private void Answer(bool reportedAsSuspicious)
    {
        if (answered)
            return;

        answered = true;
        AdScenario scenario = round[questionIndex];
        lastAnswerWasCorrect = reportedAsSuspicious == scenario.isSuspicious;

        if (lastAnswerWasCorrect)
            correctAnswers++;
        else
            incorrectAnswers++;

        BuildFeedback();
    }

    private void BuildFeedback()
    {
        ClearScreen();
        AdScenario scenario = round[questionIndex];
        string heading = lastAnswerWasCorrect ? "¡Muy bien!" : "Repasemos esta decisión";
        string status = lastAnswerWasCorrect ? "Tu clasificación fue correcta." : "La clasificación correcta era: " + (scenario.isSuspicious ? "REPORTAR COMO SOSPECHOSO" : "PARECE SEGURO");
        AddHeader("RETROALIMENTACIÓN", heading, status);
        CreateCard(screenRoot, "FeedbackCard", new Vector2(0.09f, 0.24f), new Vector2(0.91f, 0.74f), Hex("#1B2A3D"));
        AddText(screenRoot, "Explanation", scenario.explanation, new Vector2(0.14f, 0.55f), new Vector2(0.86f, 0.69f), Vector2.zero, Vector2.zero, 25, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "Signals", "Señales para recordar: " + scenario.signals, new Vector2(0.14f, 0.40f), new Vector2(0.86f, 0.54f), Vector2.zero, Vector2.zero, 21, Hex("#BCEBFF"), TextAlignmentOptions.Center);
        AddText(screenRoot, "Advice", "Qué hacer: " + scenario.advice, new Vector2(0.14f, 0.27f), new Vector2(0.86f, 0.39f), Vector2.zero, Vector2.zero, 20, Hex("#DCE6F2"), TextAlignmentOptions.Center);
        AddText(screenRoot, "Score", "Correctas: " + correctAnswers + "     Incorrectas: " + incorrectAnswers, new Vector2(0.15f, 0.16f), new Vector2(0.85f, 0.22f), Vector2.zero, Vector2.zero, 19, Hex("#A8B8CC"), TextAlignmentOptions.Center);
        AddButton(screenRoot, questionIndex == round.Count - 1 ? "Ver resultados" : "Siguiente anuncio", new Vector2(0.65f, 0.055f), new Vector2(0.91f, 0.14f), Hex("#147D92"), () =>
        {
            questionIndex++;
            if (questionIndex >= round.Count)
                BuildResults();
            else
                BuildPracticeQuestion();
        });
    }

    private void BuildResults()
    {
        ClearScreen();
        int total = correctAnswers + incorrectAnswers;
        int percentage = total == 0 ? 0 : Mathf.RoundToInt((float)correctAnswers / total * 100f);
        string message = percentage >= 80
            ? "¡Excelente trabajo! Reconociste la mayoría de las señales de riesgo."
            : percentage >= 60
                ? "¡Buen avance! Repasá las explicaciones para reconocer más señales."
                : "Seguí practicando. Recordá frenar, verificar la fuente y evitar descargas desde anuncios.";

        AddHeader("RESULTADOS", "Terminaste la práctica", "Este resultado corresponde a esta partida.");
        CreateCard(screenRoot, "ResultCard", new Vector2(0.16f, 0.30f), new Vector2(0.84f, 0.72f), Hex("#1B2A3D"));
        AddText(screenRoot, "Percentage", percentage + "%", new Vector2(0.25f, 0.53f), new Vector2(0.75f, 0.68f), Vector2.zero, Vector2.zero, 66, Hex("#70D6FF"), TextAlignmentOptions.Center);
        AddText(screenRoot, "CorrectIncorrect", "Correctas: " + correctAnswers + "     Incorrectas: " + incorrectAnswers + "     Total: " + total, new Vector2(0.20f, 0.43f), new Vector2(0.80f, 0.51f), Vector2.zero, Vector2.zero, 21, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "ResultMessage", message, new Vector2(0.22f, 0.32f), new Vector2(0.78f, 0.42f), Vector2.zero, Vector2.zero, 22, Hex("#DCE6F2"), TextAlignmentOptions.Center);
        AddButton(screenRoot, "Repetir práctica", new Vector2(0.12f, 0.12f), new Vector2(0.47f, 0.23f), Hex("#147D92"), StartPractice);
        AddButton(screenRoot, "Volver al menú", new Vector2(0.53f, 0.12f), new Vector2(0.88f, 0.23f), Hex("#34465D"), BuildMenu);
        AddText(screenRoot, "ResearchNote", "La puntuación del juego no reemplaza los cuestionarios inicial y final previstos en la investigación.", new Vector2(0.08f, 0.035f), new Vector2(0.92f, 0.085f), Vector2.zero, Vector2.zero, 15, Hex("#9FB0C5"), TextAlignmentOptions.Center);
    }


    private void BuildPlatformMockup(int platform, AdScenario scenario, int placementIndex)
    {
        Color page = platform == 2 ? Hex("#101014") : Hex("#F8F9FA");
        Color ink = platform == 2 ? Color.white : Hex("#202124");
        Color muted = platform == 2 ? Hex("#B9BBC3") : Hex("#60646B");
        CreateCard(screenRoot, "PlatformPage", new Vector2(0.015f, 0.215f), new Vector2(0.985f, 0.91f), page);

        if (platform == 0)
            BuildYouTubeAdPage(scenario, placementIndex, ink, muted);
        else if (platform == 1)
            BuildPlayStoreAdPage(scenario, placementIndex, ink, muted);
        else
            BuildTikTokAdPage(scenario, placementIndex);
    }
    private void BuildYouTubeAdPage(AdScenario scenario, int placementIndex, Color ink, Color muted)
    {
        CreateCard(screenRoot, "YouTubeHeader", new Vector2(0.015f, 0.835f), new Vector2(0.985f, 0.91f), Color.white);
        AddText(screenRoot, "YouTubeMenu", "☰", new Vector2(0.025f, 0.85f), new Vector2(0.06f, 0.895f), Vector2.zero, Vector2.zero, 23, ink, TextAlignmentOptions.Center);
        AddText(screenRoot, "YouTubeLogo", "▶ YouTube", new Vector2(0.065f, 0.85f), new Vector2(0.23f, 0.895f), Vector2.zero, Vector2.zero, 23, Hex("#E62117"), TextAlignmentOptions.Left);
        CreateCard(screenRoot, "YouTubeSearch", new Vector2(0.34f, 0.852f), new Vector2(0.72f, 0.892f), Hex("#F1F1F1"));
        AddText(screenRoot, "YouTubeSearchText", "Buscar", new Vector2(0.36f, 0.855f), new Vector2(0.70f, 0.89f), Vector2.zero, Vector2.zero, 16, Hex("#60646B"), TextAlignmentOptions.Left);
        AddText(screenRoot, "YouTubeSignIn", "Iniciar sesión", new Vector2(0.82f, 0.85f), new Vector2(0.97f, 0.895f), Vector2.zero, Vector2.zero, 15, Hex("#065FD4"), TextAlignmentOptions.Center);

        bool inStream = placementIndex % 3 == 1;
        bool shorts = placementIndex % 3 == 2;
        string format = shorts ? "PUBLICIDAD EN YOUTUBE SHORTS" : inStream ? "ANUNCIO IN-STREAM · VIDEO REPRODUCIÉNDOSE" : "IN-FEED · RESULTADOS Y RECOMENDACIONES";
        AddText(screenRoot, "YouTubeAdFormat", format, new Vector2(0.16f, 0.785f), new Vector2(0.80f, 0.82f), Vector2.zero, Vector2.zero, 14, Hex("#B3261E"), TextAlignmentOptions.Left);

        CreateCard(screenRoot, "YouTubeSidebar", new Vector2(0.025f, 0.245f), new Vector2(0.14f, 0.82f), Hex("#FFFFFF"));
        string[] nav = { "⌂  Inicio", "▶  Shorts", "▣  Suscripciones", "◷  Historial", "▤  Biblioteca", "Explorar", "Música", "Videojuegos" };
        for (int i = 0; i < nav.Length; i++)
            AddText(screenRoot, "YouTubeNav" + i, nav[i], new Vector2(0.035f, 0.735f - i * 0.061f), new Vector2(0.135f, 0.775f - i * 0.061f), Vector2.zero, Vector2.zero, 13, ink, TextAlignmentOptions.Left);

        if (shorts)
        {
            CreateCard(screenRoot, "YouTubeShortFrame", new Vector2(0.365f, 0.245f), new Vector2(0.625f, 0.775f), Hex("#080808"));
            CreateAdArtwork(scenario, new Vector2(0.375f, 0.315f), new Vector2(0.615f, 0.765f));
            AddText(screenRoot, "YouTubeShortTop", "Shorts     ⋮", new Vector2(0.38f, 0.735f), new Vector2(0.61f, 0.77f), Vector2.zero, Vector2.zero, 16, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "YouTubeShortActions", "♡\n12 mil\n\n▢\n1.2 mil\n\n↗\nCompartir", new Vector2(0.625f, 0.39f), new Vector2(0.71f, 0.68f), Vector2.zero, Vector2.zero, 13, ink, TextAlignmentOptions.Center);
            AddText(screenRoot, "YouTubeShortSponsored", "Patrocinado · " + scenario.advertiser, new Vector2(0.375f, 0.265f), new Vector2(0.615f, 0.315f), Vector2.zero, Vector2.zero, 13, Color.white, TextAlignmentOptions.Left);
        }
        else if (inStream)
        {
            CreateCard(screenRoot, "YouTubePlayer", new Vector2(0.16f, 0.435f), new Vector2(0.69f, 0.765f), Hex("#111820"));
            CreateAdArtwork(scenario, new Vector2(0.175f, 0.47f), new Vector2(0.675f, 0.755f));
            CreateCard(screenRoot, "YouTubeSkipButton", new Vector2(0.535f, 0.445f), new Vector2(0.68f, 0.49f), Hex("#171717"));
            AddText(screenRoot, "YouTubeSkipText", "Omitir anuncio  ›", new Vector2(0.54f, 0.45f), new Vector2(0.675f, 0.485f), Vector2.zero, Vector2.zero, 13, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "YouTubeAdTimer", "Anuncio · 0:05", new Vector2(0.18f, 0.44f), new Vector2(0.34f, 0.475f), Vector2.zero, Vector2.zero, 13, Color.white, TextAlignmentOptions.Left);
            AddText(screenRoot, "YouTubeVideoTitle", scenario.title, new Vector2(0.16f, 0.375f), new Vector2(0.69f, 0.425f), Vector2.zero, Vector2.zero, 19, ink, TextAlignmentOptions.Left);
            AddText(screenRoot, "YouTubeChannel", scenario.advertiser + " · Patrocinado", new Vector2(0.16f, 0.33f), new Vector2(0.69f, 0.37f), Vector2.zero, Vector2.zero, 14, muted, TextAlignmentOptions.Left);
            AddText(screenRoot, "YouTubeDescription", scenario.message, new Vector2(0.16f, 0.25f), new Vector2(0.69f, 0.325f), Vector2.zero, Vector2.zero, 13, ink, TextAlignmentOptions.Left);
            CreateCard(screenRoot, "YouTubeCTA", new Vector2(0.72f, 0.27f), new Vector2(0.95f, 0.35f), Hex("#065FD4"));
            AddText(screenRoot, "YouTubeCTAText", scenario.callToAction, new Vector2(0.73f, 0.28f), new Vector2(0.94f, 0.34f), Vector2.zero, Vector2.zero, 13, Color.white, TextAlignmentOptions.Center);
        }
        else
        {
            string[] titles = { scenario.title, "Consejos para navegar seguro", "Novedades y recomendaciones" };
            for (int i = 0; i < 3; i++)
            {
                float x = 0.16f + i * 0.265f;
                CreateCard(screenRoot, "YouTubeFeedCard" + i, new Vector2(x, 0.43f), new Vector2(x + 0.245f, 0.765f), Color.white);
                if (i == 0)
                {
                    CreateAdArtwork(scenario, new Vector2(x + 0.008f, 0.565f), new Vector2(x + 0.237f, 0.75f));
                    AddText(screenRoot, "YouTubeSponsored", "Anuncio · " + scenario.advertiser, new Vector2(x + 0.01f, 0.515f), new Vector2(x + 0.235f, 0.56f), Vector2.zero, Vector2.zero, 12, muted, TextAlignmentOptions.Left);
                    AddText(screenRoot, "YouTubeAdTitle", scenario.title, new Vector2(x + 0.01f, 0.465f), new Vector2(x + 0.235f, 0.515f), Vector2.zero, Vector2.zero, 15, ink, TextAlignmentOptions.Left);
                    CreateCard(screenRoot, "YouTubeAdCTA", new Vector2(x + 0.01f, 0.44f), new Vector2(x + 0.235f, 0.465f), Hex("#065FD4"));
                    AddText(screenRoot, "YouTubeAdCTAtext", scenario.callToAction, new Vector2(x + 0.012f, 0.441f), new Vector2(x + 0.233f, 0.464f), Vector2.zero, Vector2.zero, 11, Color.white, TextAlignmentOptions.Center);
                }
                else
                {
                    CreateCard(screenRoot, "YouTubeOrganicThumb" + i, new Vector2(x + 0.008f, 0.565f), new Vector2(x + 0.237f, 0.75f), i == 1 ? Hex("#355B72") : Hex("#765B42"));
                    AddText(screenRoot, "YouTubeOrganicArt" + i, i == 1 ? "GUÍA DE SEGURIDAD" : "NUEVO CONTENIDO", new Vector2(x + 0.015f, 0.62f), new Vector2(x + 0.23f, 0.70f), Vector2.zero, Vector2.zero, 16, Color.white, TextAlignmentOptions.Center);
                    AddText(screenRoot, "YouTubeOrganicTitle" + i, titles[i], new Vector2(x + 0.01f, 0.49f), new Vector2(x + 0.235f, 0.56f), Vector2.zero, Vector2.zero, 14, ink, TextAlignmentOptions.Left);
                    AddText(screenRoot, "YouTubeOrganicMeta" + i, i == 1 ? "Canal educativo · 24 mil vistas" : "Canal recomendado · 8 mil vistas", new Vector2(x + 0.01f, 0.45f), new Vector2(x + 0.235f, 0.49f), Vector2.zero, Vector2.zero, 11, muted, TextAlignmentOptions.Left);
                }
            }
        }

        AddText(screenRoot, "YouTubePageFooter", "Inicio     Shorts     Suscripciones     Biblioteca", new Vector2(0.16f, 0.245f), new Vector2(0.95f, 0.275f), Vector2.zero, Vector2.zero, 12, muted, TextAlignmentOptions.Left);
    }
    private void BuildPlayStoreAdPage(AdScenario scenario, int placementIndex, Color ink, Color muted)
    {
        CreateCard(screenRoot, "PlayStoreHeader", new Vector2(0.015f, 0.835f), new Vector2(0.985f, 0.91f), Color.white);
        AddText(screenRoot, "PlayStoreLogo", "▶ Google Play", new Vector2(0.04f, 0.85f), new Vector2(0.25f, 0.895f), Vector2.zero, Vector2.zero, 22, Hex("#188B55"), TextAlignmentOptions.Left);
        CreateCard(screenRoot, "PlayStoreSearch", new Vector2(0.42f, 0.852f), new Vector2(0.94f, 0.892f), Hex("#F1F3F4"));
        AddText(screenRoot, "PlayStoreSearchText", "Buscar aplicaciones y juegos", new Vector2(0.44f, 0.855f), new Vector2(0.92f, 0.89f), Vector2.zero, Vector2.zero, 14, muted, TextAlignmentOptions.Left);
        AddText(screenRoot, "PlayStoreTabs", "Para ti       Top charts       Niños       Categorías", new Vector2(0.06f, 0.785f), new Vector2(0.94f, 0.82f), Vector2.zero, Vector2.zero, 15, ink, TextAlignmentOptions.Left);

        if (placementIndex % 3 == 0)
        {
            AddText(screenRoot, "PlayStoreSearchTitle", "Resultados de búsqueda", new Vector2(0.07f, 0.735f), new Vector2(0.90f, 0.775f), Vector2.zero, Vector2.zero, 17, ink, TextAlignmentOptions.Left);
            for (int i = 0; i < 4; i++)
            {
                float top = 0.715f - i * 0.12f;
                CreateCard(screenRoot, "PlayStoreResult" + i, new Vector2(0.07f, top - 0.105f), new Vector2(0.93f, top), Color.white);
                CreateCard(screenRoot, "PlayStoreAppIcon" + i, new Vector2(0.085f, top - 0.09f), new Vector2(0.16f, top - 0.02f), i == 0 ? Hex("#6B4DB8") : new Color(0.2f + i * 0.12f, 0.45f, 0.65f, 1f));
                AddText(screenRoot, "PlayStoreAppIconText" + i, i == 0 ? "★" : "●", new Vector2(0.09f, top - 0.085f), new Vector2(0.155f, top - 0.025f), Vector2.zero, Vector2.zero, 23, Color.white, TextAlignmentOptions.Center);
                AddText(screenRoot, "PlayStoreResultName" + i, i == 0 ? scenario.advertiser : new[] { "Puzzle Diario", "Aprender en Casa", "Editor de Fotos" }[i - 1], new Vector2(0.18f, top - 0.045f), new Vector2(0.58f, top - 0.012f), Vector2.zero, Vector2.zero, 14, ink, TextAlignmentOptions.Left);
                AddText(screenRoot, "PlayStoreResultMeta" + i, i == 0 ? "Patrocinado · " + scenario.message : "Aplicación · 4," + (8 - i) + " ★ · 10 M+ descargas", new Vector2(0.18f, top - 0.09f), new Vector2(0.70f, top - 0.047f), Vector2.zero, Vector2.zero, 11, muted, TextAlignmentOptions.Left);
                if (i == 0)
                {
                    CreateCard(screenRoot, "PlayStoreAdTag", new Vector2(0.72f, top - 0.075f), new Vector2(0.91f, top - 0.025f), Hex("#E8F5E9"));
                    AddText(screenRoot, "PlayStoreAdInstall", "INSTALAR", new Vector2(0.725f, top - 0.07f), new Vector2(0.905f, top - 0.03f), Vector2.zero, Vector2.zero, 12, Hex("#188B55"), TextAlignmentOptions.Center);
                }
            }
        }
        else if (placementIndex % 3 == 1)
        {
            AddText(screenRoot, "PlayStoreSponsoredHeading", "Anuncios · Sugeridos para ti", new Vector2(0.07f, 0.73f), new Vector2(0.92f, 0.775f), Vector2.zero, Vector2.zero, 18, ink, TextAlignmentOptions.Left);
            for (int i = 0; i < 3; i++)
            {
                float x = 0.08f + i * 0.295f;
                CreateCard(screenRoot, "PlayStoreSuggested" + i, new Vector2(x, 0.39f), new Vector2(x + 0.26f, 0.71f), Color.white);
                CreateCard(screenRoot, "PlayStoreSuggestedArt" + i, new Vector2(x + 0.012f, 0.53f), new Vector2(x + 0.248f, 0.695f), i == 0 ? Hex("#6B4DB8") : i == 1 ? Hex("#267A73") : Hex("#3C659A"));
                AddText(screenRoot, "PlayStoreSuggestedSymbol" + i, i == 0 ? "★" : i == 1 ? "✦" : "◆", new Vector2(x + 0.025f, 0.56f), new Vector2(x + 0.235f, 0.67f), Vector2.zero, Vector2.zero, 44, Color.white, TextAlignmentOptions.Center);
                AddText(screenRoot, "PlayStoreSuggestedName" + i, i == 0 ? scenario.advertiser : new[] { "", "Hábitos Plus", "Foto Fácil" }[i], new Vector2(x + 0.015f, 0.485f), new Vector2(x + 0.245f, 0.53f), Vector2.zero, Vector2.zero, 14, ink, TextAlignmentOptions.Left);
                AddText(screenRoot, "PlayStoreSuggestedInfo" + i, i == 0 ? "Patrocinado · " + scenario.title : "4," + (9 - i) + " ★ · Gratis", new Vector2(x + 0.015f, 0.44f), new Vector2(x + 0.245f, 0.485f), Vector2.zero, Vector2.zero, 11, muted, TextAlignmentOptions.Left);
                AddText(screenRoot, "PlayStoreSuggestedCategory" + i, i == 0 ? "Contiene anuncios" : "Compras en la aplicación", new Vector2(x + 0.015f, 0.405f), new Vector2(x + 0.245f, 0.44f), Vector2.zero, Vector2.zero, 10, muted, TextAlignmentOptions.Left);
            }
            AddText(screenRoot, "PlayStoreMoreApps", "Recomendados para ti", new Vector2(0.08f, 0.31f), new Vector2(0.90f, 0.36f), Vector2.zero, Vector2.zero, 17, ink, TextAlignmentOptions.Left);
            AddText(screenRoot, "PlayStoreMoreAppsItems", "Juegos populares       Productividad       Educación", new Vector2(0.08f, 0.25f), new Vector2(0.92f, 0.30f), Vector2.zero, Vector2.zero, 13, muted, TextAlignmentOptions.Left);
        }
        else
        {
            CreateCard(screenRoot, "PlayStoreDetail", new Vector2(0.07f, 0.29f), new Vector2(0.93f, 0.75f), Color.white);
            CreateCard(screenRoot, "PlayStoreDetailIcon", new Vector2(0.10f, 0.61f), new Vector2(0.23f, 0.73f), Hex("#5B4AA5"));
            AddText(screenRoot, "PlayStoreDetailIconSymbol", "★", new Vector2(0.11f, 0.62f), new Vector2(0.22f, 0.72f), Vector2.zero, Vector2.zero, 48, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "PlayStoreDetailName", scenario.advertiser, new Vector2(0.26f, 0.675f), new Vector2(0.75f, 0.735f), Vector2.zero, Vector2.zero, 22, ink, TextAlignmentOptions.Left);
            AddText(screenRoot, "PlayStoreDetailDeveloper", "Desarrollador no verificado · Contiene anuncios", new Vector2(0.26f, 0.63f), new Vector2(0.76f, 0.675f), Vector2.zero, Vector2.zero, 13, muted, TextAlignmentOptions.Left);
            AddText(screenRoot, "PlayStoreDetailStats", "4,8 ★       10 M+ descargas       Para mayores de 3", new Vector2(0.10f, 0.56f), new Vector2(0.88f, 0.61f), Vector2.zero, Vector2.zero, 13, muted, TextAlignmentOptions.Left);
            CreateCard(screenRoot, "PlayStoreDetailInstall", new Vector2(0.10f, 0.49f), new Vector2(0.90f, 0.545f), Hex("#188B55"));
            AddText(screenRoot, "PlayStoreDetailInstallText", "INSTALAR · " + scenario.callToAction, new Vector2(0.11f, 0.495f), new Vector2(0.89f, 0.54f), Vector2.zero, Vector2.zero, 14, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "PlayStoreScreenshotsTitle", "Acerca de esta aplicación", new Vector2(0.10f, 0.425f), new Vector2(0.90f, 0.47f), Vector2.zero, Vector2.zero, 17, ink, TextAlignmentOptions.Left);
            for (int i = 0; i < 3; i++)
            {
                float x = 0.10f + i * 0.275f;
                CreateCard(screenRoot, "PlayStorePreview" + i, new Vector2(x, 0.315f), new Vector2(x + 0.24f, 0.415f), i == 0 ? Hex("#355D8A") : i == 1 ? Hex("#2E7D72") : Hex("#76559A"));
                AddText(screenRoot, "PlayStorePreviewText" + i, i == 0 ? "FUNCIONES" : i == 1 ? "BENEFICIOS" : "CAPTURAS", new Vector2(x + 0.01f, 0.34f), new Vector2(x + 0.23f, 0.39f), Vector2.zero, Vector2.zero, 12, Color.white, TextAlignmentOptions.Center);
            }
        }
        AddText(screenRoot, "PlayStoreBottomNav", "Juegos                         Aplicaciones                         Libros", new Vector2(0.08f, 0.235f), new Vector2(0.92f, 0.27f), Vector2.zero, Vector2.zero, 12, muted, TextAlignmentOptions.Center);
    }
    private void BuildTikTokAdPage(AdScenario scenario, int placementIndex)
    {
        CreateCard(screenRoot, "TikTokAppBackground", new Vector2(0.015f, 0.225f), new Vector2(0.985f, 0.91f), Hex("#08080A"));
        bool carousel = placementIndex % 3 == 1;
        bool topView = placementIndex % 3 == 2;
        string format = carousel ? "ANUNCIO DE COLECCIÓN / CARRUSEL" : topView ? "TOPVIEW · ANUNCIO AL ABRIR LA APLICACIÓN" : "IN-FEED · PÁGINA PARA TI";
        AddText(screenRoot, "TikTokAdFormat", format, new Vector2(0.05f, 0.785f), new Vector2(0.95f, 0.82f), Vector2.zero, Vector2.zero, 14, Hex("#FF6B76"), TextAlignmentOptions.Center);

        CreateCard(screenRoot, "TikTokPhone", new Vector2(0.365f, 0.245f), new Vector2(0.635f, 0.775f), Hex("#17171B"));
        AddText(screenRoot, "TikTokStatus", "9:41                                  ●  Wi-Fi  ▰", new Vector2(0.375f, 0.745f), new Vector2(0.625f, 0.77f), Vector2.zero, Vector2.zero, 10, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "TikTokTabs", "LIVE     Siguiendo     Para ti     ⌕", new Vector2(0.375f, 0.705f), new Vector2(0.625f, 0.745f), Vector2.zero, Vector2.zero, 12, Color.white, TextAlignmentOptions.Center);
        CreateCard(screenRoot, "TikTokVideo", new Vector2(0.375f, 0.285f), new Vector2(0.625f, 0.70f), Hex("#24344A"));
        if (carousel)
        {
            CreateCard(screenRoot, "TikTokCarouselPanel1", new Vector2(0.385f, 0.36f), new Vector2(0.455f, 0.66f), Hex("#6B4A9A"));
            CreateCard(screenRoot, "TikTokCarouselPanel2", new Vector2(0.46f, 0.36f), new Vector2(0.535f, 0.66f), Hex("#286C79"));
            CreateCard(screenRoot, "TikTokCarouselPanel3", new Vector2(0.54f, 0.36f), new Vector2(0.615f, 0.66f), Hex("#9A633C"));
            AddText(screenRoot, "TikTokCarouselText1", "01\nOFERTA", new Vector2(0.39f, 0.45f), new Vector2(0.45f, 0.57f), Vector2.zero, Vector2.zero, 13, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "TikTokCarouselText2", "02\nDETALLES", new Vector2(0.465f, 0.45f), new Vector2(0.53f, 0.57f), Vector2.zero, Vector2.zero, 12, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "TikTokCarouselText3", "03\nPREMIO", new Vector2(0.545f, 0.45f), new Vector2(0.61f, 0.57f), Vector2.zero, Vector2.zero, 13, Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "TikTokCarouselDots", "●  ○  ○", new Vector2(0.40f, 0.31f), new Vector2(0.60f, 0.35f), Vector2.zero, Vector2.zero, 12, Color.white, TextAlignmentOptions.Center);
        }
        else
        {
            CreateAdArtwork(scenario, new Vector2(0.385f, 0.36f), new Vector2(0.615f, 0.685f));
            if (topView)
                AddText(screenRoot, "TikTokTopViewTag", "TOPVIEW · PATROCINADO", new Vector2(0.39f, 0.655f), new Vector2(0.61f, 0.69f), Vector2.zero, Vector2.zero, 11, Color.white, TextAlignmentOptions.Center);
        }
        AddText(screenRoot, "TikTokActions", "＋\n♡\n12 mil\n▢\n1.2 mil\n↗", new Vector2(0.585f, 0.40f), new Vector2(0.625f, 0.64f), Vector2.zero, Vector2.zero, 11, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "TikTokAccount", "●  @" + scenario.advertiser.Replace(" ", "") + "  ·  Publicidad", new Vector2(0.385f, 0.325f), new Vector2(0.61f, 0.36f), Vector2.zero, Vector2.zero, 10, Color.white, TextAlignmentOptions.Left);
        AddText(screenRoot, "TikTokCaption", scenario.message, new Vector2(0.385f, 0.285f), new Vector2(0.61f, 0.325f), Vector2.zero, Vector2.zero, 9, Color.white, TextAlignmentOptions.Left);
        CreateCard(screenRoot, "TikTokCTA", new Vector2(0.385f, 0.255f), new Vector2(0.615f, 0.285f), Hex("#FE2C55"));
        AddText(screenRoot, "TikTokCTAText", carousel ? "Deslizá para ver más  ›" : scenario.callToAction, new Vector2(0.39f, 0.257f), new Vector2(0.61f, 0.283f), Vector2.zero, Vector2.zero, 10, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "TikTokBottomNav", "⌂        ⌕        ＋        ▢        ○", new Vector2(0.38f, 0.23f), new Vector2(0.62f, 0.255f), Vector2.zero, Vector2.zero, 11, Color.white, TextAlignmentOptions.Center);

        CreateCard(screenRoot, "TikTokReferenceNotes", new Vector2(0.68f, 0.34f), new Vector2(0.94f, 0.70f), Hex("#19191F"));
        AddText(screenRoot, "TikTokReferenceTitle", "ELEMENTOS DEL FEED", new Vector2(0.70f, 0.64f), new Vector2(0.92f, 0.69f), Vector2.zero, Vector2.zero, 14, Color.white, TextAlignmentOptions.Left);
        AddText(screenRoot, "TikTokReferenceBody", "• Video vertical\n• Etiqueta de publicidad\n• Perfil y descripción\n• Acciones laterales\n• Botón de llamada a la acción\n• Navegación inferior", new Vector2(0.70f, 0.39f), new Vector2(0.92f, 0.63f), Vector2.zero, Vector2.zero, 12, Hex("#D0D0D6"), TextAlignmentOptions.Left);
    }
    private void CreateAdArtwork(AdScenario scenario, Vector2 anchorMin, Vector2 anchorMax)
    {
        string title = scenario.title.ToUpperInvariant();
        Color baseColor = title.Contains("VIRUS") || title.Contains("BLOQUEADA") || title.Contains("ACTUALIZACIÓN") ? Hex("#7D1725")
            : title.Contains("GANASTE") ? Hex("#7A3DB8")
            : title.Contains("DESCARGA") ? Hex("#153B78")
            : title.Contains("DIBUJO") || title.Contains("LECTURA") || title.Contains("CIENCIA") ? Hex("#167B75")
            : Hex("#174A76");
        float width = anchorMax.x - anchorMin.x;
        float height = anchorMax.y - anchorMin.y;
        float centerX = (anchorMin.x + anchorMax.x) * 0.5f;
        float centerY = (anchorMin.y + anchorMax.y) * 0.5f;
        CreateCard(screenRoot, "AdCreativeArtwork", anchorMin, anchorMax, baseColor);
        CreateCard(screenRoot, "AdCreativeGlow", new Vector2(anchorMin.x + width * 0.04f, anchorMin.y + height * 0.08f), new Vector2(anchorMax.x - width * 0.04f, anchorMax.y - height * 0.08f), Color.Lerp(baseColor, Color.white, 0.16f));

        if (title.Contains("GANASTE"))
        {
            CreateCard(screenRoot, "PrizeBox", new Vector2(centerX - width * 0.16f, centerY - height * 0.16f), new Vector2(centerX + width * 0.16f, centerY + height * 0.16f), Hex("#F7C843"));
            AddText(screenRoot, "PrizeRibbon", "★", new Vector2(centerX - width * 0.12f, centerY - height * 0.12f), new Vector2(centerX + width * 0.12f, centerY + height * 0.12f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 110f, 22f, 42f), Hex("#7A3DB8"), TextAlignmentOptions.Center);
            AddText(screenRoot, "PrizeHeadline", "¡PREMIO SORPRESA!", new Vector2(anchorMin.x + width * 0.04f, anchorMax.y - height * 0.24f), new Vector2(anchorMax.x - width * 0.04f, anchorMax.y - height * 0.05f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 55f, 15f, 24f), Color.white, TextAlignmentOptions.Center);
        }
        else if (title.Contains("VIRUS") || title.Contains("BLOQUEADA") || title.Contains("ACTUALIZACIÓN"))
        {
            CreateCard(screenRoot, "AlertPanel", new Vector2(centerX - width * 0.20f, centerY - height * 0.18f), new Vector2(centerX + width * 0.20f, centerY + height * 0.18f), Hex("#FCE8E6"));
            AddText(screenRoot, "AlertIcon", "!", new Vector2(centerX - width * 0.16f, centerY - height * 0.16f), new Vector2(centerX + width * 0.16f, centerY + height * 0.16f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 125f, 28f, 64f), Hex("#B3261E"), TextAlignmentOptions.Center);
            AddText(screenRoot, "AlertHeadline", "¡ATENCIÓN!", new Vector2(anchorMin.x + width * 0.04f, anchorMax.y - height * 0.24f), new Vector2(anchorMax.x - width * 0.04f, anchorMax.y - height * 0.05f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 58f, 16f, 27f), Color.white, TextAlignmentOptions.Center);
        }
        else if (title.Contains("DESCARGA"))
        {
            AddText(screenRoot, "GameGems", "◆  ◆  ◆", new Vector2(anchorMin.x + width * 0.05f, centerY - height * 0.12f), new Vector2(anchorMax.x - width * 0.05f, centerY + height * 0.12f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 90f, 24f, 46f), Hex("#6BE7FF"), TextAlignmentOptions.Center);
            AddText(screenRoot, "GameHeadline", "TODO GRATIS", new Vector2(anchorMin.x + width * 0.04f, anchorMax.y - height * 0.24f), new Vector2(anchorMax.x - width * 0.04f, anchorMax.y - height * 0.05f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 60f, 17f, 28f), Color.white, TextAlignmentOptions.Center);
        }
        else if (title.Contains("DIBUJO") || title.Contains("LECTURA") || title.Contains("CIENCIA"))
        {
            string symbol = title.Contains("CIENCIA") ? "⚗  ✦  ⚛" : title.Contains("LECTURA") ? "▤  LIBROS  ▤" : "✎  ✿  ✎";
            AddText(screenRoot, "EducationArt", symbol, new Vector2(anchorMin.x + width * 0.04f, centerY - height * 0.16f), new Vector2(anchorMax.x - width * 0.04f, centerY + height * 0.16f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 85f, 22f, 42f), Hex("#F4F7E8"), TextAlignmentOptions.Center);
            AddText(screenRoot, "EducationHeadline", "DESCUBRÍ ALGO NUEVO", new Vector2(anchorMin.x + width * 0.04f, anchorMax.y - height * 0.24f), new Vector2(anchorMax.x - width * 0.04f, anchorMax.y - height * 0.05f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 48f, 14f, 22f), Color.white, TextAlignmentOptions.Center);
        }
        else
        {
            AddText(screenRoot, "DefaultCreative", "▶  " + scenario.advertiser, new Vector2(anchorMin.x + width * 0.04f, centerY - height * 0.14f), new Vector2(anchorMax.x - width * 0.04f, centerY + height * 0.14f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 65f, 18f, 34f), Color.white, TextAlignmentOptions.Center);
            AddText(screenRoot, "DefaultHeadline", scenario.title, new Vector2(anchorMin.x + width * 0.04f, anchorMax.y - height * 0.24f), new Vector2(anchorMax.x - width * 0.04f, anchorMax.y - height * 0.05f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 52f, 15f, 23f), Color.white, TextAlignmentOptions.Center);
        }
        AddText(screenRoot, "CreativeAdMark", "PUBLICIDAD", new Vector2(anchorMin.x + width * 0.025f, anchorMin.y + height * 0.015f), new Vector2(anchorMin.x + width * 0.34f, anchorMin.y + height * 0.12f), Vector2.zero, Vector2.zero, Mathf.Clamp(width * 26f, 10f, 14f), Color.white, TextAlignmentOptions.Left);
    }
    private void AddHeader(string section, string title, string subtitle)
    {
        AddText(screenRoot, "Section", section, new Vector2(0.07f, 0.91f), new Vector2(0.93f, 0.97f), Vector2.zero, Vector2.zero, 18, Hex("#70D6FF"), TextAlignmentOptions.Left);
        AddText(screenRoot, "ScreenTitle", title, new Vector2(0.07f, 0.79f), new Vector2(0.93f, 0.90f), Vector2.zero, Vector2.zero, 38, Color.white, TextAlignmentOptions.Center);
        AddText(screenRoot, "ScreenSubtitle", subtitle, new Vector2(0.10f, 0.73f), new Vector2(0.90f, 0.78f), Vector2.zero, Vector2.zero, 18, Hex("#A8B8CC"), TextAlignmentOptions.Center);
    }

    private void CreateCard(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject card = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rect = card.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = card.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private TextMeshProUGUI AddText(Transform parent, string objectName, string value, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, float fontSize, Color color, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Min(fontSize, Mathf.Max(10, fontSize * 0.65f));
        text.fontSizeMax = fontSize;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
        return text;
    }

    private void AddButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Color color, Action action)
    {
        GameObject buttonObject = new GameObject("Button_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(color.r, color.g, color.b, 0.45f);
        button.colors = colors;
        button.onClick.AddListener(() => action?.Invoke());

        TextMeshProUGUI labelText = AddText(rect, "Label", label, Vector2.zero, Vector2.one, new Vector2(12, 8), new Vector2(-12, -8), 21, Color.white, TextAlignmentOptions.Center);
        labelText.fontStyle = FontStyles.Bold;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString(value, out Color color);
        return color;
    }

    private void StopTutorialVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            Destroy(videoPlayer.gameObject);
            videoPlayer = null;
        }

        if (videoTexture != null)
        {
            videoTexture.Release();
            Destroy(videoTexture);
            videoTexture = null;
        }
    }

    private sealed class AdScenario
    {
        public readonly string title;
        public readonly string advertiser;
        public readonly string message;
        public readonly string callToAction;
        public readonly bool isSuspicious;
        public readonly string explanation;
        public readonly string signals;
        public readonly string advice;

        public AdScenario(string title, string advertiser, string message, string callToAction, bool isSuspicious, string explanation, string signals, string advice)
        {
            this.title = title;
            this.advertiser = advertiser;
            this.message = message;
            this.callToAction = callToAction;
            this.isSuspicious = isSuspicious;
            this.explanation = explanation;
            this.signals = signals;
            this.advice = advice;
        }
    }
}
