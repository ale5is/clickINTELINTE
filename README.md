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

Durante la práctica, cada anuncio aparece dentro de una interfaz ilustrativa distinta según la plataforma y la ubicación publicitaria:
- **YouTube:** anuncios in-feed, in-stream y Shorts, con reproductor, miniatura, canal, etiqueta de patrocinado, descripción y llamada a la acción.
- **Google Play:** anuncios en búsqueda, recomendaciones y promoción de aplicaciones, con icono, ficha, valoración, descripción y botón de instalación ficticio.
- **TikTok:** anuncios in-feed, carrusel y TopView, con composición vertical, identidad del anunciante, texto, acciones sociales y llamada a la acción.

Las creatividades incluyen ilustraciones originales para premios falsos, alertas de virus, actualizaciones urgentes, descargas de juegos, aplicaciones y propuestas educativas. Se diseñaron tomando como referencia la estructura de anuncios y las capturas públicas de las plataformas, sin copiar imágenes, marcas de terceros ni publicidad ajena. Los recursos se dibujan con elementos UI de Unity: no requieren conexión a internet, imágenes remotas ni cuentas reales. Los botones publicitarios son simulados y no ejecutan descargas.

### Referencias visuales y de formatos

- [Formatos de anuncios de vídeo de YouTube](https://support.google.com/google-ads/answer/2375464?hl=es)
- [Anuncios de vídeo in-feed de YouTube](https://support.google.com/google-ads/answer/6227733?hl=es)
- [Anuncios In-Feed de TikTok](https://ads.tiktok.com/help/article/tiktok-auction-in-feed-ads?lang=es)
- [Anuncios carrusel de TikTok](https://ads.tiktok.com/resources/help/article/carousel-ads?lang=es)
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
