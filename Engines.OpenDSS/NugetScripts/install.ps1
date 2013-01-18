try {
$obj = New-Object -ComObject OpenDSSengine.DSS
}
catch {
	$null = [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
	$yesno = [System.Windows.Forms.MessageBox]::Show("It looks like you don't have OpenDSS installed. Head over to http://sourceforge.net/projects/electricdss/ and grab a copy?", "Hey!", 4)
	if ($yesno -eq "YES")
	{
		start 'http://sourceforge.net/projects/electricdss/'
	}
}