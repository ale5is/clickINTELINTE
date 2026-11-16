# Click Inteligente

Proyecto educativo de Unity orientado a enseñar a niños y adolescentes de 10 a 16 años a reconocer anuncios falsos que intentan inducir la descarga de aplicaciones o archivos maliciosos.

## Versión del proyecto

- Unity: 2022.3.62f3
- Escena de inicio: `Assets/Scenes/Menu.unity`
- Controlador principal: `Assets/scripts/ClickInteligenteController.cs`

## Recorrido educativo

1. Menú de inicio.
2. Tutorial dividido en tres partes: detenerse antes de hacer clic, reconocer señales de alerta y elegir cómo protegerse.
3. Práctica con nueve anuncios ficticios, mezclados aleatoriamente.
4. Retroalimentación inmediata después de cada clasificación.
5. Resultados con respuestas correctas, incorrectas y porcentaje final.

Las decisiones disponibles son **Parece seguro** y **Reportar como sospechoso**. Los ejemplos no contienen enlaces activos ni descargas.

## Contenido educativo

Los escenarios trabajan señales como:
- Mensajes que generan urgencia o amenazan con bloquear una cuenta.
- Premios inesperados y ofertas demasiado buenas.
- Alertas alarmistas de supuestos virus.
- Solicitudes para instalar aplicaciones o archivos desconocidos.
- Pedidos de contraseñas, datos personales o pagos.
- Verificación del anunciante y consulta de canales oficiales.

## Pantallas simuladas de plataformas

Durante la práctica, cada anuncio aparece dentro de una recreación de la interfaz de la plataforma y de su ubicación publicitaria:
- **YouTube:** in-feed dentro de una cuadrícula de recomendaciones, in-stream dentro de un reproductor con temporizador y opción de omitir, y Shorts en un visor vertical con acciones laterales.
- **Google Play:** resultado patrocinado dentro de la lista de búsqueda, tarjetas de anuncios sugeridos y ficha de aplicación con desarrollador, valoración, descargas, botón de instalación y capturas promocionales.
- **TikTok:** feed vertical Para ti con barra de estado, pestañas, video, perfil, etiqueta de publicidad, acciones laterales, CTA y navegación inferior; el carrusel muestra varias piezas y TopView se identifica como anuncio de apertura.

**Estado de los recursos visuales:** el controlador todavía contiene miniaturas dibujadas por código. Esa parte está pendiente de reemplazo; no se considera terminada ni se deben confundir las referencias de investigación con imágenes ya integradas al juego.

### Referencias visuales actuales

Las referencias se eligieron para comparar cada formato con capturas reales de las aplicaciones. El importador de Unity permite descargarlas al proyecto para inspección local; no las descarga durante el juego ni las incorpora automáticamente al HUD.

- YouTube In-feed: [captura de anuncio dentro del feed](https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png).
- YouTube In-stream: [captura móvil de la interfaz de anuncio y omisión](https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg).
- YouTube Shorts: [captura de anuncio vertical con sticker interactivo](https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png).
- Google Play Search: [captura de resultados de búsqueda con ubicación patrocinada (2025)](https://images.indianexpress.com/2025/09/Play-Store-Search-bar.jpeg).
- Google Play Suggested: [captura de recomendaciones patrocinadas en interfaz reciente (2026)](https://www.apptweak.com/img/2026/01/image8.png).
- Google Play Suggested, tema oscuro: [captura de sección Sponsored · Suggested for you](https://www.digitaltrends.com/tachyon/2024/06/Screenshot_20240606_160856_Google-Play-Store.jpg?resize=720%2C720).
- TikTok TopView: [ejemplo visual de TopView](https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png).
- TikTok Carousel: [ejemplo visual de anuncio carrusel](https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png).

Documentación oficial para comprobar el comportamiento y los elementos de cada formato:
- [Formatos oficiales de anuncios de YouTube](https://support.google.com/google-ads/answer/2375464?hl=es).
- [Anuncios de Shorts y ubicaciones compatibles](https://support.google.com/google-ads/answer/16040527).
- [Superficies y recursos visuales de Google Play](https://support.google.com/googleplay/android-developer/answer/9866151).

Las capturas externas son referencias de interfaz de terceros. Antes de distribuirlas dentro del juego hay que comprobar sus licencias y permisos; el importador las guarda en una carpeta de referencia, no las añade al build.

### Importar las referencias en Unity

1. Abrir el proyecto con Unity 2022.3.62f3.
2. Esperar a que compile el script del Editor.
3. Ejecutar **Click Inteligente > Importar referencias visuales**.
4. Revisar los archivos descargados en `Assets/ReferenceScreenshots/Platforms`.

El importador informa qué descargas fallaron. No sustituye todavía las miniaturas del HUD; esa integración debe realizarse después de inspeccionar las imágenes importadas y seleccionar recursos utilizables con permisos adecuados.

## Videos tutoriales

El controlador admite clips mediante el arreglo `videosTutoriales` del componente `ClickInteligenteController` en el Canvas de la escena `Menu`.

Para incorporarlos:
1. Importar los archivos de video al proyecto como assets `VideoClip`.
2. Seleccionar el objeto `Canvas` de la escena `Menu`.
3. Asignar los clips al arreglo `Videos Tutoriales` desde el Inspector, en el orden de las páginas del tutorial.
4. Probar la reproducción en el Editor y en cada plataforma de destino.

El repositorio inspeccionado no contenía clips de video; por eso el tutorial muestra un espacio informativo cuando no hay uno asignado.

## Relación con la investigación

La puntuación del videojuego mide el desempeño durante la práctica. No sustituye los cuestionarios inicial y final previstos en la metodología de investigación, que deben aplicarse a los tres grupos con contenidos equivalentes.

## Comprobación manual pendiente

- Abrir el proyecto con Unity 2022.3.62f3.
- Esperar la importación y confirmar que no haya errores de compilación.
- Ejecutar la escena Menu.
- Completar las tres páginas del tutorial.
- Clasificar todos los anuncios y revisar la explicación posterior a cada respuesta.
- Confirmar el conteo de respuestas y el porcentaje final.
- Repetir la partida y comprobar la mezcla de anuncios.
- Probar diferentes resoluciones y dispositivos de destino.
