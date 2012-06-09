using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Properties;
using System.Diagnostics;

namespace HealthTracker.DataRepository.Models
{
   /// <summary>
   /// A meal template is a meal that is used as a template to make other meals.
   /// 
   /// NOTE: At this time, there is no difference between a Meal and a MealTemplate,
   ///       and it might be worth looking into combining the classes, at which point
   ///       we may need a flag to let the repository know if something is a Meal or
   ///       a template.
   /// </summary>
   public class MealTemplate : MealBase
   {
      #region Constructors
      public MealTemplate()
         : this( Guid.NewGuid(), null, default( DateTime ), null, null )
      { }

      public MealTemplate( Guid id, MealType mealType, DateTime timeOfMeal, String name, String description )
         : base( id, mealType, timeOfMeal, name, description )
      { }

      public MealTemplate( MealTemplate mealTemplate )
         : base( mealTemplate )
      { }

      public MealTemplate( Meal meal )
         : base( meal )
      { }
      #endregion

      #region IDataErrorInfo Memebers
      public override String this[String PropertyName]
      {
         get
         {
            return base[PropertyName];
         }
      }
      #endregion
   }
}
