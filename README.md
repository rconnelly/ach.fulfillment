## Deployment
1. Download sources from github repository.
2. Create Database on your Database Server.
3. Go to \Build\ folder and run Build.Local.cmd
4. Go to \Build\Assembly\ folder, open setup.cmd and change connection string to proper one. 
5. Go to \Build\Assembly\Packages\ folder, open Ach.Fulfillment.Web.SetParameters.xml and change connection string to proper one.
6. Run RunSetup.cmd
7. Go to Web folder, open Web.config - here in section AppSettings we have   <add key ="DefaultUser" value="admin"/> - 
it is login for default user. It will be used for non-authentificated requests. We get company payment information from PartnerDetails for this user. 

## Job configuration
1. Go to quartz_jobs.config file 
2. Open "job-data-map" section - there for entry with key "AchFilesStore" you should put path to the directory where genarated files will be stored.
3. Open "trigger" section - there you can configure when your job will be run. Change cron-expression for your one. For more information look at http://en.wikipedia.org/wiki/Cron


## API

**CREATE TRANSACTION**

**POST**    >> http://servicehost/transaction

**Accept:** application/json

**Content-Type:** application/json

**Request Body:**

    {
      "IndividualIdNumber" : "123456789012345",
      "ReceiverName" : "ReceiverName",
      "EntryDescription":"PAYROLL",
      "EntryDate":"01.01.2012",
      "TransactionCode":"22", 
      "TransitRoutingNumber":"123456789",
      "DfiAccountId":"12345678901234567", 
      "Amount":"123,00", 
      "EntryClassCode":"PPD", 
      "CallbackUrl":"http://send.callback.here",
      "PaymentRelatedInfo":"",
      "ServiceClassCode":"200"
    }


