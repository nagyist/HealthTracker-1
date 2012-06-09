using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Properties;

namespace HealthTracker.DataRepository.Models
{
   /// <summary>
   /// A meal is a group of food items consumed at a specific time.
   /// 
   /// NOTE: At this time, there is no difference between a Meal and a MealTemplate,
   ///       and it might be worth looking into combining the classes, at which point
   ///       we may need a flag to let the repository know if something is a Meal or
   ///       a template.
   /// </summary>
   public class Meal : MealBase
   {
      #region constructors
      /// <summary>
      /// Create an empty meal that only has a generated ID.
      /// </summary>
      public Meal()
         : this( Guid.NewGuid(), null, default( DateTime ), null, null )
      { }

      /// <summary>
      /// Create a new meal using the specified ID, type, and time.  Use this constructor when the ID of the 
      /// meal is already known, such as when loading the meal from a file or database.  The FoodItemServings
      /// collection will be instantiated, but needs to be populated seperately.
      /// </summary>
      /// <param name="id">The ID for the meal</param>
      /// <param name="typeOfMeal">The type of meal</param>
      /// <param name="timeOfMeal">The time the meal was eaten</param>
      /// <param name="name">The name of the meal</param>
      /// <param name="description">The description of the meal</param>
      public Meal( Guid id, MealType typeOfMeal, DateTime timeOfMeal, String name, String description )
         : base( id, typeOfMeal, timeOfMeal, name, description )
      { }

      /// <summary>
      /// Creates a new meal using the passed in meal as a model.
      /// </summary>
      /// <param name="meal">The meal to use as a model</param>
      public Meal( Meal meal )
         : base( meal )
      { }
      #endregion

      #region IDataErrorInfo Memebers
      public override String this[String PropertyName]
      {
         get
         {
             if (PropertyName == propertyNameTimeOfMeal)
            {
               return ValidateTimeOfMeal();
            }

            return base[PropertyName];
         }
      }
      #endregion

      #region validations
      private const String propertyNameTimeOfMeal = "TimeOfMeal";

      static readonly String[] ValidatedProperties = 
      {
         propertyNameTimeOfMeal
      };

      /// <summary>
      /// Determine if the object is valid or not.
      /// </summary>
      /// <returns>true if the object is valid, false otherwise</returns>
      //
      public override Boolean IsValid
      {
         get
         {
            String error = null;

            foreach (String propertyName in ValidatedProperties)
            {
               switch (propertyName)
               {   
                  case propertyNameTimeOfMeal:
                     error = ValidateTimeOfMeal();
                     break;

                  default:
                     Debug.Fail( "Unexpected property: " + propertyName );
                     break;
               }

               if (error != null)
               {
                  return false;
               }
            }

            return base.IsValid;
         }
      }

      private String ValidateTimeOfMeal()
      {
         if (DateAndTimeOfMeal == default( DateTime ))
         {
            return Messages.Error_No_MealTime;
         }
         return null;
      }
      #endregion
   }
}
