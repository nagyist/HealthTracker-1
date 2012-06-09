using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.Configuration.Models;
using System.Configuration;

namespace HealthTracker.Configuration.Services
{
   [Export (typeof(IConfiguration))]
   public class ConfigurationRepository : IConfiguration
   {
      #region Constructors
      public ConfigurationRepository()
      {
         GetDataSource();
      }
      #endregion

      #region IConfiguration Properties
      public DataSourceType DataSource 
      {
         get
         {
            return _dataSourceElement.TypeOfDataSource;
         }
      }

      public String FileName
      {
         get
         {
            if (this.DataSource == DataSourceType.XMLFile)
            {
               return _dataSourceElement.Path;
            }
            else
            {
               return null;
            }
         }
      }

      public String ConnectString
      {
         get
         {
            if (this.DataSource == DataSourceType.SQLServerDatabase)
            {
               // TODO: The path will be the name of a connection string.
               return "Not Implemented";
            }
            else
            {
               return null;
            }
         }
      }
      #endregion

      #region Private Helpers
      private DataSourceElement _dataSourceElement;
      private void GetDataSource()
      {
         String sectionName = "dataSource";

         System.Configuration.Configuration configuration =
            ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.PerUserRoamingAndLocal );

         DataSourceSection section = configuration.GetSection( sectionName ) as DataSourceSection;
         if (section == null)
         {
            section = new DataSourceSection();
            section.CurrentDataSource = new DataSourceElement( DataSourceType.XMLFile, "Data.xml" );

            section.SectionInformation.AllowOverride = true;
            section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;

            configuration.Sections.Add( sectionName, section );
            configuration.Save( ConfigurationSaveMode.Modified );
            ConfigurationManager.RefreshSection( sectionName );
         }

         _dataSourceElement = section.CurrentDataSource;
      }
      #endregion
   }
}
