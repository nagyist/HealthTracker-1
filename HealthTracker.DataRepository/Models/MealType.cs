using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthTracker.DataRepository.Models
{
   public class MealType : Category
   {
      #region Private Utilities
      private void SetDefaultValues()
      {
         this.DefaultTimeOfMeal = DateTime.Now;
         this.UseDefaultMealTime = true;
      }
      #endregion

      #region Constructors
      public MealType()
         : base()
      {
         SetDefaultValues();
      }

      public MealType( Guid id, String name, String description, DateTime defaultMealTime, Boolean useDefaultTime )
         : base( id, name, description )
      {
         this.DefaultTimeOfMeal = defaultMealTime;
         this.UseDefaultMealTime = useDefaultTime;
      }

      public MealType( MealType mealType )
         : this( mealType.ID, mealType.Name, mealType.Description, mealType.DefaultTimeOfMeal, mealType.UseDefaultMealTime )
      { }
      #endregion

      #region Public Methods
      public override void InitializeData( RepositoryObjectBase source )
      {
         // If Initializing something to itself, just bail.
         if (this == source)
         {
            return;
         }

         var mealType = source as MealType;
         if (mealType != null)
         {
            this.UseDefaultMealTime = ((MealType)source).UseDefaultMealTime;
            this.DefaultTimeOfMeal = ((MealType)source).DefaultTimeOfMeal;
         }

         base.InitializeData( source );
      }
      #endregion

      #region Public Data
      /// <summary>
      /// Default time that a meal of this type will be eaten.
      /// </summary>
      public DateTime DefaultTimeOfMeal { get; set; }

      /// <summary>
      /// Specifies whether or not the default meal time should be used when creating meals.
      /// Having a seperate flag allows the user to turn this on or off without losing the
      /// currently set default meal time.
      /// </summary>
      public Boolean UseDefaultMealTime { get; set; }
      #endregion
   }
}
