using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthTracker.Configuration.Interfaces
{
   public interface IConfiguration
   {
      /// <summary>
      /// The type of Data Source that is currently being used
      /// </summary>
      DataSourceType DataSource { get; }

      /// <summary>
      /// If the DataSource is a file, this is the file name including the full path.
      /// </summary>
      String FileName { get; }

      /// <summary>
      /// If the DataSource is a database, this is the connection string that should be used.
      /// </summary>
      String ConnectString { get; }
   }
}
