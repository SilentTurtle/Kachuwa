#
# Edouard Kombo <edouard.kombo@gmail.com>
# 2014/02/19
# Powershell script
# Upload pictures through ftp
#

#Directory where to find pictures to upload
$baseDirectory = $PSScriptRoot 
$location = "$baseDirectory\test"
$Dir= $location

#Directory where to save uploaded pictures
$saveDir = "\"

#ftp server params
$ftp = 'ftps://waws-prod-sn1-123.ftp.azurewebsites.windows.net'
$user = 'kachuwa\kachuwa'
$pass = 'nepali1234%'

#Connect to ftp webclient
$webclient = New-Object System.Net.WebClient 
$webclient.Credentials = New-Object System.Net.NetworkCredential($user,$pass)  
 Write-Host "$webclient"
#Initialize var for infinite loop
$i=0

#Infinite loop
#while($i -eq 0){ 
   Write-Host " qwe"
    #Pause 1 seconde before continue
    #Start-Sleep -sec 1

    #Search for pictures in directory
    foreach($item in (dir $Dir "*"))
    {
          # Start-Sleep -s 2
            Write-Host "$item"
        #Set default network status to 1
        $onNetwork = "1"


            #If upload fails, we set network status at 0
            try{

                $uri = New-Object System.Uri($ftp+$item.Name)

                $webclient.UploadFile($uri, $item.FullName)

            } catch [Exception] {
                
                $onNetwork = "0"
                write-host $_.Exception.Message;
            }

            #If upload succeeded, we do further actions
            if($onNetwork -eq "1"){
                "Copying $item..."
                Copy-Item -path $item.fullName -destination $saveDir$item 

                "Deleting $item..."
                Remove-Item $item.fullName
            }


       
    }
#}	