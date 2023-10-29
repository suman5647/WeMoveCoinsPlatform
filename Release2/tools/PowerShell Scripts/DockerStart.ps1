#start the docker container only if the docker is started
Function Start-Docker-Container {
	# Intialize parameters
	$name = "facetec_custom_server"
	$cID = $(docker ps -qf "name=$name")

	# Check that container is running
    If($cID)
	{
        Write-Output "The dev container $name is already running with ID: $cID";
    }
    Else 
	{
        Write-Output "Starting dev container $name...";
		Set-Location "E:\services\facetec\9.0";
        docker-compose up;
    }    
}

#Get the docker desktop status
Function Get-Docker-Process {
	return Get-Process 'Docker Desktop' -ErrorAction SilentlyContinue
}

$processes =  Get-Docker-Process
If ($processes.Count -gt 0)
{
    Write-Output "Docker is already running"
    Start-Docker-Container
}
Else 
{
	Write-Output "Docker is not running"
	Do {

		$status = Get-Process 'Docker Desktop' -ErrorAction SilentlyContinue

		If (!($status)) 
		{
			Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe" 
			Write-Host 'Waiting for process to start'  
			Start-Sleep -Seconds 60
		}
		Else 
		{ 
			Write-Host 'Process has started' 
			$started = $true 
		}

	}
	Until ( $started )
	$processes =  Get-Process 'Docker Desktop'

	If ($processes.Count -gt 0)
	{
		Write-Output "Docker started running and next start container"
		Start-Sleep -Seconds 60
		Start-Docker-Container
	} 
	Else 
	{
		Start-Sleep -Seconds 120
		Start-Docker-Container
	}
}	
