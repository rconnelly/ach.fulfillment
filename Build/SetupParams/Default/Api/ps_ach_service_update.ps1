$service = "api"
$subID = "e8636035-1760-468b-9d18-324a989af982"
$subName = "Windows Azure MSDN - Visual Studio Ultimate"
$thumbprint = "D73E3662A5C781CF8194A121872E1999AA4C6109"
$package = ".\Api\Ach.Fulfillment.Api.Azure.cspkg"
$config = ".\Api\ServiceConfiguration.${CloudTargetProfile}.cscfg"
$storage = "portalvhdsxmg86s9pngrcg"
$slot = "Staging"
$label = "Stagev${ProductVersion}"

#$cert = Get-Item cert:\\CurrentUser\My\$thumbprint 
#Set-AzureSubscription -SubscriptionName $subName -SubscriptionId $subID -Certificate $cert -CurrentStorageAccount $storage
Set-AzureSubscription -SubscriptionName $subName -SubscriptionId $subID -CurrentStorageAccount $storage

$exist = Get-AzureDeployment -ServiceName $service -Slot $slot
if($exist)
{
	Set-AzureDeployment -Upgrade -ServiceName $service -Package $package -Configuration $config -Slot $slot -Label $label
}
else
{
	New-AzureDeployment -ServiceName $service -Package $package -Configuration $config -Slot $slot -Label $label
}

exit