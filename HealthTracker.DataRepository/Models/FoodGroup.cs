using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthTracker.DataRepository.Models
{
   public class FoodGroup : Category
   {
      #region Constructors
      public FoodGroup() : base()
      {
      }

      public FoodGroup( Guid id, String name, String description )
         : base( id, name, description )
      {
      }

      public FoodGroup( FoodGroup item )
         : this( item.ID, item.Name, item.Description )
      { }
      #endregion
   }
}
