Clear-Host
## Automate FTP uploads
## Go to destination
$baseDirectory = $PSScriptRoot 
$location = "$baseDirectory\test"
"We are here: $location"
## Get files
$files = Get-ChildItem $location -recurse  
## Get ftp object
$ftp_client = New-Object System.Net.WebClient
$ftp_address = "ftps://waws-prod-sn1-123.ftp.azurewebsites.windows.net"

#ftp server params
$ftp = 'ftps://waws-prod-sn1-123.ftp.azurewebsites.windows.net'
$user = 'kachuwa\kachuwa'
$pass = 'nepali1234%'

#Connect to ftp webclient
$ftp_client.Credentials = New-Object System.Net.NetworkCredential($user,$pass)

Register-ObjectEvent -InputObject $ftp_client -EventName "UploadProgressChanged" -Action { Write-Progress -Activity "Upload progress..." -Status "Uploading" -PercentComplete $EventArgs.ProgressPercentage } > $null

## Make uploads
foreach($file in $files)
{
    
    $directory = "";
    $source = $file.DirectoryName + "\" + $file;
    if ($File.DirectoryName.Length -gt 0)
    {
        $directory = $file.DirectoryName.Replace($Location,"")
    }
   
   try{
        $FtpCommand = $ftp_address + $directory+ "/"+ $file
        $FtpCommand=$FtpCommand.Replace("\","/")
        Write-Host "$FtpCommand $file"
        $uri = New-Object System.Uri "$FtpCommand"
        $webclient.UploadFileAsync($uri,"STOR", $file)
          Write-Host "upload"
    }
    catch  [Net.WebException]
    {
        Write-Host $_.Exception.ToString() -foregroundcolor red
    }
    while ($webclient.IsBusy)
     {
        continue 
     }
}



