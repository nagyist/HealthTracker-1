using System;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class MealTest
   {
      #region Contructor
      public MealTest()
      { }
      #endregion

      #region Unit Tests
      [TestMethod]
      public void MealDefault()
      {
         // Create a mealTemplate.  It should be invalid and empty with the exception of an ID.
         // Other tests handle making a valid object, so ignore that with this one.
         Meal meal = new Meal();
         Assert.IsFalse( meal.IsValid );
         Assert.IsNotNull( meal.ID );
         Assert.IsNull( meal.TypeOfMeal );
         Assert.AreEqual( meal.DateAndTimeOfMeal, default( DateTime ) );
         Assert.AreEqual( 0, meal.FoodItemServings.Count );
         Assert.AreEqual( 0, meal.FoodGroupServings.Count );
         Assert.AreEqual( 0, meal.Calories );
         Assert.AreEqual( Messages.Error_No_MealType, meal["TypeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_MealTime, meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );
      }

      [TestMethod]
      public void MealIdTypeAndTime()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         Meal meal = new Meal(
            new Guid( "d678325f-6bd0-4fae-8847-ebb07323b9bc" ),
            dataRepository.FindMealType( mt => mt.Name == "Lunch" ),
            theTime, "Lunch", null );

         // Should be invalid when instantiated since there is a lot of missing data
         Assert.IsFalse( meal.IsValid );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.IsNull( meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );
         Assert.AreEqual( new Guid( "d678325f-6bd0-4fae-8847-ebb07323b9bc" ), meal.ID );
         Assert.AreEqual( "Lunch", meal.TypeOfMeal.Name );
         Assert.ReferenceEquals( dataRepository.FindMealType( mt => mt.Name == "Lunch" ), meal.TypeOfMeal );
         Assert.AreEqual( theTime, meal.DateAndTimeOfMeal );
         Assert.AreEqual( 0, meal.FoodItemServings.Count );
         Assert.AreEqual( 0, meal.Calories );
         Assert.AreEqual( 0, meal.FoodGroupServings.Count );

         // Add missing data, show that it is now valid
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Deluxe Bacon Cheese Burger" ), 1 ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Glass of Skim Milk" ), 2 ) );

         Assert.IsTrue( meal.IsValid );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.IsNull( meal["TimeOfMeal"] );
         Assert.IsNull( meal["FoodItemServings"] );
         Assert.AreEqual( (2.5M * 40) + 650 + (2 * 90), meal.Calories );
         Assert.AreEqual( 4, meal.FoodGroupServings.Count );
         Assert.AreEqual( 3, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Vegetable" ).Quantity );
         Assert.AreEqual( 1, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Grain" ).Quantity );
         Assert.AreEqual( 1.5M, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Meat" ).Quantity );
         Assert.AreEqual( 5, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );
      }

      [TestMethod]
      public void MealCopy()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         Meal mealModel = new Meal(
            new Guid( "d88a6942-663d-4f6f-831d-ea9127ccc0e9" ),
            dataRepository.FindMealType( mt => mt.Name == "Lunch" ),
            theTime, "Lunch", null );

         mealModel.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         mealModel.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Deluxe Bacon Cheese Burger" ), 1 ) );
         mealModel.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Glass of Skim Milk" ), 2 ) );

         Assert.IsTrue( mealModel.IsValid );

         Meal meal = new Meal( mealModel );
         Assert.IsNotNull( meal.ID );
         Assert.AreEqual( mealModel.ID, meal.ID );
         Assert.IsTrue( meal.IsValid );
         Assert.AreEqual( mealModel.TypeOfMeal, meal.TypeOfMeal );
         Assert.AreEqual( mealModel.DateAndTimeOfMeal, meal.DateAndTimeOfMeal );
         Assert.AreEqual( mealModel.Calories, meal.Calories );
         Assert.AreEqual( mealModel.FoodItemServings.Count, meal.FoodItemServings.Count );
         for (Int32 idx = 0; idx < mealModel.FoodItemServings.Count; idx++)
         {
            Assert.AreEqual( mealModel.FoodItemServings[idx].Quantity, meal.FoodItemServings[idx].Quantity );
            Assert.AreEqual( mealModel.FoodItemServings[idx].Entity, meal.FoodItemServings[idx].Entity );
         }
         Assert.AreEqual( mealModel.FoodGroupServings.Count, meal.FoodGroupServings.Count );
      }

      [TestMethod]
      public void MealInvalidFoodItemServings()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         Meal meal = new Meal( Guid.NewGuid(), dataRepository.FindMealType( mt => mt.Name == "Dinner" ), theTime, "Dinner", null );

         // Should be invalid when instantiated since there is a lot of missing data
         Assert.IsFalse( meal.IsValid );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.IsNull( meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );
         Assert.IsNotNull( meal.ID );
         Assert.AreEqual( "Dinner", meal.TypeOfMeal.Name );
         Assert.ReferenceEquals( dataRepository.FindMealType( mt => mt.Name == "Dinner" ), meal.TypeOfMeal );
         Assert.AreEqual( theTime, meal.DateAndTimeOfMeal );
         Assert.AreEqual( 0, meal.FoodItemServings.Count );
         Assert.AreEqual( 0, meal.Calories );
         Assert.AreEqual( 0, meal.FoodGroupServings.Count );

         // Add missing data, show that it is now valid
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Deluxe Bacon Cheese Burger" ), 1 ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Glass of Skim Milk" ), 2 ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( null, 1.5M ) );

         Assert.IsFalse( meal.IsValid );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.IsNull( meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_ServingItem, meal["FoodItemServings"] );
         Assert.AreEqual( (2.5M * 40) + 650 + (2 * 90), meal.Calories );
         Assert.AreEqual( 4, meal.FoodGroupServings.Count );
         Assert.AreEqual( 3, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Vegetable" ).Quantity );
         Assert.AreEqual( 1, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Grain" ).Quantity );
         Assert.AreEqual( 1.5M, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Meat" ).Quantity );
         Assert.AreEqual( 5, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );
      }

      [TestMethod]
      public void MealValidationTest()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         Meal meal = new Meal( Guid.NewGuid(), default( MealType ), default( DateTime ), null, null );

         Assert.IsFalse( meal.IsValid );
         Assert.IsNull( meal["ID"] );
         Assert.AreEqual( Messages.Error_No_Name, meal["Name"] );
         Assert.AreEqual( Messages.Error_No_MealType, meal["TypeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_MealTime, meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );

         // Add a name
         meal.Name = "A nice little lunch";
         Assert.IsNull( meal["ID"] );
         Assert.IsNull( meal["Name"] );
         Assert.AreEqual( Messages.Error_No_MealType, meal["TypeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_MealTime, meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );

         // Add a valid mealTemplate type
         meal.TypeOfMeal = dataRepository.FindMealType( mt => mt.Name == "Lunch" );
         Assert.IsFalse( meal.IsValid );
         Assert.IsNull( meal["ID"] );
         Assert.IsNull( meal["Name"] );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_MealTime, meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );

         // Add a valid time for the mealTemplate
         meal.DateAndTimeOfMeal = theTime;
         Assert.IsFalse( meal.IsValid );
         Assert.IsNull( meal["ID"] );
         Assert.IsNull( meal["Name"] );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.IsNull( meal["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, meal["FoodItemServings"] );

         // Add a food item serving
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         Assert.IsTrue( meal.IsValid );
         Assert.IsNull( meal["ID"] );
         Assert.IsNull( meal["Name"] );
         Assert.IsNull( meal["TypeOfMeal"] );
         Assert.IsNull( meal["TimeOfMeal"] );
         Assert.IsNull( meal["FoodItemServings"] );
      }
      #endregion
   }
}
