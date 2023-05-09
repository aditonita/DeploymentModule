param(
$PasswordFtp = "Hello passwordFtp",
$PasswordTest = "Hello passwordTest",
$PasswordDb = "Hello passwordDb",
$Country = "country",
$Version = "version"
)

Write-Output "Test-2 script";
Write-Output $PasswordFtp;
Write-Output $PasswordTest;
Write-Output $PasswordDb;
Write-Output $Country;
Write-Output $Version;
#Rename-Item "c:\ftp\blabl.txt" "bla.log";
Write-Output (Get-Date);
sleep -s 5;
Start-Process "C:\Windows\System32\notepad.exe" -NoNewWindow -Wait
Write-Output (Get-Date);