using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class AdReferenceImporter
{
    private const string ReferenceDestination = "Assets/Resources/AdReferences";
    private const string CreativeDestination = "Assets/Resources/AdCreatives";

    private static readonly (string fileName, string url)[] ReferenceSources =
    {
        ("YouTube_InFeed_2023", "https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png"),
        ("YouTube_InStream_2025", "https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg"),
        ("YouTube_Shorts_Interactive_2025", "https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png?format=1000w"),
        ("GooglePlay_SearchSponsored_2026", "https://cdn.sp-selfservice.com/ml/7200426-f-3dcd3bb1-7fa4-4714-97d0-3dbee21eaf98.png?height=904&width=874"),
        ("GooglePlay_SuggestedDark_2026", "https://www.apptweak.com/img/2026/01/image8.png"),
        ("GooglePlay_SuggestedLight_2026", "https://www.apptweak.com/img/2026/01/image8.png"),
        ("TikTok_InFeed_Screen_2025", "https://support.ecwid.com/hc/article_attachments/15645628291996"),
        ("TikTok_TopView_2022", "https://megadigital.ai/wp-content/uploads/2022/08/quang-quang-cao-top-view-tiktok-la-gi.png"),
        ("TikTok_Carousel_2025", "https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png")
    };

    private static readonly (string fileName, string url)[] CreativeSources =
    {
        ("FakeVirus_2020", "https://www.pcrisk.es/images/stories/screenshots202004/your-windows-10-is-infected-with-5-viruses-update-2020-04-07-another-variant.jpg"),
        ("FakePrize_iPhone14_2023", "https://www.pcrisk.pl/images/stories/screenshots202303/apple-iphone-14-winner-scam-main.jpg"),
        ("FakeAPK_SuperMario_2017", "https://images.contentstack.io/v3/assets/blt281ecbfc2563bf9b/blt816b52937e07916e/6643f697b444a97e41d83d91/2_mario_install.png"),
        ("FakeAccount_2023", "https://uploads-ssl.webflow.com/5767b4eb3a9c9e1015d06770/650dab46144e5bb3e4a474ff_klient-screen.png"),
        ("Real_Loreal_DermaControl_2025", "https://www.loreal-paris.es/-/media/project/loreal/brand-sites/oap/emea/es/products/men/cuidado-facial/derma-control/reno2025/banner-derma-control.jpg"),
        ("Real_KhanAcademy_2026", "https://cdn.adswipefile.com/tracking/159403248441/961728760044823.jpg"),
        ("Real_Argentina_Freedom_2025", "https://www.continental.com.ar/adjuntos/1200/continental/2025/10/11-freedom.jpg")
    };

    [MenuItem("Click Inteligente/Importar anuncios y referencias visuales")]
    private static async void ImportReferences()
    {
        int referenceDownloaded = 0;
        int creativeDownloaded = 0;

        try
        {
            referenceDownloaded = await ImportSources(ReferenceSources, ReferenceDestination, "Referencias de plataformas");
            creativeDownloaded = await ImportSources(CreativeSources, CreativeDestination, "Creatividades publicitarias");
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Error al importar imágenes", exception.Message, "Aceptar");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog(
            "Importación visual",
            "Referencias de plataformas: " + referenceDownloaded + " / " + ReferenceSources.Length +
            "\nCreatividades de anuncios: " + creativeDownloaded + " / " + CreativeSources.Length +
            "\n\nCarpetas: " + ReferenceDestination + " y " + CreativeDestination,
            "Aceptar");
    }

    private static async Task<int> ImportSources((string fileName, string url)[] sources, string destination, string label)
    {
        Directory.CreateDirectory(destination);
        int downloaded = 0;

        for (int i = 0; i < sources.Length; i++)
        {
            var source = sources[i];
            EditorUtility.DisplayProgressBar(label, "Descargando " + source.fileName, (float)i / sources.Length);

            using (UnityWebRequest request = UnityWebRequest.Get(source.url))
            {
                request.timeout = 30;
                request.SetRequestHeader("Accept", "image/png,image/jpeg;q=0.9,*/*;q=0.1");
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Delay(100);

                byte[] data = request.downloadHandler == null ? null : request.downloadHandler.data;
                if (request.result != UnityWebRequest.Result.Success || !IsSupportedImage(data))
                {
                    string contentType = request.GetResponseHeader("Content-Type") ?? "(sin Content-Type)";
                    int byteCount = data == null ? 0 : data.Length;
                    string signature = data == null || data.Length < 4
                        ? "(sin firma)"
                        : data[0].ToString("X2") + " " + data[1].ToString("X2") + " " + data[2].ToString("X2") + " " + data[3].ToString("X2");
                    Debug.LogWarning("No se importó " + source.fileName +
                        ": HTTP " + request.responseCode +
                        ", Content-Type " + contentType +
                        ", bytes " + byteCount +
                        ", firma " + signature +
                        ", URL final " + request.url +
                        ", error " + request.error);
                    continue;
                }

                string extension = IsPng(data) ? ".png" : ".jpg";
                string path = Path.Combine(destination, source.fileName + extension);
                File.WriteAllBytes(path, data);
                downloaded++;
            }
        }

        return downloaded;
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