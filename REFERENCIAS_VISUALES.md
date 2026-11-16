# Referencias visuales para Click Inteligente

Las pantallas de práctica cargan imágenes reales de referencia desde `Assets/Resources/AdReferences`. No se generan creatividades publicitarias con primitivas geométricas ni se dibujan logos aproximados para reemplazar las marcas.

## Recursos conectados

| Plataforma | Variante | Recurso local | Fuente |
|---|---|---|---|
| YouTube | In-feed | `YouTube_InFeed_2023` | [Strike Social](https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png) |
| YouTube | In-stream | `YouTube_InStream_2025` | [ComputerHoy](https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg) |
| YouTube | Shorts | `YouTube_Shorts_Interactive_2025` | [Squarespace CDN](https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png) |
| Google Play | Búsqueda patrocinada | `GooglePlay_SearchSponsored_2025` | [Indian Express](https://images.indianexpress.com/2025/09/Play-Store-Search-bar.jpeg) |
| Google Play | Sugerencias oscuras | `GooglePlay_SuggestedDark_2026` | [AppTweak](https://www.apptweak.com/img/2026/01/image8.png) |
| Google Play | Sugerencias claras | `GooglePlay_SuggestedLight_2024` | [Digital Trends](https://www.digitaltrends.com/tachyon/2024/06/Screenshot_20240606_160856_Google-Play-Store.jpg?resize=720%2C720) |
| TikTok | In-feed | `TikTok_InFeed_Creative_2026` | [TikTok for Business](https://lf-tt4b.tiktokcdn.com/obj/i18nblog/tt4b_cms/en-US/tipdilz7wysq-52DY2FWG6YW1lkjzUCrIi7.jpg) |
| TikTok | TopView | `TikTok_TopView_2022` | [MegalDigital](https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png) |
| TikTok | Carrusel | `TikTok_Carousel_2025` | [Webflow CDN](https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png) |

Los casos, títulos y mensajes que aparecen en el ejercicio siguen siendo ficticios y están marcados como tales. Las imágenes sirven como referencias visuales reales; no se afirma que las marcas o anunciantes de las capturas originales sean los responsables de los casos ficticios.

## Correspondencia con el código

- `BuildPlatformMockup` selecciona capturas reales para las tres variantes de YouTube, las tres de Google Play y las variantes TikTok Carrusel/TopView.
- `BuildTikTokAdPage` conserva la estructura vertical del feed In-Feed y `CreateAdArtwork` conecta allí la creatividad fotográfica real.
- `CreateReferenceImage` carga los archivos desde `Resources/AdReferences` y ajusta la relación de aspecto.
- Si una imagen falta, el juego muestra un aviso de recurso no importado. No sustituye la imagen por una ilustración inventada.

## Importación

En Unity 2022.3.62f3, ejecutar **Click Inteligente > Importar referencias visuales**. El importador del Editor descarga y valida PNG/JPEG en `Assets/Resources/AdReferences`. El workflow de GitHub Actions ejecuta también `Scripts/Import-AdReferences.ps1` antes del build de Windows.

El juego no realiza solicitudes de red en tiempo de ejecución. Las imágenes deben estar presentes localmente antes de compilar para que aparezcan en el ejecutable.

## Exactitud y permisos

La documentación oficial confirma que los anuncios In-feed de YouTube incluyen miniatura, título y texto, y que los Shorts aparecen integrados en el feed vertical. TikTok describe In-Feed como un anuncio nativo del feed Para ti y el carrusel como varias imágenes que se recorren deslizando. Google Play permite que las fichas utilicen iconos, capturas y videos de vista previa.

- [YouTube: formatos de anuncios](https://support.google.com/google-ads/answer/2375464)
- [YouTube: anuncios In-feed](https://support.google.com/google-ads/answer/6227733)
- [TikTok for Business: guía de anuncios de video](https://ads.tiktok.com/business/en/guides/video-advertising-guide)
- [TikTok: anuncios carrusel](https://ads.tiktok.com/help/article/carousel-ads?lang=en)
- [Google Play: recursos de vista previa](https://support.google.com/googleplay/android-developer/answer/9866151)

La integración técnica no otorga derechos de redistribución. Antes de publicar o distribuir el juego, verificar las licencias y permisos de cada imagen de terceros; reemplazar cualquier recurso que no pueda reutilizarse legalmente. No presentar la descarga de una imagen pública como autorización para redistribuirla.
