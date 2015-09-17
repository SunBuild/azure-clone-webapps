# azure-clone-webapps
Allows you to clone one web app to another web app on azure app service web apps 

#Usage
Download the tool from Release folder. Before running the tool download the source and destination web app's publish settings file from the Azure Portal .  If your application has a database , update SQLServerDBConnectionString (if using sql azure databse) or mySQLDBConnectionString ( if using mysql datbase) to point to remote database associated with the app 

   AzureWebAppClone.exe -s <source publish settings> -d <dest publish settings>

#Example

AzureWebAppClone.exe -s "c:\publish\site1.publishsettings" -d "c:\publish\site2.publishsettings"

#Sample Publish settings file 
```
<publishData>
 <!-- Web deploy publish method is used by this tool -->
   <publishProfile profileName="phpsimplesite-stage - Web Deploy" 
     publishMethod="MSDeploy" publishUrl="phpsimplesite-stage.scm.azurewebsites.net:443" 
     msdeploySite="phpsimplesite__stage" userName="$phpsimplesite__stage"         
     userPWD="lk49jRLwy9Cmn0Ecy6ZunzflTcvjjiY6rQtNEaLn5Wwdsn5p3lnJZoqJJ7mp" 
     destinationAppUrl="http://phpsimplesite-stage.azurewebsites.net" SQLServerDBConnectionString="" 
     mySQLDBConnectionString="Database=wordpress2stage;Data Source=us-cdbr-azure-west-c.cloudapp.net;User 
     Id=b1e3c09216e681;Password=22po09f4" 
     hostingProviderForumLink=""
     controlPanelLink="http://windows.azure.com" 
     webSystem="WebSites">
     <databases>
         <add name="default" connectionString="Database=wordpress2stage;Data Source=us-cdbr-azure-west-c.cloudapp.net;User Id=b1e3c01216e681;Password=22a809f4" providerName="MySql.Data.MySqlClient" type="MySql"/>
     </databases>
   </publishProfile>
   <!-- This section is below is not used by the tool -->
   <publishProfile profileName="phpsimplesite-stage - FTP" 
      publishMethod="FTP" publishUrl="ftp://waws-prod-bay-019.ftp.azurewebsites.windows.net/site/wwwroot" 
      ftpPassiveMode="True" 
      userName="phpsimplesite__stage\$phpsimplesite__stage" 
      userPWD="lk49jRLwy9Cmn0Ec4rZunzflTcvjjiY6rQtNEaLn5Wwdsn5p3lnJZoqJJ7mp" 
      destinationAppUrl="http://phpsimplesite-stage.azurewebsites.net" SQLServerDBConnectionString="" 
      mySQLDBConnectionString="Database=wordpress2stage;Data Source=us-cdbr-azure-west-c.cloudapp.net;User Id=b1e3c01de16e681;Password=22op09f4" 
      hostingProviderForumLink="" 
      controlPanelLink="http://windows.azure.com" webSystem="WebSites">
   <databases>
       <add name="default" connectionString="Database=wordpress2stage;Data Source=us-cdbr-azure-west-c.cloudapp.net;User Id=b1e3c01216e681;Password=22a809f4" providerName="MySql.Data.MySqlClient" type="MySql"/>
    </databases>
    </publishProfile>
  </publishData>
```
