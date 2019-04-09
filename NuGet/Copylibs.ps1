Copy-Item "..\scr\Plugin.LocalNotification\bin\Release\Plugin.LocalNotification.*.nupkg" -Destination '.\Temp\nupkg.zip'
Expand-Archive -Path '.\Temp\nupkg.zip' -DestinationPath Temp
Copy-Item '.\Temp\lib' -Destination '.\' -Recurse -ErrorAction SilentlyContinue