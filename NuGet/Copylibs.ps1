Remove-Item ".\Temp\*" -Recurse

Copy-Item "..\scr\Plugin.LocalNotification\bin\Release\Plugin.LocalNotification.*.nupkg" -Destination '.\Temp\nupkg.zip'
#Copy-Item "..\scr\Plugin.LocalNotification\bin\Release\Plugin.LocalNotification.*.snupkg" -Destination '.\Temp\snupkg.zip'

Expand-Archive -Path '.\Temp\nupkg.zip' -DestinationPath "Temp\pkg"
#Expand-Archive -Path '.\Temp\snupkg.zip' -DestinationPath "Temp\spkg"

Copy-Item '.\Temp\pkg\lib\*' -Destination '.\pkglib' -Recurse -ErrorAction SilentlyContinue
#Copy-Item '.\Temp\spkg\lib\*' -Destination '.\spkglib' -Recurse -ErrorAction SilentlyContinue