using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HealthTracker.DataRepository.Models
{
   /// <summary>
   /// A Category is just a way to categorize something.  There can be multiple types
   /// of categories in the system, but they will all be objects of this class (or
   /// a child of this class).
   /// </summary>
   public class Category : DataObject
   {
      #region Constructors
      public Category() : base()
      {
      }

      public Category( Guid id, String name, String description )
         : base( id, name, description )
      {
      }
      #endregion

      #region Public Interface
      public override void InitializeData( RepositoryObjectBase source )
      {
         // If Initializing something to itself, just bail.
         if (this == source)
         {
            return;
         }

         base.InitializeData( source );
      }
      #endregion
   }
}
