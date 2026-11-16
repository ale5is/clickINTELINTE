using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class AdReferenceImporter
{
    private const string Destination = "Assets/ReferenceScreenshots/Platforms";

    private static readonly (string fileName, string url)[] Sources =
    {
        ("YouTube_InFeed_2023.png", "https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png"),
        ("YouTube_InStream_2025.jpg", "https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg"),
        ("YouTube_Shorts_Interactive_2025.png", "https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png"),
        ("GooglePlay_SearchSponsored_2025.jpg", "https://images.indianexpress.com/2025/09/Play-Store-Search-bar.jpeg"),
        ("GooglePlay_SuggestedDark_2026.png", "https://www.apptweak.com/img/2026/01/image8.png"),
        ("GooglePlay_SuggestedLight_2024.jpg", "https://www.digitaltrends.com/tachyon/2024/06/Screenshot_20240606_160856_Google-Play-Store.jpg?resize=720%2C720"),
        ("TikTok_TopView_2022.png", "https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png"),
        ("TikTok_Carousel_2025.png", "https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png")
    };

    [MenuItem("Click Inteligente/Importar referencias visuales")]
    private static async void ImportReferences()
    {
        Directory.CreateDirectory(Destination);
        int downloaded = 0;
        int failed = 0;

        try
        {
            for (int i = 0; i < Sources.Length; i++)
            {
                var source = Sources[i];
                EditorUtility.DisplayProgressBar("Click Inteligente", "Descargando " + source.fileName, (float)i / Sources.Length);

                using (UnityWebRequest request = UnityWebRequest.Get(source.url))
                {
                    request.timeout = 30;
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                    while (!operation.isDone)
                        await Task.Delay(100);

                    if (request.result != UnityWebRequest.Result.Success || request.downloadHandler == null || request.downloadHandler.data == null || request.downloadHandler.data.Length == 0)
                    {
                        Debug.LogWarning("No se pudo importar " + source.fileName + ": " + request.error);
                        failed++;
                        continue;
                    }

                    string path = Path.Combine(Destination, source.fileName);
                    File.WriteAllBytes(path, request.downloadHandler.data);
                    downloaded++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Referencias visuales", "Importadas: " + downloaded + "\nFallidas: " + failed + "\n\nCarpeta: " + Destination, "Aceptar");
    }
}
