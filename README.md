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

Durante la práctica, las variantes de anuncio se presentan con capturas reales de la plataforma; TikTok In-Feed conserva una recreación del feed y utiliza una creatividad fotográfica real:
- **YouTube:** cada práctica muestra una captura real distinta para In-feed, In-stream o Shorts.
- **Google Play:** cada práctica muestra una captura real distinta para búsqueda patrocinada o sugerencias en tema oscuro/claro.
- **TikTok:** In-feed conserva el feed vertical y usa una creatividad real; Carrusel y TopView muestran sus referencias visuales reales sin envolverlas en otra interfaz inventada.

**Estado de integración:** las pantallas de práctica ya cargan referencias visuales reales desde `Resources/AdReferences`. YouTube usa una referencia distinta para in-feed, in-stream y Shorts; Google Play usa capturas separadas para búsqueda patrocinada y sugerencias; TikTok usa creatividades reales para in-feed, carrusel y TopView. Los textos superpuestos que describen el caso siguen siendo ficticios y están identificados como tales.

### Referencias visuales actuales

Las referencias se usan como imágenes de las pantallas de práctica, no como contenido generado. El juego las carga localmente desde `Resources/AdReferences`; no realiza solicitudes de red mientras se ejecuta.

- YouTube In-feed: [captura de anuncio dentro del feed](https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png).
- YouTube In-stream: [captura móvil de la interfaz de anuncio y omisión](https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg).
- YouTube Shorts: [captura de anuncio vertical con sticker interactivo](https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png).
- Google Play Search: [captura de resultados de búsqueda con ubicación patrocinada (2025)](https://images.indianexpress.com/2025/09/Play-Store-Search-bar.jpeg).
- Google Play Suggested: [captura de recomendaciones patrocinadas en interfaz reciente (2026)](https://www.apptweak.com/img/2026/01/image8.png).
- Google Play Suggested, tema oscuro: [captura de sección Sponsored · Suggested for you](https://www.digitaltrends.com/tachyon/2024/06/Screenshot_20240606_160856_Google-Play-Store.jpg?resize=720%2C720).
- TikTok In-Feed: [creatividad fotográfica de la guía oficial de TikTok for Business](https://lf-tt4b.tiktokcdn.com/obj/i18nblog/tt4b_cms/en-US/tipdilz7wysq-52DY2FWG6YW1lkjzUCrIi7.jpg).
- TikTok TopView: [ejemplo visual de TopView](https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png).
- TikTok Carousel: [ejemplo visual de anuncio carrusel](https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png).

Documentación oficial para comprobar el comportamiento y los elementos de cada formato:
- [Formatos oficiales de anuncios de YouTube](https://support.google.com/google-ads/answer/2375464?hl=es).
- [Anuncios de Shorts y ubicaciones compatibles](https://support.google.com/google-ads/answer/16040527).
- [Superficies y recursos visuales de Google Play](https://support.google.com/googleplay/android-developer/answer/9866151).

Las capturas y creatividades pertenecen a sus respectivos titulares. Su integración técnica no concede derechos de redistribución: antes de publicar o distribuir el juego hay que comprobar permisos/licencias o reemplazar cualquier recurso no autorizado. Las referencias se descargan durante la preparación del proyecto/build y se empaquetan localmente; el juego no consulta esos sitios en tiempo de ejecución.

### Importar las referencias en Unity

1. Abrir el proyecto con Unity 2022.3.62f3.
2. Esperar a que compile el script del Editor.
3. Ejecutar **Click Inteligente > Importar referencias visuales**.
4. Revisar los archivos descargados en `Assets/Resources/AdReferences`.
5. Ejecutar la escena `Menu`; las pantallas de práctica cargarán las imágenes desde `Resources`.

El workflow de GitHub Actions también intenta descargar las referencias antes de compilar Windows. Si una fuente externa no responde, el build continúa y la pantalla muestra qué referencia falta, en lugar de sustituirla por una ilustración inventada.

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
