using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using HealthTracker.Configuration.Interfaces;

namespace HealthTracker.Configuration.Models
{
   public class DataSourceElement : ConfigurationElement
   {
      #region Constructors
      public DataSourceElement()
      {
      }

      public DataSourceElement( DataSourceType dataSourceType, String path )
      {
         this.TypeOfDataSource = dataSourceType;
         this.Path = path;
      }
      #endregion

      #region Configuration Properties
      [ConfigurationProperty( "name", IsRequired = true, IsKey = true, DefaultValue = "My Health Data" )]
      public String Name
      {
         get
         {
            return this["name"] as String;
         }
         set
         {
            this["name"] = value;
         }
      }

      [ConfigurationProperty( "type", IsRequired = true, DefaultValue = DataSourceType.XMLFile )]
      public DataSourceType TypeOfDataSource
      {
         get
         {
            return (DataSourceType)this["type"];
         }
         set
         {
            this["type"] = value;
         }
      }

      [ConfigurationProperty( "path", IsRequired = true )]
      public String Path
      {
         get
         {
            return this["path"] as String;
         }
         set
         {
            this["path"] = value;
         }
      }
      #endregion
   }
}
