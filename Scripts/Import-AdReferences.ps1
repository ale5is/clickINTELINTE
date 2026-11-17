$ErrorActionPreference = "Stop"

$referenceDestination = Join-Path $PSScriptRoot "..\Assets\Resources\AdReferences"
$creativeDestination = Join-Path $PSScriptRoot "..\Assets\Resources\AdCreatives"

$referenceSources = @(
    @{ Name = "YouTube_InFeed_2023"; Url = "https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png" },
    @{ Name = "YouTube_InStream_2025"; Url = "https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg" },
    @{ Name = "YouTube_Shorts_Interactive_2025"; Url = "https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png?format=1000w" },
    @{ Name = "GooglePlay_SearchSponsored_2026"; Url = "https://cdn.sp-selfservice.com/ml/7200426-f-3dcd3bb1-7fa4-4714-97d0-3dbee21eaf98.png?height=904&width=874" },
    @{ Name = "GooglePlay_SuggestedDark_2026"; Url = "https://www.apptweak.com/img/2026/01/image8.png" },
    @{ Name = "GooglePlay_SuggestedLight_2026"; Url = "https://www.apptweak.com/img/2026/01/image8.png" },
    @{ Name = "TikTok_InFeed_Screen_2025"; Url = "https://support.ecwid.com/hc/article_attachments/15645628291996" },
    @{ Name = "TikTok_TopView_2022"; Url = "https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png" },
    @{ Name = "TikTok_Carousel_2025"; Url = "https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png" }
)

$creativeSources = @(
    @{ Name = "FakeVirus_2020"; Url = "https://www.pcrisk.es/images/stories/screenshots202004/your-windows-10-is-infected-with-5-viruses-update-2020-04-07-another-variant.jpg" },
    @{ Name = "FakePrize_iPhone14_2023"; Url = "https://www.pcrisk.pl/images/stories/screenshots202303/apple-iphone-14-winner-scam-main.jpg" },
    @{ Name = "FakeAPK_SuperMario_2017"; Url = "https://images.contentstack.io/v3/assets/blt281ecbfc2563bf9b/blt816b52937e07916e/6643f697b444a97e41d83d91/2_mario_install.png" },
    @{ Name = "FakeAccount_2023"; Url = "https://uploads-ssl.webflow.com/5767b4eb3a9c9e1015d06770/650dab46144e5bb3e4a474ff_klient-screen.png" },
    @{ Name = "Real_Loreal_DermaControl_2025"; Url = "https://www.loreal-paris.es/-/media/project/loreal/brand-sites/oap/emea/es/products/men/cuidado-facial/derma-control/reno2025/banner-derma-control.jpg" },
    @{ Name = "Real_KhanAcademy_2026"; Url = "https://cdn.adswipefile.com/tracking/159403248441/961728760044823.jpg" },
    @{ Name = "Real_Argentina_Freedom_2025"; Url = "https://www.continental.com.ar/adjuntos/1200/continental/2025/10/11-freedom.jpg" }
)

function Import-ImageSources {
    param(
        [object[]]$Sources,
        [string]$Destination
    )

    New-Item -ItemType Directory -Force -Path $Destination | Out-Null
    $downloaded = 0
    $failed = 0

    foreach ($source in $Sources) {
        $temporary = Join-Path $Destination ($source.Name + ".download")
        try {
            Invoke-WebRequest -Uri $source.Url -OutFile $temporary -TimeoutSec 30 -UseBasicParsing -Headers @{ Accept = "image/png,image/jpeg,*/*;q=0.1" }
            $bytes = [System.IO.File]::ReadAllBytes($temporary)
            $isPng = $bytes.Length -ge 8 -and $bytes[0] -eq 0x89 -and $bytes[1] -eq 0x50 -and $bytes[2] -eq 0x4E -and $bytes[3] -eq 0x47 -and $bytes[4] -eq 0x0D -and $bytes[5] -eq 0x0A -and $bytes[6] -eq 0x1A -and $bytes[7] -eq 0x0A
            $isJpeg = $bytes.Length -ge 3 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xD8 -and $bytes[2] -eq 0xFF

            if ($isPng) {
                $extension = ".png"
            }
            elseif ($isJpeg) {
                $extension = ".jpg"
            }
            else {
                throw "La respuesta no contiene una imagen PNG/JPEG válida."
            }

            Remove-Item -Path (Join-Path $Destination ($source.Name + ".png")) -Force -ErrorAction SilentlyContinue
            Remove-Item -Path (Join-Path $Destination ($source.Name + ".jpg")) -Force -ErrorAction SilentlyContinue
            Move-Item -Path $temporary -Destination (Join-Path $Destination ($source.Name + $extension)) -Force -ErrorAction Stop
            $downloaded++
            Write-Host ("OK " + $source.Name + $extension)
        }
        catch {
            $failed++
            Write-Warning ($source.Name + ": " + $_.Exception.Message)
            if (Test-Path $temporary) {
                Remove-Item $temporary -Force
            }
        }
    }

    Write-Host ("Recursos descargados: " + $downloaded + " / " + $Sources.Count + " en " + $Destination)
    Write-Host ("Recursos no disponibles: " + $failed)
    return $downloaded
}

$referenceDownloaded = Import-ImageSources -Sources $referenceSources -Destination $referenceDestination
$creativeDownloaded = Import-ImageSources -Sources $creativeSources -Destination $creativeDestination

if ($referenceDownloaded -ne $referenceSources.Count) {
    throw "No se descargaron todas las referencias visuales de plataforma. Descargadas: $referenceDownloaded / $($referenceSources.Count)."
}

if ($creativeDownloaded -ne $creativeSources.Count) {
    throw "No se descargaron todas las creatividades publicitarias. Descargadas: $creativeDownloaded / $($creativeSources.Count). Se cancela la compilación para no generar un ejecutable incompleto."
}
