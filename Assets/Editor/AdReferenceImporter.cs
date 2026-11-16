using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class AdReferenceImporter
{
    private const string Destination = "Assets/Resources/AdReferences";

    private static readonly (string fileName, string url)[] Sources =
    {
        ("YouTube_InFeed_2023", "https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png"),
        ("YouTube_InStream_2025", "https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg"),
        ("YouTube_Shorts_Interactive_2025", "https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png"),
        ("GooglePlay_SearchSponsored_2026", "https://cdn.sp-selfservice.com/ml/7200426-f-3dcd3bb1-7fa4-4714-97d0-3dbee21eaf98.png?height=904&width=874"),
        ("GooglePlay_SuggestedDark_2026", "https://www.apptweak.com/img/2026/01/image8.png"),
        ("GooglePlay_SuggestedLight_2026", "https://www.apptweak.com/img/2026/01/image8.png"),
        ("TikTok_InFeed_Screen_2025", "https://support.ecwid.com/hc/article_attachments/15645628291996"),
        ("TikTok_TopView_2022", "https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png"),
        ("TikTok_Carousel_2025", "https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png")
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

                    byte[] data = request.downloadHandler == null ? null : request.downloadHandler.data;
                    if (request.result != UnityWebRequest.Result.Success || !IsSupportedImage(data))
                    {
                        Debug.LogWarning("No se importó " + source.fileName + ": respuesta no válida o imagen PNG/JPEG no reconocida. " + request.error);
                        failed++;
                        continue;
                    }

                    string extension = IsPng(data) ? ".png" : ".jpg";
                    string path = Path.Combine(Destination, source.fileName + extension);
                    File.WriteAllBytes(path, data);
                    downloaded++;
                }
            }
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Error al importar referencias", exception.Message, "Aceptar");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Referencias visuales", "Importadas: " + downloaded + "\nFallidas: " + failed + "\n\nCarpeta: " + Destination, "Aceptar");
    }

    private static bool IsSupportedImage(byte[] data)
    {
        return IsPng(data) || IsJpeg(data);
    }

    private static bool IsPng(byte[] data)
    {
        return data != null && data.Length >= 8 &&
               data[0] == 0x89 && data[1] == 0x50 &&
               data[2] == 0x4E && data[3] == 0x47 &&
               data[4] == 0x0D && data[5] == 0x0A &&
               data[6] == 0x1A && data[7] == 0x0A;
    }

    private static bool IsJpeg(byte[] data)
    {
        return data != null && data.Length >= 3 &&
               data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF;
    }
}
