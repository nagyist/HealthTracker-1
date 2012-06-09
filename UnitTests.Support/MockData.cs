using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.DataRepository.Models;

namespace UnitTests.Support
{
   public class MockData
   {
      #region Object IDs
      public static readonly Guid milkID = Guid.NewGuid();
      public static readonly Guid ojID = Guid.NewGuid();
      public static readonly Guid friedEggID = Guid.NewGuid();
      public static readonly Guid hashBrownsID = Guid.NewGuid();
      public static readonly Guid baconID = Guid.NewGuid();
      public static readonly Guid cerealID = Guid.NewGuid();
      public static readonly Guid hamSandwichID = Guid.NewGuid();
      public static readonly Guid baconCheeseBurgerID = Guid.NewGuid();
      public static readonly Guid frenchFriesID = Guid.NewGuid();
      public static readonly Guid carrotSticksID = Guid.NewGuid();
      public static readonly Guid chickenDinnerID = Guid.NewGuid();
      public static readonly Guid steakDinnerID = Guid.NewGuid();

      public static readonly Guid fruitID = Guid.NewGuid();
      public static readonly Guid dairyID = Guid.NewGuid();
      public static readonly Guid meatID = Guid.NewGuid();
      public static readonly Guid vegetableID = Guid.NewGuid();
      public static readonly Guid starchID = Guid.NewGuid();

      public static readonly Guid breakfastID = Guid.NewGuid();
      public static readonly Guid lunchID = Guid.NewGuid();
      public static readonly Guid dinnerID = Guid.NewGuid();
      public static readonly Guid snackID = Guid.NewGuid();

      public static readonly Guid BigBreakfastID = Guid.NewGuid();
      public static readonly Guid SmallBreakfastID = Guid.NewGuid();
      public static readonly Guid CheeseburgerLunchID = Guid.NewGuid();

      public static readonly Guid DayOneBreakfastID = Guid.NewGuid();
      public static readonly Guid DayOneLunchID = Guid.NewGuid();
      public static readonly Guid DayOneDinnerID = Guid.NewGuid();
      public static readonly Guid DayTwoBreakfastID = Guid.NewGuid();
      public static readonly Guid DayTwoDinnerID = Guid.NewGuid();
      #endregion

      #region Object Collections
      private List<FoodGroup> _foodGroups;
      public List<FoodGroup> FoodGroups()
      {
         if (_foodGroups == null)
         {
            _foodGroups = new List<FoodGroup>();

            _foodGroups.Add( new FoodGroup( fruitID, "Fruit", "Sweet, Juicy plant sex organs" ) );
            _foodGroups.Add( new FoodGroup( dairyID, "Dairy", "Mamal Juice" ) );
            _foodGroups.Add( new FoodGroup( meatID, "Meat", "Flesh of dead animals" ) );
            _foodGroups.Add( new FoodGroup( vegetableID, "Vegetable", "Bean sprouts, and such" ) );
            _foodGroups.Add( new FoodGroup( starchID, "Starch", "Carbs and sugars" ) );
         }

         return _foodGroups;
      }

      private List<MealType> _mealTypes;
      public List<MealType> MealTypes()
      {
         if (_mealTypes == null)
         {
            _mealTypes = new List<MealType>();

            _mealTypes.Add( new MealType( breakfastID, "Breakfast", "First meal of the day", DateTime.Today.AddHours( 6 ), true ) );
            _mealTypes.Add( new MealType( lunchID, "Lunch", "Mid-day meal", DateTime.Today.AddHours( 12 ), true ) );
            _mealTypes.Add( new MealType( dinnerID, "Dinner", "Last meal of the day", DateTime.Today.AddHours( 18 ), true ) );
            _mealTypes.Add( new MealType( snackID, "Snack", "Snacks are good food", DateTime.Today.AddHours( 21 ), false ) );
         }

         return _mealTypes;
      }

