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
        int placement = placementIndex % 3;

        if (platform == 0)
        {
            string youtubeReference = placement == 0 ? "YouTube_InFeed_2023" : placement == 1 ? "YouTube_InStream_2025" : "YouTube_Shorts_Interactive_2025";
            BuildReferencePlatformScreen(scenario, youtubeReference, "YOUTUBE");
            return;
        }

        if (platform == 1)
        {
            string playStoreReference = placement == 0 ? "GooglePlay_SearchSponsored_2026" : placement == 1 ? "GooglePlay_SuggestedDark_2026" : "GooglePlay_SuggestedLight_2026";
            BuildReferencePlatformScreen(scenario, playStoreReference, "GOOGLE PLAY");
            return;
        }

        string tikTokReference = placement == 0
            ? "TikTok_InFeed_Screen_2025"
            : placement == 1
                ? "TikTok_Carousel_2025"
                : "TikTok_TopView_2022";
        BuildReferencePlatformScreen(scenario, tikTokReference, "TIKTOK");
    }

    private void BuildReferencePlatformScreen(AdScenario scenario, string referenceName, string platformName)
    {
        RectTransform referenceRect;
        if (!CreateReferenceImage("RealPlatformReference", new Vector2(0.07f, 0.215f), new Vector2(0.93f, 0.90f), referenceName, out referenceRect))
        {
            CreateCard(screenRoot, "MissingPlatformReference", new Vector2(0.07f, 0.31f), new Vector2(0.93f, 0.82f), Hex("#243A50"));
            AddText(screenRoot, "MissingPlatformReferenceText", "FALTA IMPORTAR LA REFERENCIA REAL DE " + platformName + ".\\nEn Unity: Click Inteligente > Importar referencias visuales.", new Vector2(0.12f, 0.43f), new Vector2(0.88f, 0.68f), Vector2.zero, Vector2.zero, 22, Color.white, TextAlignmentOptions.Center);
            return;
        }

        CreateScenarioAdOverlay(referenceRect, scenario, referenceName);
    }

    private void CreateScenarioAdOverlay(RectTransform imageRoot, AdScenario scenario, string referenceName)
    {
        Vector2 panelMin = new Vector2(0.08f, 0.08f);
        Vector2 panelMax = new Vector2(0.92f, 0.30f);
        float titleSize = 17f;
        float messageSize = 12f;

        if (referenceName == "YouTube_InFeed_2023")
        {
            panelMin = new Vector2(0.02f, 0.54f);
            panelMax = new Vector2(0.82f, 0.91f);
        }
        else if (referenceName == "YouTube_InStream_2025")
        {
            panelMin = new Vector2(0.02f, 0.05f);
            panelMax = new Vector2(0.48f, 0.38f);
            titleSize = 13f;
            messageSize = 9f;
        }
        else if (referenceName == "YouTube_Shorts_Interactive_2025")
        {
            panelMin = new Vector2(0.30f, 0.10f);
            panelMax = new Vector2(0.70f, 0.38f);
            titleSize = 13f;
            messageSize = 9f;
        }
        else if (referenceName == "GooglePlay_SearchSponsored_2026")
        {
            panelMin = new Vector2(0.02f, 0.38f);
            panelMax = new Vector2(0.49f, 0.70f);
            titleSize = 13f;
            messageSize = 9f;
        }
        else if (referenceName == "GooglePlay_SuggestedLight_2026" || referenceName == "GooglePlay_SuggestedDark_2026")
        {
            panelMin = new Vector2(0.05f, 0.18f);
            panelMax = new Vector2(0.95f, 0.50f);
            titleSize = 13f;
            messageSize = 9f;
        }
        else if (referenceName == "TikTok_InFeed_Screen_2025")
        {
            panelMin = new Vector2(0.27f, 0.14f);
            panelMax = new Vector2(0.72f, 0.40f);
            titleSize = 12f;
            messageSize = 9f;
        }
        else if (referenceName == "TikTok_Carousel_2025")
        {
            panelMin = new Vector2(0.12f, 0.12f);
            panelMax = new Vector2(0.88f, 0.40f);
            titleSize = 13f;
            messageSize = 9f;
        }
        else if (referenceName == "TikTok_TopView_2022")
        {
            panelMin = new Vector2(0.04f, 0.28f);
            panelMax = new Vector2(0.34f, 0.62f);
            titleSize = 12f;
            messageSize = 9f;
        }

        CreateCard(imageRoot, "ScenarioAdContent", panelMin, panelMax, new Color(0.035f, 0.055f, 0.085f, 0.94f));
        float panelHeight = panelMax.y - panelMin.y;
        AddText(imageRoot, "ScenarioAdAdvertiser", scenario.advertiser + " · ANUNCIO", new Vector2(panelMin.x + 0.025f, panelMax.y - panelHeight * 0.25f), new Vector2(panelMax.x - 0.025f, panelMax.y - panelHeight * 0.05f), Vector2.zero, Vector2.zero, Mathf.Max(9f, titleSize - 4f), Hex("#BCEBFF"), TextAlignmentOptions.Left);
        AddText(imageRoot, "ScenarioAdTitle", scenario.title, new Vector2(panelMin.x + 0.025f, panelMin.y + panelHeight * 0.48f), new Vector2(panelMax.x - 0.025f, panelMax.y - panelHeight * 0.29f), Vector2.zero, Vector2.zero, titleSize, Color.white, TextAlignmentOptions.Left);
        AddText(imageRoot, "ScenarioAdMessage", scenario.message, new Vector2(panelMin.x + 0.025f, panelMin.y + panelHeight * 0.18f), new Vector2(panelMax.x - 0.025f, panelMin.y + panelHeight * 0.46f), Vector2.zero, Vector2.zero, messageSize, Hex("#E1E8F0"), TextAlignmentOptions.Left);
        AddText(imageRoot, "ScenarioAdCallToAction", scenario.callToAction, new Vector2(panelMin.x + 0.025f, panelMin.y + panelHeight * 0.02f), new Vector2(panelMax.x - 0.025f, panelMin.y + panelHeight * 0.17f), Vector2.zero, Vector2.zero, Mathf.Max(9f, titleSize - 3f), Hex("#8FD4FF"), TextAlignmentOptions.Right);
    }

    private bool CreateReferenceImage(string objectName, Vector2 anchorMin, Vector2 anchorMax, string resourceName, out RectTransform rect)
    {
        rect = null;
        Texture2D texture = Resources.Load<Texture2D>("AdReferences/" + resourceName);
        if (texture == null)
            return false;

        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        rect = imageObject.GetComponent<RectTransform>();
        rect.SetParent(screenRoot, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = texture;
        image.raycastTarget = false;

        Rect crop = new Rect(0f, 0f, 1f, 1f);
        if (resourceName == "GooglePlay_SuggestedLight_2026")
            crop = new Rect(0f, 0f, 0.5f, 1f);
        else if (resourceName == "GooglePlay_SuggestedDark_2026")
            crop = new Rect(0.5f, 0f, 0.5f, 1f);
        image.uvRect = crop;

        AspectRatioFitter fitter = imageObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = (float)texture.width * crop.width / (texture.height * crop.height);
        return true;
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
