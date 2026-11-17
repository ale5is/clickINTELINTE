using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ClickInteligenteSceneGenerator
{
    private const string SceneFolder = "Assets/Scenes";
    private static readonly Color Background = new Color32(16, 24, 39, 255);
    private static readonly Color Panel = new Color32(27, 42, 61, 255);
    private static readonly Color Cyan = new Color32(112, 214, 255, 255);
    private static readonly Color Teal = new Color32(20, 125, 146, 255);
    private static readonly Color Green = new Color32(35, 122, 88, 255);
    private static readonly Color Red = new Color32(167, 70, 70, 255);
    private static readonly Color White = Color.white;
    private static readonly Color Muted = new Color32(214, 226, 240, 255);

    static ClickInteligenteSceneGenerator()
    {
        EditorApplication.delayCall += GenerateIfNeeded;
    }

    [MenuItem("Click Inteligente/Generar escenas estáticas")]
    public static void GenerateManually()
    {
        GenerateAll();
        Debug.Log("Click Inteligente: escenas estáticas generadas y agregadas a Build Settings.");
    }

    private static void GenerateIfNeeded()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += GenerateIfNeeded;
            return;
        }
        string[] required = { "Menu", "Tutorial01", "Tutorial02", "Tutorial03", "Anuncio01", "Anuncio02", "Anuncio03", "Anuncio04", "Anuncio05", "Anuncio06", "Anuncio07", "Anuncio08", "Anuncio09", "Explicacion01", "Explicacion02", "Explicacion03", "Explicacion04", "Explicacion05", "Explicacion06", "Explicacion07", "Explicacion08", "Explicacion09", "Resultados" };
        foreach (string sceneName in required)
        {
            if (!System.IO.File.Exists(SceneFolder + "/" + sceneName + ".unity"))
            {
                GenerateAll();
                return;
            }
        }
    }

    private static void GenerateAll()
    {
        if (!AssetDatabase.IsValidFolder(SceneFolder))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        CreateMenu();
        for (int i = 0; i < 3; i++)
            CreateTutorial(i);
        for (int i = 0; i < ClickInteligenteSceneFlow.Questions.Length; i++)
        {
            CreateQuestion(i);
            CreateFeedback(i);
        }
        CreateResults();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateBuildSettings();
    }

    private static void CreateMenu()
    {
        Scene scene = NewScene("Menu");
        ClickInteligenteSceneFlow flow = AddBase(scene, ClickInteligenteSceneFlow.ScreenType.Menu, 0);
        AddText(scene, "Brand", "CLICK INTELIGENTE", 0.91f, 0.97f, 30, Cyan, TextAlignmentOptions.Left);
        AddText(scene, "MainTitle", "Antes de hacer clic, pensá", 0.68f, 0.84f, 54, White, TextAlignmentOptions.Center);
        AddPanel(scene, "IntroPanel", 0.31f, 0.63f);
        AddText(scene, "Intro", "Aprendé a reconocer anuncios engañosos que intentan que descargues aplicaciones o archivos peligrosos. Vas a practicar con campañas reales y ejemplos fraudulentos documentados.", 0.38f, 0.58f, 25, Muted, TextAlignmentOptions.Center);
        AddText(scene, "Audience", "Actividad educativa para chicos y chicas de 10 a 16 años", 0.25f, 0.32f, 20, Muted, TextAlignmentOptions.Center);
        AddButton(scene, "StartButton", "COMENZAR", 0.12f, 0.23f, Teal);
        AddText(scene, "Safety", "Los botones son simulaciones educativas: no abren sitios ni descargan archivos.", 0.035f, 0.085f, 16, Muted, TextAlignmentOptions.Center);
        Save(scene, "Menu");
    }

    private static void CreateTutorial(int index)
    {
        string sceneName = "Tutorial0" + (index + 1);
        Scene scene = NewScene(sceneName);
        AddBase(scene, ClickInteligenteSceneFlow.ScreenType.Tutorial, index + 1);
        string[] titles = { "1. Frená y observá", "2. Buscá señales de alerta", "3. Elegí cómo protegerte" };
        string[] descriptions =
        {
            "Antes de tocar un anuncio, leé el mensaje completo. No actúes por apuro y no descargues archivos porque una publicidad te lo pida.",
            "Desconfiá de los premios inesperados, las ofertas increíbles, las amenazas de virus, las cuentas regresivas y las solicitudes de instalar aplicaciones desconocidas.",
            "No compartas contraseñas ni datos personales desde un anuncio. Verificá al anunciante por canales oficiales y buscá aplicaciones desde tiendas confiables."
        };
        string[][] tips =
        {
            new[] { "No hagas clic por impulso", "No descargues desde anuncios", "Verificá quién publica" },
            new[] { "Urgencia o amenazas", "Premios demasiado buenos", "Archivos o apps desconocidas" },
            new[] { "No compartas contraseñas", "Usá canales oficiales", "Pedí ayuda si tenés dudas" }
        };
        AddHeader(scene, "TUTORIAL", titles[index], "Leé cada consejo. Después vas a ponerlo en práctica.");
        AddPanel(scene, "LessonPanel", 0.31f, 0.73f);
        AddText(scene, "LessonText", descriptions[index], 0.47f, 0.68f, 28, White, TextAlignmentOptions.Center);
        for (int i = 0; i < 3; i++)
        {
            float x = 0.10f + i * 0.27f;
            AddPanel(scene, "TipPanel" + i, 0.28f, 0.43f, x, x + 0.25f);
            AddText(scene, "TipText" + i, tips[index][i], 0.29f, 0.42f, 18, Cyan, TextAlignmentOptions.Center, x, x + 0.25f);
        }
        if (index == 2)
        {
            AddPanel(scene, "VideoPlaceholderPanel", 0.11f, 0.24f, 0.25f, 0.75f);
            AddText(scene, "VideoPlaceholder", "ESPACIO PARA VIDEO EDUCATIVO", 0.14f, 0.21f, 18, Muted, TextAlignmentOptions.Center, 0.27f, 0.73f);
        }
        AddButton(scene, "ContinueButton", index == 2 ? "COMENZAR PRÁCTICA" : "CONTINUAR", 0.055f, 0.15f, Teal, 0.68f, 0.92f);
        if (index > 0)
            AddButton(scene, "PreviousButton", "VOLVER", 0.055f, 0.15f, new Color32(52, 70, 93, 255), 0.08f, 0.30f);
        Save(scene, sceneName);
    }

    private static void CreateQuestion(int index)
    {
        string sceneName = "Anuncio" + (index + 1).ToString("00");
        Scene scene = NewScene(sceneName);
        AddBase(scene, ClickInteligenteSceneFlow.ScreenType.Question, index);
        ClickInteligenteSceneFlow.QuestionData data = ClickInteligenteSceneFlow.Questions[index];
        AddText(scene, "Brand", "CLICK INTELIGENTE  ·  APRENDÉ A NAVEGAR SEGURO", 0.93f, 0.985f, 20, Cyan, TextAlignmentOptions.Left);
        AddText(scene, "Progress", "ANUNCIO " + (index + 1) + " / " + ClickInteligenteSceneFlow.Questions.Length, 0.87f, 0.925f, 18, White, TextAlignmentOptions.Right, 0.75f, 0.97f);
        AddPanel(scene, "AdCard", 0.25f, 0.82f, 0.14f, 0.86f);
        AddText(scene, "PlatformLabel", "PUBLICIDAD  ·  EJEMPLO EDUCATIVO", 0.76f, 0.81f, 15, Cyan, TextAlignmentOptions.Left, 0.18f, 0.82f);
        AddText(scene, "QuestionTitle", data.title, 0.65f, 0.75f, 31, White, TextAlignmentOptions.Center, 0.18f, 0.82f);
        AddText(scene, "Advertiser", data.advertiser, 0.56f, 0.64f, 22, Cyan, TextAlignmentOptions.Center, 0.18f, 0.82f);
        AddPanel(scene, "AdCreativePanel", 0.35f, 0.54f, 0.20f, 0.80f);
        AddText(scene, "AdMessage", data.message, 0.37f, 0.52f, 21, White, TextAlignmentOptions.Center, 0.23f, 0.77f);
        AddButton(scene, "FakeAdButton", data.buttonLabel, 0.28f, 0.35f, new Color32(61, 95, 130, 255), 0.33f, 0.67f);
        AddText(scene, "QuestionPrompt", "¿Este anuncio parece seguro o intenta engañarte?", 0.205f, 0.255f, 20, Muted, TextAlignmentOptions.Center);
        AddButton(scene, "SafeButton", "PARECE SEGURO", 0.09f, 0.18f, Green, 0.10f, 0.47f);
        AddButton(scene, "ReportButton", "REPORTAR COMO SOSPECHOSO", 0.09f, 0.18f, Red, 0.53f, 0.90f);
        Save(scene, sceneName);
    }

    private static void CreateFeedback(int index)
    {
        string sceneName = "Explicacion" + (index + 1).ToString("00");
        Scene scene = NewScene(sceneName);
        AddBase(scene, ClickInteligenteSceneFlow.ScreenType.Feedback, index);
        AddHeader(scene, "RETROALIMENTACIÓN", "Repasemos esta decisión", "Aprendé por qué el anuncio es seguro o sospechoso.");
        AddPanel(scene, "FeedbackPanel", 0.24f, 0.75f, 0.09f, 0.91f);
        AddText(scene, "FeedbackTitle", "¡Muy bien!", 0.66f, 0.73f, 32, Cyan, TextAlignmentOptions.Center);
        AddText(scene, "FeedbackStatus", "Tu clasificación fue correcta.", 0.59f, 0.65f, 21, White, TextAlignmentOptions.Center);
        AddText(scene, "Explanation", "", 0.48f, 0.58f, 21, White, TextAlignmentOptions.Center, 0.14f, 0.86f);
        AddText(scene, "Signals", "", 0.36f, 0.47f, 18, Cyan, TextAlignmentOptions.Center, 0.14f, 0.86f);
        AddText(scene, "Advice", "", 0.27f, 0.37f, 18, Muted, TextAlignmentOptions.Center, 0.14f, 0.86f);
        AddText(scene, "Score", "", 0.19f, 0.24f, 17, Muted, TextAlignmentOptions.Center);
        AddButton(scene, "NextButton", index == ClickInteligenteSceneFlow.Questions.Length - 1 ? "VER RESULTADOS" : "SIGUIENTE ANUNCIO", 0.055f, 0.14f, Teal, 0.63f, 0.92f);
        Save(scene, sceneName);
    }

    private static void CreateResults()
    {
        Scene scene = NewScene("Resultados");
        AddBase(scene, ClickInteligenteSceneFlow.ScreenType.Results, 0);
        AddHeader(scene, "RESULTADOS", "Terminaste la práctica", "Este resultado corresponde a esta partida.");
        AddPanel(scene, "ResultPanel", 0.28f, 0.72f, 0.18f, 0.82f);
        AddText(scene, "FinalScore", "0%", 0.48f, 0.67f, 66, Cyan, TextAlignmentOptions.Center);
        AddText(scene, "FinalDetails", "", 0.38f, 0.48f, 24, White, TextAlignmentOptions.Center);
        AddText(scene, "FinalMessage", "", 0.30f, 0.39f, 20, Muted, TextAlignmentOptions.Center);
        AddButton(scene, "RestartButton", "VOLVER AL MENÚ", 0.10f, 0.20f, Teal);
        Save(scene, "Resultados");
    }

    private static Scene NewScene(string name)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0, 0, -10);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Background;
        GameObject eventObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return scene;
    }

    private static ClickInteligenteSceneFlow AddBase(Scene scene, ClickInteligenteSceneFlow.ScreenType type, int index)
    {
        GameObject root = new GameObject("SceneFlow");
        SceneManager.MoveGameObjectToScene(root, scene);
        ClickInteligenteSceneFlow flow = root.AddComponent<ClickInteligenteSceneFlow>();
        flow.screenType = type;
        flow.itemIndex = index;
        return flow;
    }

    private static void AddHeader(Scene scene, string eyebrow, string title, string subtitle)
    {
        AddText(scene, "Eyebrow", eyebrow, 0.90f, 0.97f, 20, Cyan, TextAlignmentOptions.Left);
        AddText(scene, "HeaderTitle", title, 0.79f, 0.89f, 38, White, TextAlignmentOptions.Center);
        AddText(scene, "HeaderSubtitle", subtitle, 0.73f, 0.79f, 17, Muted, TextAlignmentOptions.Center);
    }

    private static void AddPanel(Scene scene, string name, float bottom, float top, float left = 0.08f, float right = 0.92f)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = Panel;
    }

    private static void AddText(Scene scene, string name, string value, float bottom, float top, float fontSize, Color color, TextAlignmentOptions alignment, float left = 0.08f, float right = 0.92f)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = new Vector2(12, 4);
        rect.offsetMax = new Vector2(-12, -4);
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14;
        text.fontSizeMax = fontSize;
        text.enableWordWrapping = true;
        text.raycastTarget = false;
    }

    private static void AddButton(Scene scene, string name, string label, float bottom, float top, Color color, float left = 0.27f, float right = 0.73f)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = new Vector2(4, 4);
        rect.offsetMax = new Vector2(-4, -4);
        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        AddTextToButton(buttonObject.transform, label);
    }

    private static void AddTextToButton(Transform parent, string label)
    {
        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(10, 4);
        rect.offsetMax = new Vector2(-10, -4);
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = label;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = 22;
        text.fontStyle = FontStyles.Bold;
        text.color = White;
        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14;
        text.fontSizeMax = 22;
        text.raycastTarget = false;
    }

    private static void Save(Scene scene, string sceneName)
    {
        string path = SceneFolder + "/" + sceneName + ".unity";
        EditorSceneManager.SaveScene(scene, path);
    }

    private static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        string[] names =
        {
            "Menu", "Tutorial01", "Tutorial02", "Tutorial03",
            "Anuncio01", "Explicacion01", "Anuncio02", "Explicacion02",
            "Anuncio03", "Explicacion03", "Anuncio04", "Explicacion04",
            "Anuncio05", "Explicacion05", "Anuncio06", "Explicacion06",
            "Anuncio07", "Explicacion07", "Anuncio08", "Explicacion08",
            "Anuncio09", "Explicacion09", "Resultados"
        };
        foreach (string name in names)
        {
            string path = SceneFolder + "/" + name + ".unity";
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (!string.IsNullOrEmpty(guid))
                scenes.Add(new EditorBuildSettingsScene(path, true));
        }
        string[] legacy = { "Inicio", "Iniciofake", "VideoV1", "VideoV2Fake", "VideoV2Real", "Final" };
        foreach (string name in legacy)
        {
            string path = SceneFolder + "/" + name + ".unity";
            if (System.IO.File.Exists(path))
                scenes.Add(new EditorBuildSettingsScene(path, true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
