using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace HealthTracker.Configuration.Models
{
   public class DataSourceSection : ConfigurationSection
   {
      [ConfigurationProperty( "name", IsKey = true, IsRequired = true, DefaultValue = "Data Sources" )]
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

      [ConfigurationProperty( "current" )]
      public DataSourceElement CurrentDataSource
      {
         get
         {
            return (DataSourceElement)this["current"];
         }
         set
         {
            this["current"] = value;
         }
      }
   }
}
