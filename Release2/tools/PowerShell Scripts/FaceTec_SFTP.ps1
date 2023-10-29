# Load WinSCP .NET assembly
Add-Type -Path "C:\Program Files (x86)\WinSCP\WinSCPnet.dll"


# Set up session options
$sessionOptions = New-Object WinSCP.SessionOptions -Property @{
    Protocol = [WinSCP.Protocol]::Sftp
    HostName = "54.70.182.147"
    UserName = "306395-KCekJaK8"
    SshHostKeyFingerprint = "ssh-rsa 2048 vrp+Lg1504O/nnn80MxjaBWwlHvVBi3CwNaSL7kt5ME="
    SshPrivateKeyPath = "C:\Users\omkarsunku\Desktop\Keys\MonniPri.ppk"
    PrivateKeyPassphrase = "monni"
}

$session = New-Object WinSCP.Session
$localPath = "C:\Users\omkarsunku\Desktop\Custom_Server_SDK\Custom_Server_SDK\facetec-usage-logs\"
$RemotePath = "/ft-logs-sftp-production/306395-D7CKcVUd/"
$OutputSuccesPath  = "C:\Users\omkarsunku\Desktop\SFTP scripts\SFTP_OutputSuccess.txt"
$OutputFailedPath  = "C:\Users\omkarsunku\Desktop\SFTP scripts\SFTP_OutputFailed.txt"

$latest = Get-ChildItem -Path $localPath | Sort-Object LastAccessTime -Descending | Select-Object -First 1
Write-Host $latest.name
$localPath = $localPath + $latest.name
try
{
       # Connect
	$session.Open($sessionOptions)
 
        # Upload files, collect results
        $transferResult = $session.PutFiles($localPath, $remotePath)
 
        # Iterate over every transfer
        foreach ($transfer in $transferResult.Transfers)
        {
            # Success or error?
            if ($transfer.Error -eq $Null)
            {
                Write-Output "Upload of $($transfer.FileName) succeeded and Date and time is: $((Get-Date).ToString())" | Out-File $OutputSuccesPath -Append
            }
            else
            {
                Write-Output "Upload of $($transfer.FileName) failed: $($transfer.Error.Message) and Date and time is: $((Get-Date).ToString())" | Out-File $OutputFailedPath -Append
            }
        }
	

    exit 0
}
catch
{
    Write-Host "Error: $($_.Exception.Message)"
    Write-Output "Error: $($_.Exception.Message) and Date and time is: $((Get-Date).ToString())" | Out-File $OutputFailedPath -Append
    exit 1
}
finally	
{
	# Disconnect, clean up
	$session.Dispose()
}