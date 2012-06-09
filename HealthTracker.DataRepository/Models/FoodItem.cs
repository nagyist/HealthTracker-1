using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Properties;

namespace HealthTracker.DataRepository.Models
{
   public class FoodItem : DataObject
   {
      #region Constructors
      public FoodItem()
         : this( Guid.NewGuid(), null, null, 0 )
      { }

      public FoodItem( Guid id, String name, String description, Decimal calories )
         : base( id, name, description )
      {
         this.Initialize( calories );
      }

      public FoodItem( FoodItem foodItem )
         : this()
      {
         this.InitializeData( foodItem );
      }

      private void Initialize( Decimal calories )
      {
         this.CaloriesPerServing = calories;
         this.FoodGroupsPerServing = new List<Serving<FoodGroup>>();
      }
      #endregion

      #region Public Interface
      public Decimal CaloriesPerServing { get; set; }
      public List<Serving<FoodGroup>> FoodGroupsPerServing { get; private set; }

      public override void InitializeData( RepositoryObjectBase source )
      {
         // If Initializing something to itself, just bail.
         if (this == source)
         {
            return;
         }

         base.InitializeData( source );

         if (source is FoodItem)
         {
            FoodItem foodItem = source as FoodItem;
            this.CaloriesPerServing = foodItem.CaloriesPerServing;
            this.FoodGroupsPerServing.Clear();
            foreach (Serving<FoodGroup> foodGroupServing in foodItem.FoodGroupsPerServing)
            {
               this.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroupServing ) );
            }
         }
      }
      #endregion

      #region IDataErrorInfo Memebers
      public override String this[String PropertyName]
      {
         get
         {
            if (PropertyName == propertyNameCaloriesPerServing)
            {
               return ValidateCatoriesPerServing();
            }
            else if (PropertyName == propertyNameFoodGroupsPerServing)
            {
               return ValidateFoodGroupsPerServing();
            }

            return base[PropertyName];
         }
      }
      #endregion

      #region Validations
      private const String propertyNameCaloriesPerServing = "CaloriesPerServing";
      private const String propertyNameFoodGroupsPerServing = "FoodGroupsPerServing";

      private static readonly String[] ValidatedProperties = 
      {
         propertyNameCaloriesPerServing,
         propertyNameFoodGroupsPerServing
      };

      /// <summary>
      /// Determine if the object is valid or not.
      /// </summary>
      /// <returns>true if the object is valid, false otherwise</returns>
      public override Boolean IsValid
      {
         get
         {
            String error = null;

            foreach (String propertyName in ValidatedProperties)
            {
               switch (propertyName)
               {
                  case propertyNameCaloriesPerServing:
                     error = ValidateCatoriesPerServing();
                     break;

                  case propertyNameFoodGroupsPerServing:
                     error = ValidateFoodGroupsPerServing();
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

      private String ValidateCatoriesPerServing()
      {
         if (CaloriesPerServing < 0)
         {
            return Messages.Error_Negative_Calories;
         }
         return null;
      }

      private String ValidateFoodGroupsPerServing()
      {
         if (FoodGroupsPerServing.Count == 0)
         {
            return Messages.Error_No_FoodGroups;
         }

         StringBuilder errorString = new StringBuilder();
         foreach (Serving<FoodGroup> foodGroupServing in FoodGroupsPerServing)
         {
            if (!foodGroupServing.IsValid)
            {
               errorString.Append( ((errorString.Length == 0) ? "" : "  ") + foodGroupServing.Error );
            }
         }

         return ((errorString.Length == 0) ? null : errorString.ToString());
      }
      #endregion
   }
}
