# Referencias visuales y creatividades de anuncios

El juego utiliza dos capas de imágenes independientes:

- **Referencia de plataforma:** capturas reales de YouTube, Google Play y TikTok que conservan navegación, controles y estructura del formato.
- **Creatividad publicitaria:** una imagen completa descargada por separado y colocada dentro del espacio publicitario de la captura. El juego ya no escribe anunciante, título, descripción ni CTA encima de la captura.

## Creatividades publicitarias

| Tipo | Recurso | Contenido | Fuente |
|---|---|---|---|
| Falso documentado | `FakeVirus_2020` | Falsa alerta de infección que imita Windows Defender y presiona para descargar una solución | [PCRisk – fake virus alert](https://www.pcrisk.es/images/stories/screenshots202004/your-windows-10-is-infected-with-5-viruses-update-2020-04-07-another-variant.jpg) |
| Falso documentado | `FakePrize_iPhone14_2023` | Pop-up fraudulento que promete un iPhone 14 como premio | [PCRisk – iPhone winner scam](https://www.pcrisk.pl/images/stories/screenshots202303/apple-iphone-14-winner-scam-main.jpg) |
| Falso documentado | `FakeAPK_SuperMario_2017` | Página de descarga APK falsa que imita una descarga de juego | [SonicWall – fake Super Mario Run APK](https://images.contentstack.io/v3/assets/blt281ecbfc2563bf9b/blt816b52937e07916e/6643f697b444a97e41d83d91/2_mario_install.png) |
| Falso documentado | `FakeAccount_2023` | Aviso falso de suspensión de cuenta y solicitud de verificación | [Webflow – fake account suspension](https://uploads-ssl.webflow.com/5767b4eb3a9c9e1015d06770/650dab46144e5bb3e4a474ff_klient-screen.png) |
| Real / adaptación de encuadre | `Real_Loreal_DermaControl_2025` | Campaña oficial L'Oréal Men Expert Derma Control | [L'Oréal Paris](https://www.loreal-paris.es/-/media/project/loreal/brand-sites/oap/emea/es/products/men/cuidado-facial/derma-control/reno2025/banner-derma-control.jpg) |
| Real / adaptación de encuadre | `Real_KhanAcademy_2026` | Anuncio activo de Khan Academy para preparación de exámenes | [Ad Swipe File – Khan Academy](https://cdn.adswipefile.com/tracking/159403248441/961728760044823.jpg) |
| Real / adaptación de encuadre | `Real_Argentina_Freedom_2025` | Creatividad de la campaña turística Argentina. Freedom Lives Here | [Continental – campaña Argentina](https://www.continental.com.ar/adjuntos/1200/continental/2025/10/11-freedom.jpg) |

Las creatividades falsas son ejemplos documentados por fuentes de seguridad o investigación, no anuncios generados por el juego. Las campañas de marca se conservan como piezas reales; Unity solo adapta su tamaño y encuadre al espacio del formato. La asignación del escenario a la creatividad se realiza en `GetScenarioCreativeName`.

## Correspondencia de escenarios

- Alertas de virus y actualización urgente: `FakeVirus_2020`.
- Premios inesperados: `FakePrize_iPhone14_2023`.
- Descargas APK externas: `FakeAPK_SuperMario_2017`.
- Bloqueos y verificaciones de cuenta: `FakeAccount_2023`.
- Rutina facial: campaña real L'Oréal Men Expert.
- Aprendizaje y preparación de exámenes: campaña real Khan Academy.
- Turismo: campaña real Visit Argentina.

## Referencias de plataforma

| Plataforma | Variante | Recurso local | Fuente |
|---|---|---|---|
| YouTube | In-feed | `YouTube_InFeed_2023` | [Strike Social](https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png) |
| YouTube | In-stream | `YouTube_InStream_2025` | [ComputerHoy](https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg) |
| YouTube | Shorts | `YouTube_Shorts_Interactive_2025` | [Squarespace CDN](https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png) |
| Google Play | Búsqueda patrocinada | `GooglePlay_SearchSponsored_2026` | [Vodafone device guide](https://cdn.sp-selfservice.com/ml/7200426-f-3dcd3bb1-7fa4-4714-97d0-3dbee21eaf98.png?height=904&width=874) |
| Google Play | Sugerencias oscuras | `GooglePlay_SuggestedDark_2026` | [AppTweak](https://www.apptweak.com/img/2026/01/image8.png) |
| Google Play | Sugerencias claras | `GooglePlay_SuggestedLight_2026` | [AppTweak](https://www.apptweak.com/img/2026/01/image8.png) |
| TikTok | In-feed | `TikTok_InFeed_Screen_2025` | [Ecwid support article](https://support.ecwid.com/hc/article_attachments/15645628291996) |
| TikTok | TopView | `TikTok_TopView_2022` | [MegaDigital](https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png) |
| TikTok | Carrusel | `TikTok_Carousel_2025` | [Webflow CDN](https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png) |

## Integración en Unity

- `BuildPlatformMockup` selecciona una referencia real de plataforma.
- `CreateReferenceImage` carga la captura desde `Resources/AdReferences`.
- `CreateScenarioAdCreative` carga una creatividad independiente desde `Resources/AdCreatives` y la coloca en la zona publicitaria correspondiente a cada formato.
- `GetScenarioCreativeName` vincula el caso educativo con la pieza real o falsa documentada.
- No se dibujan titulares ni botones publicitarios encima de la captura. La creatividad completa es una imagen independiente.
- Si una creatividad no se descarga, se registra una advertencia y la captura de plataforma permanece visible sin sustituirla por una pieza inventada.

## Importación

En Unity 2022.3.62f3, ejecutar **Click Inteligente > Importar anuncios y referencias visuales**. El importador valida PNG/JPEG y descarga los recursos en:

- `Assets/Resources/AdReferences`
- `Assets/Resources/AdCreatives`

El workflow de GitHub Actions ejecuta `Scripts/Import-AdReferences.ps1` antes de compilar Windows. El juego no necesita conectividad de red en tiempo de ejecución.

## Licencias y uso educativo

Las capturas y creatividades pertenecen a sus respectivos autores, marcas o sitios de investigación. La disponibilidad pública de una imagen no implica autorización de redistribución. Antes de publicar o distribuir el ejecutable, revisar licencias, permisos y condiciones de cada fuente; reemplazar cualquier recurso cuyo uso no esté autorizado. Los ejemplos de estafa se presentan exclusivamente para educación y reconocimiento de señales de riesgo.
