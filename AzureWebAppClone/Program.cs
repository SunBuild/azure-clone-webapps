using Microsoft.Web.Deployment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;
using System.Xml;
using System.Xml.Linq;

namespace AzureWebAppClone
{
    class Program
    {
        public class CloneAppArgs
        {
            [ArgRequired]
            [ArgDescription("Source Web App publish settings file ")]
            public string sourceSitePublishSettings { get; private set; }
            [ArgRequired]
            [ArgDescription("Destination Web App publish settings file ")]
            public string destSitePublishSettings { get; private set; }
        }

       public  class Profile
        {
            public string userPwd { get; set; }
            public string userName { get;  set; }
            public string mysqlConnectionstring { get;  set; }
            public string sqlazureconnectionstring { get;  set; }
            public string sitename { get;  set; }
            public string publishUrl { get;  set; }

            public string destinationUrl { get;  set; } 
        }
         static void Main(string[] args)
        {
            try {
                //Parse publish profile for source and destination sites 
                //  string sourcedb, destdb, sitename;
                CloneAppArgs app_args = Args.Parse<CloneAppArgs>(args);

                XmlDocument destsettings = new XmlDocument();
                destsettings.Load(app_args.destSitePublishSettings);

                XmlDocument sourcesettings = new XmlDocument();
                sourcesettings.Load(app_args.sourceSitePublishSettings);

                Profile srcprofiledata = getProfile(sourcesettings);
                Profile destprofiledata = getProfile(destsettings);

                string dbType = GetDBtype(srcprofiledata, destprofiledata);
                if (!string.IsNullOrEmpty(dbType))
                {
                    SyncDatabases(srcprofiledata, destprofiledata, dbType);
                }
                else
                    Console.WriteLine("No databases to sync");

                SyncWebApps(srcprofiledata, destprofiledata);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
          
        }

        public static string GetDBtype(Profile src , Profile dest)
        {
            
            if (!String.IsNullOrEmpty(src.mysqlConnectionstring ) && !string.IsNullOrEmpty(dest.mysqlConnectionstring))
            {
                return "mysql";
            }
            if (!String.IsNullOrEmpty(src.sqlazureconnectionstring) && !string.IsNullOrEmpty(dest.sqlazureconnectionstring))
            {
                return "sql";
            }

            return string.Empty;


        }

        public static Profile  getProfile(XmlDocument doc )
        {
            
            XmlElement root = doc.DocumentElement;
            XmlNodeList list = root.SelectNodes("//publishProfile");

            Profile profiledata = new Profile();

            foreach (XmlNode node in list)
            {

                if (node.Attributes["profileName"].Value.Contains("Web Deploy"))
                {
                    profiledata.userPwd = node.Attributes["userPWD"].Value;
                    profiledata.userName = node.Attributes["userName"].Value;
                    profiledata.mysqlConnectionstring = node.Attributes["mySQLDBConnectionString"].Value;
                    profiledata.sqlazureconnectionstring = node.Attributes["SQLServerDBConnectionString"].Value;
                    profiledata.publishUrl = node.Attributes["publishUrl"].Value;
                    profiledata.sitename = node.Attributes["msdeploySite"].Value;
                    profiledata.destinationUrl = node.Attributes["destinationAppUrl"].Value;

                    return profiledata;

                }


            }
            return null;
            
        }
        public static void SyncDatabases(Profile src, Profile dest , string dbtype)
        {
            DeploymentSyncOptions syncOptions = new DeploymentSyncOptions();
            DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();            
            DeploymentBaseOptions sourceBaseOptions = new DeploymentBaseOptions();
            destBaseOptions.Trace += TraceEventHandler;
            sourceBaseOptions.Trace += TraceEventHandler;
            destBaseOptions.TraceLevel = TraceLevel.Verbose;
            sourceBaseOptions.TraceLevel = TraceLevel.Verbose;

            DeploymentProviderOptions destProviderOptions = null;

            DeploymentObject sourceObj = null;
            if (dbtype.Equals("mysql", StringComparison.InvariantCultureIgnoreCase))
            {
                destProviderOptions = new DeploymentProviderOptions(DeploymentWellKnownProvider.DBMySql);
                destProviderOptions.Path = dest.mysqlConnectionstring;
               sourceObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.DBMySql, src.mysqlConnectionstring, sourceBaseOptions);

            }
            else if (dbtype.Equals("sql", StringComparison.InvariantCultureIgnoreCase))
            {
                destProviderOptions = new DeploymentProviderOptions(DeploymentWellKnownProvider.DBFullSql);
                destProviderOptions.Path = dest.sqlazureconnectionstring;
                sourceObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.DBFullSql,src.sqlazureconnectionstring, sourceBaseOptions);
            }
            if (sourceObj != null)
            {

                sourceObj.SyncTo(destProviderOptions, destBaseOptions, syncOptions);

            }
            

        }
        public static void SyncWebApps(Profile src, Profile dest)
        {
            DeploymentSyncOptions syncOptions = new DeploymentSyncOptions()
            {
                Rules =
                {
                    new DeploymentSkipRule("SkipASearch", "AddChild", "dirPath", "app.data.search", ""),
                    new DeploymentSkipRule("SkipUSearch", "Update", "dirPath", "app.data.search", "")
                }
            };

            DeploymentBaseOptions sourceBaseOptions = new DeploymentBaseOptions();
            sourceBaseOptions.ComputerName = "https://" + src.publishUrl + "/msdeploy.axd";
            sourceBaseOptions.UserName = src.userName;
            sourceBaseOptions.Password = src.userPwd;
            sourceBaseOptions.AuthenticationType = "basic";

            sourceBaseOptions.Trace += TraceEventHandler;

            sourceBaseOptions.TraceLevel = TraceLevel.Verbose;
            
            DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();
            destBaseOptions.ComputerName = "https://"+dest.publishUrl+"/msdeploy.axd";
            destBaseOptions.UserName = dest.userName;
            destBaseOptions.Password = dest.userPwd;
            destBaseOptions.AuthenticationType = "basic";
            

            destBaseOptions.Trace += TraceEventHandler;

            destBaseOptions.TraceLevel = TraceLevel.Verbose;

            DeploymentProviderOptions destProviderOptions = new DeploymentProviderOptions(DeploymentWellKnownProvider.ContentPath);
            destProviderOptions.Path = dest.sitename;
            DeploymentObject sourceObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, src.sitename,sourceBaseOptions);
            sourceObj.SyncTo(destProviderOptions, destBaseOptions, syncOptions); 
            
        }

        static void TraceEventHandler(object sender, DeploymentTraceEventArgs e)
        {

            Console.WriteLine(e.Message);

        }




    }
}
