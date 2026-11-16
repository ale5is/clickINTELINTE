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

Las creatividades usan miniaturas ficticias originales generadas por el propio proyecto en tiempo de ejecución: 16 diseños visuales distintos (mascotas, autos, videojuegos, ciencia, comida, naturaleza, deportes, arte y otros temas), con paletas y composiciones variadas. Cada formato selecciona miniaturas diferentes para evitar repetir siempre la misma imagen. Las cabeceras incluyen marcas visuales reconocibles de YouTube, Google Play y TikTok, recreadas con elementos UI y sin depender de logos descargados. Las capturas externas se usan como referencia de distribución y jerarquía visual, no se incorporan al juego como fondos ni se descargan en tiempo de ejecución. Los botones publicitarios son simulados y no ejecutan descargas ni abren enlaces.

### Referencias visuales consultadas

- [Captura de la página principal y página de reproducción de YouTube (PDF, figuras 1 y 3)](https://www.accc.gov.au/system/files/Digital%20Platform%20Services%20Inquiry%20-%20Social%20Media%20Interim%20Report%20-%20Public%20Submission%20-%20Google%20Australia%20-%20November%202022.pdf?download=y)
- [Formatos oficiales de anuncios de YouTube](https://support.google.com/google-ads/answer/2375464?hl=es)
- [Capturas de resultados patrocinados y sugerencias de Google Play](https://www.radaso.com/blog/google-play-store-advertising-android-app-promotion-in-2024)
- [Guía oficial de capturas y superficies de Google Play](https://support.google.com/googleplay/android-developer/answer/9866151)
- [Capturas de TikTok For You y etiqueta de publicidad (Anexo 5, PDF)](https://commission.europa.eu/document/download/bb70bfd5-7a26-471a-b265-7a59f0426a57_en?filename=2022.06.15_tiktok_updated_commitments.pdf&prefLang=hu)
- [Diseño web modular de TikTok para escritorio](https://newsroom.tiktok.com/new-features-bring-tiktok-magic-to-desktop?lang=en)
- [Ejemplos de ventanas emergentes fraudulentas y malware móvil](https://www.welivesecurity.com/la-es/2020/01/15/como-funciona-el-malware-dirigido-a-dispositivos-moviles/)

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