      private List<FoodItem> _foods;
      public List<FoodItem> Foods()
      {
         if (_foods == null)
         {
            _foods = new List<FoodItem>();

            var fruit = FoodGroups().Find( fg => fg.ID == fruitID );
            var dairy = FoodGroups().Find( fg => fg.ID == dairyID );
            var meat = FoodGroups().Find( fg => fg.ID == meatID );
            var vegetable = FoodGroups().Find( fg => fg.ID == vegetableID );
            var starch = FoodGroups().Find( fg => fg.ID == starchID );

            var food = new FoodItem( milkID, "Milk", "Moo Juice", 90.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( dairy, 1.0M ) );
            _foods.Add( food );

            food = new FoodItem( ojID, "Orange Juice", "Fresh Squeazed, lots of pulp", 100.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( fruit, 1.0M ) );
            _foods.Add( food );

            // breakfast Foods
            food = new FoodItem( friedEggID, "Egg", "Chicken abortion, fried", 60.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1.0M ) );
            _foods.Add( food );

            food = new FoodItem( hashBrownsID, "Hash Browns", "Shredded tators", 125.5M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 1.0M ) );
            _foods.Add( food );

            food = new FoodItem( baconID, "Bacon", "Meat of the gods", 100.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1.0M ) );
            _foods.Add( food );

