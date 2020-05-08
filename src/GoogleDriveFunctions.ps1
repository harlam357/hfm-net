# https://monteledwards.com/2017/03/05/powershell-oauth-downloadinguploading-to-google-drive-via-drive-api/

Function Get-GoogleAuthToken {
    param([Parameter(Mandatory=$true)]
          [string]$RefreshToken,
          [Parameter(Mandatory=$true)]
          [string]$ClientID,
          [Parameter(Mandatory=$true)]
          [string]$ClientSecret)

    $grantType = 'refresh_token' 
    $requestUri = 'https://accounts.google.com/o/oauth2/token' 
    $authBody = "refresh_token=$RefreshToken&client_id=$ClientID&client_secret=$ClientSecret&grant_type=$grantType" 
    $response = Invoke-RestMethod -Method Post -Uri $requestUri -ContentType 'application/x-www-form-urlencoded' -Body $authBody 
    return $response.access_token
}

Function Upload-ToGoogleDrive {
    param([Parameter(Mandatory=$true)]
          [string]$RefreshToken,
          [Parameter(Mandatory=$true)]
          [string]$ClientID,
          [Parameter(Mandatory=$true)]
          [string]$ClientSecret,
          [Parameter(Mandatory=$true)]
          [string]$ParentFolder, 
          [Parameter(Mandatory=$true)]
          $File)

    $accessToken = Get-GoogleAuthToken -RefreshToken $RefreshToken -ClientID $ClientID -ClientSecret $ClientSecret
    $fileContentBytes = [System.IO.File]::ReadAllBytes($File.FullName)
    $fileContentBase64 = [Convert]::ToBase64String($fileContentBytes)
    $inputMime = 'application/octet-stream'

## Begin HTTP Body
    $body= @"
--BOUNDARY
Content-Type: application/json; charset=UTF-8

{
    "name": "$($File.Name)",
    "parents": [
        "$ParentFolder" 
    ],
    "mimeType": "$inputMime"
}
--BOUNDARY
Content-Type: $inputMime
Content-Transfer-Encoding: base64

$fileContentBase64
--BOUNDARY--

"@
## End HTTP Body
    
    # Calculates the size in bytes of the HTTP POST body, necessary for Content-Length in the request header 
    $bodyLength = ($body | Measure-Object -Property Length -Sum).Sum

    $requestHeaders = @{"Authorization" = "Bearer $accessToken" 
                        "Content-type" = 'multipart/related; boundary="BOUNDARY"'
                        "Content-Length" = "$bodyLength"}

    $requestUri = 'https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart'

    try {
        $response = Invoke-WebRequest -Method Post -Uri $requestUri -Body $body -Headers $requestHeaders -UseBasicParsing
        $response
    } catch {
        $_.Exception
    }
}