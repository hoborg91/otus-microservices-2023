param(
	[string] $uri = "arch.homework/otusapp/hoborg91/test",
	[int] $intervalMilliseconds = 1000
)

$i = 0
while ($true) {
	$i++
	$startAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
	$requestResult = Invoke-WebRequest -Uri $uri
	$finishAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
	Write-Host "[$startAt -> $finishAt] $i : StatusCode = $($requestResult.StatusCode), Content = $($requestResult.Content)"
	Start-Sleep -Milliseconds $intervalMilliseconds
}