            food = new FoodItem( cerealID, "Cereal", "Shredded Mini-Wheats", 125.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 2.0M ) );
            _foods.Add( food );

            // lunch foods
            food = new FoodItem( hamSandwichID, "Ham Sandwich", "Dead cured pig between bread", 320.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1.0M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetable, 0.5M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( dairy, 0.5M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 2.0M ) );
            _foods.Add( food );

            food = new FoodItem( baconCheeseBurgerID, "Bacon Cheese Burger", "Burger of the gods", 600.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 2.0M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 1.0M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetable, 0.25M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( dairy, 0.5M ) );
            _foods.Add( food );

            food = new FoodItem( frenchFriesID, "French Fries", "Skinny deep fried tators", 200.0M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 1.5M ) );
            _foods.Add( food );

            food = new FoodItem( carrotSticksID, "carrotSticks", "Orange skinny poser FFs", 40M );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetable, 1.0M ) );
            _foods.Add( food );

            // Dinner foods - food items dones as full meals.
            food = new FoodItem( chickenDinnerID, "Chicken Dinner", "Chicken, spuds, and veggies", 700 );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1.0M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetable, 1.0M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 1.0M ) );
            _foods.Add( food );

            food = new FoodItem( steakDinnerID, "Steak Dinner", "Cow, spuds, and veggies", 1200 );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1.5M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetable, 1.0M ) );
            food.FoodGroupsPerServing.Add( new Serving<FoodGroup>( starch, 1.0M ) );
            _foods.Add( food );
         }

         return _foods;
      }

      private List<MealTemplate> _mealTemplates;
      public List<MealTemplate> MealTemplates()
      {
         var fruit = FoodGroups().Find( fg => fg.ID == fruitID );
         var dairy = FoodGroups().Find( fg => fg.ID == dairyID );
         var meat = FoodGroups().Find( fg => fg.ID == meatID );
         var vegetable = FoodGroups().Find( fg => fg.ID == vegetableID );
         var starch = FoodGroups().Find( fg => fg.ID == starchID );

         var breakfast = MealTypes().Find( mt => mt.ID == breakfastID );
         var lunch = MealTypes().Find( mt => mt.ID == lunchID );
         var dinner = MealTypes().Find( mt => mt.ID == dinnerID );

         // drinks
         var milk = Foods().Find( f => f.ID == milkID );
         var oj = Foods().Find( f => f.ID == ojID );
         var friedEgg = Foods().Find( f => f.ID == friedEggID );
         var hashBrowns = Foods().Find( f => f.ID == hashBrownsID );
         var bacon = Foods().Find( f => f.ID == baconID );
         var cereal = Foods().Find( f => f.ID == cerealID );
         var hamSandwich = Foods().Find( f => f.ID == hamSandwichID );
         var baconCheeseBurger = Foods().Find( f => f.ID == baconCheeseBurgerID );
         var frenchFries = Foods().Find( f => f.ID == frenchFriesID );
         var carrotSticks = Foods().Find( f => f.ID == carrotSticksID );
         var chickenDinner = Foods().Find( f => f.ID == chickenDinnerID );
         var steakDinner = Foods().Find( f => f.ID == steakDinnerID );

         if (_mealTemplates == null)
         {
            _mealTemplates = new List<MealTemplate>();

            // Big Breakfast                          Calories
            //    three eggs                             180.0
            //    6 strips of bacon (2 servings)         200.0
            //    hashbrowns                             125.5
            //    OJ                                     100.0
            //                                        --------
            //                                           605.5
            // Food Groups
            //    Meat         5.0
            //    Fruit        1.0
            //    Starch       1.0
            var mealTemplate = new MealTemplate( BigBreakfastID, breakfast, DateTime.Today.AddHours( 6 ), "Big Breakfast", "A Rather Large Breakfast" );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( friedEgg, 3.0M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( hashBrowns, 1.0M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( bacon, 2.0M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( oj, 1.0M ) );
            _mealTemplates.Add( mealTemplate );

            // Small Breakfast                        Calories
            //    Cereal                                 125.0
            //    Milk (half serving)                     45.0
            //    OJ                                     100.0
            //                                        --------
            //                                           270.0
            // Food Groups
            //    Dairy        0.5
            //    Fruit        1.0
            //    Starch       1.0
            mealTemplate = new MealTemplate( SmallBreakfastID, breakfast, DateTime.Today.AddHours( 6 ), "Small Breakfast", "A Rather Normal Breakfast" );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( cereal, 1.0M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( milk, 0.5M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( oj, 1.0M ) );
            _mealTemplates.Add( mealTemplate );

            // Cheeseburger Lunch                     Calories
            //    Bacon Cheese Burger                    600.0
            //    French Fries                           200.0
            //    Milk                                    90.0
            //                                        --------
            //                                           890.0
            // Food Groups
            //    Dairy        1.5
            //    Meat         2.0
            //    Starch       2.5
            //    Vegetable    0.25
            mealTemplate = new MealTemplate( CheeseburgerLunchID, lunch, DateTime.Today.AddHours( 12 ), "Greasy Lunch", "A Rather unhealthy lunch" );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( baconCheeseBurger, 1.0M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( frenchFries, 0.5M ) );
            mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( milk, 1.0M ) );
            _mealTemplates.Add( mealTemplate );
         }

         return _mealTemplates;
      }

      public const int DaysToAddForFutureMeals = 3;
      private List<Meal> _meals;
      public List<Meal> Meals()
      {
         var fruit = FoodGroups().Find( fg => fg.ID == fruitID );
         var dairy = FoodGroups().Find( fg => fg.ID == dairyID );
         var meat = FoodGroups().Find( fg => fg.ID == meatID );
         var vegetable = FoodGroups().Find( fg => fg.ID == vegetableID );
         var starch = FoodGroups().Find( fg => fg.ID == starchID );

         var breakfast = MealTypes().Find( mt => mt.ID == breakfastID );
         var lunch = MealTypes().Find( mt => mt.ID == lunchID );
         var dinner = MealTypes().Find( mt => mt.ID == dinnerID );

         // drinks
         var milk = Foods().Find( f => f.ID == milkID );
         var oj = Foods().Find( f => f.ID == ojID );
         var friedEgg = Foods().Find( f => f.ID == friedEggID );
         var hashBrowns = Foods().Find( f => f.ID == hashBrownsID );
         var bacon = Foods().Find( f => f.ID == baconID );
         var cereal = Foods().Find( f => f.ID == cerealID );
         var hamSandwich = Foods().Find( f => f.ID == hamSandwichID );
         var baconCheeseBurger = Foods().Find( f => f.ID == baconCheeseBurgerID );
         var frenchFries = Foods().Find( f => f.ID == frenchFriesID );
         var carrotSticks = Foods().Find( f => f.ID == carrotSticksID );
         var chickenDinner = Foods().Find( f => f.ID == chickenDinnerID );
         var steakDinner = Foods().Find( f => f.ID == steakDinnerID );

         if (_meals == null)
         {
            _meals = new List<Meal>();

            // Meals for Today
            //
            // Breakfast:
            // 2.5 glasses of Orange Juice
            //    Calories: 100 * 2.5 = 250
            //    Food Groups:
            //       Fruit: 2.5
            // 1.0 Cereal
            //    Calories: 125
            //    Food Groups:
            //      Starch: 2.0
            // 0.75 Milk
            //    Calories: 90 * 0.75 = 67.5
            //    Food Groups:
            //       Dairy: 0.75
            //
            // Lunch:
            // 1.0 Ham Sandwich
            //    Calories: 320
            //    Food Groups:
            //       Meat: 1.0
            //       Vegetable: 0.5
            //       Dairy: 0.5
            //       Starch: 2.0
            // 2.0 Carrot Sticks
            //    Calories: 2 * 40 = 80
            //    Food Groups:
            //       Vegetable: 2.0
            // 1.5 Glasses of Milk
            //    Calories: 1.5 * 90 = 135
            //    Food Groups:
            //       Dairy: 1.5
            //
            // Dinner:
            // 1.0 Steak Dinner
            //    Calories: 1200
            //    Food Groups:
            //       Meat: 1.5
            //       Vegetable: 1.0
            //       Starch: 1.0
            // 2.0 Glasses of Milk
            //    Calories: 2.0 * 90 = 180
            //    Food Groups:
            //       Dairy: 2.0
            //
            // Totals:
            //    Calories: 250 + 125 + 67.5 + 320 + 80 + 135 + 1200 + 180 = 2357.5
            //    Food Groups:
            //       Fruit: 2.5
            //       Starch: 2.0 + 2.0 + 1.0 = 5.0
            //       Dairy: 0.75 + 0.5 + 1.5 + 2.0 = 4.75
            //       Meat: 1.0 + 1.5 = 2.5
            //       Vegetable: 0.5 + 2.0 + 1.0 = 3.5
            var meal = new Meal( DayOneBreakfastID, breakfast, DateTime.Today.AddHours( 6.5 ), "Breakfast", "" );
            meal.FoodItemServings.Add( new Serving<FoodItem>( oj, 2.5M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( cereal, 1.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( milk, 0.75M ) );
            _meals.Add( meal );

            meal = new Meal( DayOneLunchID, lunch, DateTime.Today.AddHours( 12 ), "Lunch", "" );
            meal.FoodItemServings.Add( new Serving<FoodItem>( hamSandwich, 1.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( carrotSticks, 2.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( milk, 1.5M ) );
            _meals.Add( meal );

            meal = new Meal( DayOneDinnerID, dinner, DateTime.Today.AddHours( 17.75 ), "Dinner", "" );
            meal.FoodItemServings.Add( new Serving<FoodItem>( steakDinner, 1.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( milk, 2.0M ) );
            _meals.Add( meal );

            // Meals for DaysToAddForFutureMeals days into the future.
            //
            // Breakfast:
            // 2.0 Eggs
            //    Calories: 2 * 60.0 = 120.0
            //    Food Groups:
            //       Meat: 2.0
            // 1.0 Hash Browns
            //    Calories: 125.5
            //    Food Groups:
            //       Starch: 1.0
            // 1.0 Bacon
            //    Calories: 100
            //    Food Groups:
            //       Meat: 1.0
            // 2.0 oj
            //    Calories: 200
            //    Food Groups:
            //       Fruit: 2.0
            //
            // Lunch:
            //    None
            //
            // Dinner:
            // 1.0 Chicken Dinner
            //    Calories: 700
            //    Food Groups:
            //       Meat: 1.0
            //       Vegetable: 1.0
            //       Starch: 1.0
            // 2.0 Glasses of Milk
            //    Calories: 2.0 * 90 = 180
            //    Food Groups:
            //       Dairy: 2.0
            //
            // Totals:
            //    Calories: 700 + 180 + 120 + 125.5 + 100 + 200 = 1425.5
            //    Food Groups:
            //       Meat: 1.0 + 1.0 + 1.0 = 3.0
            //       Vegetable: 1.0
            //       Starch: 1.0 + 1.0 = 2.0
            //       Dairy: 2.0
            //       Fruit: 2.0
            meal = new Meal( DayTwoBreakfastID, breakfast, DateTime.Today.AddDays( DaysToAddForFutureMeals ).AddHours( 6 ), "Breakfast", "" );
            meal.FoodItemServings.Add( new Serving<FoodItem>( friedEgg, 2.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( hashBrowns, 1.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( bacon, 1.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( oj, 2.0M ) );
            _meals.Add( meal );

            meal = new Meal( DayTwoDinnerID, dinner, DateTime.Today.AddDays( DaysToAddForFutureMeals ).AddHours( 18 ), "Dinner", "" );
            meal.FoodItemServings.Add( new Serving<FoodItem>( chickenDinner, 1.0M ) );
            meal.FoodItemServings.Add( new Serving<FoodItem>( milk, 2.0M ) );
            _meals.Add( meal );
         }

         return _meals;
      }
      #endregion
   }
}
