using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;
using Microsoft.Practices.Prism.Modularity;

namespace HealthTracker.DataRepository.Interfaces
{
   /// <summary>
   /// Event arguments related to Food Groups in the data repository
   /// </summary>
   [Export]
   public class RepositoryObjectEventArgs : EventArgs
   {
      public RepositoryObjectEventArgs( RepositoryObjectBase item )
         : base()
      {
         this.Item = item;
      }

      /// <summary>
      /// The Repository Object associated with the event.
      /// </summary>
      public RepositoryObjectBase Item { get; private set; }
   }
}
