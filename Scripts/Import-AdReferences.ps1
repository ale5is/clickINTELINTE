$ErrorActionPreference = "Continue"
$destination = Join-Path $PSScriptRoot "..\Assets\Resources\AdReferences"
New-Item -ItemType Directory -Force -Path $destination | Out-Null

$sources = @(
    @{ Name = "YouTube_InFeed_2023"; Url = "https://strikesocial.com/wp-content/uploads/2023/10/youtube-video-view-campaign-safe-zone-in-feed-video-placement.png" },
    @{ Name = "YouTube_InStream_2025"; Url = "https://i.blogs.es/02f9cc/publicidad-youtube-2/450_1000.jpeg" },
    @{ Name = "YouTube_Shorts_Interactive_2025"; Url = "https://images.squarespace-cdn.com/content/v1/644354d16184622c5b9e06f6/ae95cbf9-dd19-4fce-b6ec-4db5d12e77b2/Stickers%2Bon%2BShorts.png" },
    @{ Name = "GooglePlay_SearchSponsored_2025"; Url = "https://images.indianexpress.com/2025/09/Play-Store-Search-bar.jpeg" },
    @{ Name = "GooglePlay_SuggestedDark_2026"; Url = "https://www.apptweak.com/img/2026/01/image8.png" },
    @{ Name = "GooglePlay_SuggestedLight_2024"; Url = "https://www.digitaltrends.com/tachyon/2024/06/Screenshot_20240606_160856_Google-Play-Store.jpg?resize=720%2C720" },
    @{ Name = "TikTok_InFeed_Creative_2026"; Url = "https://lf-tt4b.tiktokcdn.com/obj/i18nblog/tt4b_cms/en-US/tipdilz7wysq-52DY2FWG6YW1lkjzUCrIi7.jpg" },
    @{ Name = "TikTok_TopView_2022"; Url = "https://megadigital.ai/wp-content/uploads/2022/08/quang-cao-top-view-tiktok-la-gi.png" },
    @{ Name = "TikTok_Carousel_2025"; Url = "https://cdn.prod.website-files.com/63343441a1da9b07cf060122/68dc12472457a9d475b28075_ccd4e5ad.png" }
)

$downloaded = 0
$failed = 0

foreach ($source in $sources) {
    $temporary = Join-Path $destination ($source.Name + ".download")
    try {
        Invoke-WebRequest -Uri $source.Url -OutFile $temporary -TimeoutSec 30 -UseBasicParsing
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

        Get-ChildItem -Path $destination -Filter ($source.Name + ".*") -File | Remove-Item -Force
        Move-Item -Path $temporary -Destination (Join-Path $destination ($source.Name + $extension)) -Force
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

Write-Host ("Referencias descargadas: " + $downloaded + " / " + $sources.Count)
Write-Host ("Referencias no disponibles: " + $failed)
