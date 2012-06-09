using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using HealthTracker.Infrastructure.Properties;

namespace HealthTracker.DataRepository.Models
{
   public abstract class MealBase : DataObject
   {
      #region constructors
      /// <summary>
      /// Create an empty meal that only has a generated ID.
      /// </summary>
      public MealBase()
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
      public MealBase( Guid id, MealType typeOfMeal, DateTime timeOfMeal, String name, String description )
         : base( id, name, description )
      {
         TypeOfMeal = typeOfMeal;
         DateAndTimeOfMeal = timeOfMeal;

         FoodItemServings = new List<Serving<FoodItem>>();
      }

      /// <summary>
      /// Creates a new meal using the passed in meal as a model.
      /// </summary>
      /// <param name="meal">The meal to use as a model</param>
      public MealBase( MealBase meal )
         : this( meal.ID, meal.TypeOfMeal, meal.DateAndTimeOfMeal, meal.Name, meal.Description )
      {
         foreach (Serving<FoodItem> serving in meal.FoodItemServings)
         {
            this.FoodItemServings.Add( new Serving<FoodItem>( serving ) );
         }
      }
      #endregion

      #region Public Interface
      /// <summary>
      /// The type of meal that was eaten
      /// </summary>
      public MealType TypeOfMeal { get; set; }

      /// <summary>
      /// The date and time that the meal was consumed
      /// </summary>
      public DateTime DateAndTimeOfMeal { get; set; }

      /// <summary>
      /// A collection of food items that were included in the meal
      /// </summary>
      public List<Serving<FoodItem>> FoodItemServings { get; private set; }

      public Decimal Calories
      {
         get
         {
            return (from serving in this.FoodItemServings
                    select (serving.Quantity *
                            ((serving.Entity == null) ? 0 : serving.Entity.CaloriesPerServing))).Sum();
         }
      }

      public List<Serving<FoodGroup>> FoodGroupServings
      {
         get
         {
            // Step #1 - Create a full list of all food group servings
            //
            // The meal consists of a list of food items.
            // Each food item contains a list of food group servings.
            // Create a list of all food groups for all food items, adjusted for the
            // quantity of that food item in the meal.
            //
            // Example:
            //   A Kabob could have the following food groups:
            //      1 meat
            //      2 veggitable
            //   If the meal was 1.5 kabobs, we want:
            //      1.5 meat
            //      3   veggitable
            List<Serving<FoodGroup>> allFoodGroupServings = new List<Serving<FoodGroup>>();
            foreach (Serving<FoodItem> foodItemServing in FoodItemServings)
            {
               // It is possible to have a food item servinig without an entity
               // That serving is invalid for the period of time that it is not
               // associated with a food item entitiy, so ignore it
               if (foodItemServing.Entity != null)
               {
                  allFoodGroupServings.AddRange(
                     (from serving in foodItemServing.Entity.FoodGroupsPerServing
                      select new Serving<FoodGroup>( serving.Entity,
                         serving.Quantity * foodItemServing.Quantity )).ToList() );
               }
            }

            // Step #2 - Aggregate the list created above
            return
               (from foodGroupServing in allFoodGroupServings
                group foodGroupServing by foodGroupServing.Entity into fgs
                select new Serving<FoodGroup>( fgs.Key, fgs.Sum( q => q.Quantity ) )).ToList();
         }
      }

      public override void InitializeData( RepositoryObjectBase source )
      {
         // If Initializing something to itself, just bail.
         if (this == source)
         {
            return;
         }

         base.InitializeData( source );

         if (source is MealBase)
         {
            MealBase meal = source as MealBase;
            this.TypeOfMeal = meal.TypeOfMeal;
            this.DateAndTimeOfMeal = meal.DateAndTimeOfMeal;

            this.FoodItemServings.Clear();
            foreach (Serving<FoodItem> foodItemServing in meal.FoodItemServings)
            {
               this.FoodItemServings.Add( new Serving<FoodItem>( foodItemServing ) );
            }
         }
      }
      #endregion

      #region IDataErrorInfo Memebers
      public override String this[String PropertyName]
      {
         get
         {
            if (PropertyName == propertyNameFoodItemServings)
            {
               return ValidateFoodItemServings();
            }
            else if (PropertyName == propertyNameTypeOfMeal)
            {
               return ValidateTypeOfMeal();
            }

            return base[PropertyName];
         }
      }
      #endregion

      #region validations
      private const String propertyNameTypeOfMeal = "TypeOfMeal";
      private const String propertyNameFoodItemServings = "FoodItemServings";

      static readonly String[] ValidatedProperties = 
      {
         propertyNameTypeOfMeal,
         propertyNameFoodItemServings
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
                  case propertyNameTypeOfMeal:
                     error = ValidateTypeOfMeal();
                     break;

                  case propertyNameFoodItemServings:
                     error = ValidateFoodItemServings();
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

      private String ValidateTypeOfMeal()
      {
         if (TypeOfMeal == default( MealType ))
         {
            return Messages.Error_No_MealType;
         }
         return null;
      }
      
      private String ValidateFoodItemServings()
      {
         if (FoodItemServings.Count == 0)
         {
            return Messages.Error_No_FoodItems;
         }

         StringBuilder errorString = new StringBuilder();
         foreach (Serving<FoodItem> foodItemServing in FoodItemServings)
         {
            if (!foodItemServing.IsValid)
            {
               errorString.Append( ((errorString.Length == 0) ? "" : "  ") + foodItemServing.Error );
            }
         }
         return ((errorString.Length == 0) ? null : errorString.ToString());
      }
      #endregion
   }
}
