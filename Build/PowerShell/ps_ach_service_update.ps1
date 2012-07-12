$service = "ach-service"
$subID = "e8636035-1760-468b-9d18-324a989af982"
$subName = "Windows Azure MSDN - Visual Studio Ultimate"
$thumbprint = "D73E3662A5C781CF8194A121872E1999AA4C6109"
$package = "D:\Projects\Quad\ach.fulfillment\Source\Ach.Fulfillment.Service.Azure\bin\Release\app.publish\Ach.Fulfillment.Service.Azure.cspkg"
$config = "D:\Projects\Quad\ach.fulfillment\Source\Ach.Fulfillment.Service.Azure\bin\Release\app.publish\ServiceConfiguration.Cloud.cscfg"
$storage = "portalvhdsxmg86s9pngrcg"
$slot = "Staging"
$label = "Stagev2.6"

#$cert = Get-Item cert:\\CurrentUser\My\$thumbprint 
$cert = Get-AzureCertificate -ServiceName $service

#Set-AzureSubscription –SubscriptionName $subName -SubscriptionId $subID -Certificate $cert -CurrentStorageAccount $storage
Set-AzureSubscription –SubscriptionName $subName -SubscriptionId $subID -CurrentStorageAccount $storage

Set-AzureDeployment -Upgrade -ServiceName $service –Package $package -Configuration $config -Slot $slot -Label $label

exit