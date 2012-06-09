using System;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Support;
using HealthTracker.Configuration.Interfaces;
using Moq;
using HealthTracker.DataRepository.Services;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class MealTemplateTest
   {
      #region Constructor
      public MealTemplateTest()
      { }
      #endregion

      #region Unit Tests
      [TestMethod]
      public void MealTempalateDefault()
      {
         // Create a mealTemplate template.  It should be invalid and empty with the exception of an ID.
         // Other tests handle making a valid object, so ignore that with this one.
         MealTemplate mealTemplate = new MealTemplate();
         Assert.IsFalse( mealTemplate.IsValid );
         Assert.IsNotNull( mealTemplate.ID );
         Assert.IsNull( mealTemplate.TypeOfMeal );
         Assert.IsNull( mealTemplate.Name );
         Assert.IsNull( mealTemplate.Description );
         Assert.AreEqual( mealTemplate.DateAndTimeOfMeal, default( DateTime ) );
         Assert.AreEqual( 0, mealTemplate.FoodItemServings.Count );
         Assert.AreEqual( 0, mealTemplate.FoodGroupServings.Count );
         Assert.AreEqual( 0, mealTemplate.Calories );
         Assert.AreEqual( Messages.Error_No_MealType, mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, mealTemplate["FoodItemServings"] );
         Assert.AreEqual( Messages.Error_No_Name, mealTemplate["Name"] );
         Assert.IsNull( mealTemplate["Description"] );
      }

      [TestMethod]
      public void MealTemplateMeal()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         Meal meal = new Meal( Guid.NewGuid(), dataRepository.FindMealType( mt => mt.Name == "Lunch" ), theTime, "Lunch", null );
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         meal.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Deluxe Bacon Cheese Burger" ), 1 ) );
         Assert.IsTrue( meal.IsValid );
         Assert.AreEqual( 750, meal.Calories );

         MealTemplate mealTemplate = new MealTemplate( meal );
         Assert.AreEqual( meal.ID, mealTemplate.ID );
         Assert.AreEqual( meal.TypeOfMeal, mealTemplate.TypeOfMeal );
         Assert.AreEqual( meal.DateAndTimeOfMeal, mealTemplate.DateAndTimeOfMeal );
         Assert.AreEqual( 2, mealTemplate.FoodItemServings.Count );
         Assert.IsNotNull( mealTemplate.FoodItemServings.Find( s => s.Entity.Name == "Baby Carrots" ) );
         Assert.IsNotNull( mealTemplate.FoodItemServings.Find( s => s.Entity.Name == "Deluxe Bacon Cheese Burger" ) );
         Assert.IsTrue( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["Name"] );
      }


      [TestMethod]
      public void MealTemplateIDTypeTimeNameDescription()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         MealTemplate mealTemplate = new MealTemplate( new Guid( "1c456eca-17c6-4395-a14f-85800a5c9d35" ),
            dataRepository.FindMealType( mt => mt.Name == "Dinner" ),
            theTime, "Just a test mealTemplate", "And this is the description" );

         // Should be invalid when instantiated since there is a lot of missing data
         Assert.IsFalse( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, mealTemplate["FoodItemServings"] );
         Assert.IsNull( mealTemplate["Name"] );
         Assert.IsNull( mealTemplate["ID"] );
         Assert.AreEqual( new Guid( "1c456eca-17c6-4395-a14f-85800a5c9d35" ), mealTemplate.ID );
         Assert.AreEqual( "Dinner", mealTemplate.TypeOfMeal.Name );
         Assert.ReferenceEquals( dataRepository.FindMealType( mt => mt.Name == "Dinner" ), mealTemplate.TypeOfMeal );
         Assert.AreEqual( theTime, mealTemplate.DateAndTimeOfMeal );
         Assert.AreEqual( "Just a test mealTemplate", mealTemplate.Name );
         Assert.AreEqual( "And this is the description", mealTemplate.Description );
         Assert.AreEqual( 0, mealTemplate.FoodItemServings.Count );
         Assert.AreEqual( 0, mealTemplate.Calories );
         Assert.AreEqual( 0, mealTemplate.FoodGroupServings.Count );

         // Add missing data, show that it is now valid
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Deluxe Bacon Cheese Burger" ), 1 ) );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Glass of Skim Milk" ), 2 ) );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Fruit Salad" ), 1.5M ) );

         Assert.IsTrue( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.IsNull( mealTemplate["FoodItemServings"] );
         Assert.IsNull( mealTemplate["Name"] );
         Assert.AreEqual( (2.5M * 40) + 650 + (2 * 90) + (1.5M * 150), mealTemplate.Calories );
         Assert.AreEqual( 5, mealTemplate.FoodGroupServings.Count );
         Assert.AreEqual( 3, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Vegetable" ).Quantity );
         Assert.AreEqual( 1, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Grain" ).Quantity );
         Assert.AreEqual( 1.5M, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Meat" ).Quantity );
         Assert.AreEqual( 5, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );
         Assert.AreEqual( 3, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Fruit" ).Quantity );
      }

      [TestMethod]
      public void MealTemplateValidationTests()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         DateTime theTime = DateTime.Now;
         MealTemplate mealTemplate = new MealTemplate( Guid.NewGuid(), default( MealType ), default( DateTime ), null, null );

         Assert.IsFalse( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["ID"] );
         Assert.AreEqual( Messages.Error_No_MealType, mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, mealTemplate["FoodItemServings"] );
         Assert.AreEqual( Messages.Error_No_Name, mealTemplate["Name"] );

         // Add a valid mealTemplate type
         mealTemplate.TypeOfMeal = dataRepository.FindMealType( mt => mt.Name == "Lunch" );
         Assert.IsFalse( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["ID"] );
         Assert.IsNull( mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, mealTemplate["FoodItemServings"] );
         Assert.AreEqual( Messages.Error_No_Name, mealTemplate["Name"] );

         // Add a valid time for the mealTemplate
         mealTemplate.DateAndTimeOfMeal = theTime;
         Assert.IsFalse( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["ID"] );
         Assert.IsNull( mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, mealTemplate["FoodItemServings"] );
         Assert.AreEqual( Messages.Error_No_Name, mealTemplate["Name"] );

         // Add a food item serving
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.FindFoodItem( food => food.Name == "Baby Carrots" ), 2.5M ) );
         Assert.IsFalse( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["ID"] );
         Assert.IsNull( mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.IsNull( mealTemplate["FoodItemServings"] );
         Assert.AreEqual( Messages.Error_No_Name, mealTemplate["Name"] );

         // Assign a valid name
         mealTemplate.Name = "A test mealTemplate template";
         Assert.IsTrue( mealTemplate.IsValid );
         Assert.IsNull( mealTemplate["ID"] );
         Assert.IsNull( mealTemplate["TypeOfMeal"] );
         Assert.IsNull( mealTemplate["TimeOfMeal"] );
         Assert.IsNull( mealTemplate["FoodItemServings"] );
         Assert.IsNull( mealTemplate["Name"] );
      }

      [TestMethod]
      public void MealTemplateName()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplate mealTemplate = new MealTemplate( Guid.NewGuid(), dataRepository.FindMealType( mt => mt.Name == "Dinner" ),
            DateTime.Now, "Just a test mealTemplate", "" );

         Assert.IsNull( mealTemplate["Name"] );

         // Use String w/ just whitespace
         mealTemplate.Name = "      ";
         Assert.AreEqual( Messages.Error_No_Name, mealTemplate["Name"] );
         Assert.IsNull( mealTemplate.Name );

         // Whitespace on front
         mealTemplate.Name = "   The Name";
         Assert.IsNull( mealTemplate["Name"] );
         Assert.AreEqual( "The Name", mealTemplate.Name );

         // Whitespace on end
         mealTemplate.Name = "The Name    ";
         Assert.IsNull( mealTemplate["Name"] );
         Assert.AreEqual( "The Name", mealTemplate.Name );

         // Whitespace on front and end
         mealTemplate.Name = "   The Name     ";
         Assert.IsNull( mealTemplate["Name"] );
         Assert.AreEqual( "The Name", mealTemplate.Name );

         // Only whitespace in middle
         mealTemplate.Name = "The Name";
         Assert.IsNull( mealTemplate["Name"] );
         Assert.AreEqual( "The Name", mealTemplate.Name );
      }

      [TestMethod]
      public void MealTemplateDescription()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplate mealTemplate = new MealTemplate( Guid.NewGuid(), dataRepository.FindMealType( mt => mt.Name == "Dinner" ),
            DateTime.Now, "Just a test mealTemplate","" );

         // Use String w/ just whitespace
         mealTemplate.Description = "      ";
         Assert.IsNull( mealTemplate.Description );

         // Whitespace on front
         mealTemplate.Description = "   The Description";
         Assert.AreEqual( "The Description", mealTemplate.Description );

         // Whitespace on end
         mealTemplate.Description = "The Description    ";
         Assert.AreEqual( "The Description", mealTemplate.Description );

         // Whitespace on front and end
         mealTemplate.Description = "   The Description     ";
         Assert.AreEqual( "The Description", mealTemplate.Description );

         // Only whitespace in middle
         mealTemplate.Description = "The Description";
         Assert.AreEqual( "The Description", mealTemplate.Description );
      }

      [TestMethod]
      public void InitializeData()
      {
         FoodGroup fruit = new FoodGroup( Guid.NewGuid(), "Fruit", "" );
         FoodGroup meat = new FoodGroup( Guid.NewGuid(), "Meat", "" );
         FoodGroup vegitable = new FoodGroup( Guid.NewGuid(), "Vegetable", "" );
         FoodGroup dairy = new FoodGroup( Guid.NewGuid(), "Dairy", "" );
         FoodGroup grain = new FoodGroup( Guid.NewGuid(), "Grain", "" );
         MealType lunch = new MealType( Guid.NewGuid(), "Lunch", "", DateTime.Now, false );
         MealType snack = new MealType( Guid.NewGuid(), "Snack", "", DateTime.Now, false );
         DateTime currentDateTime = DateTime.Now;

         FoodItem hamburger = new FoodItem( Guid.NewGuid(), "Hamburger", "", 300 );
         hamburger.FoodGroupsPerServing.Add( new Serving<FoodGroup>( grain, 2 ) );
         hamburger.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1 ) );

         FoodItem glassOfMilk = new FoodItem( Guid.NewGuid(), "Glass of Milk", "", 90 );
         glassOfMilk.FoodGroupsPerServing.Add( new Serving<FoodGroup>( dairy, 1 ) );

         FoodItem babyCarrots = new FoodItem( Guid.NewGuid(), "Baby Carrots", "", 40 );
         babyCarrots.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegitable, 1 ) );

         MealTemplate sourceMealTemplate = new MealTemplate( Guid.NewGuid(), lunch, currentDateTime, "Test Meal", "This is a test" );
         sourceMealTemplate.FoodItemServings.Add( new Serving<FoodItem>( hamburger, 1 ) );
         sourceMealTemplate.FoodItemServings.Add( new Serving<FoodItem>( glassOfMilk, 2 ) );
         sourceMealTemplate.FoodItemServings.Add( new Serving<FoodItem>( babyCarrots, 1.5M ) );

         MealTemplate mealTemplate = new MealTemplate();
         Assert.AreNotEqual( sourceMealTemplate.ID, mealTemplate.ID );

         mealTemplate.InitializeData( sourceMealTemplate );
         Assert.AreEqual( sourceMealTemplate.ID, mealTemplate.ID );
         Assert.AreEqual( sourceMealTemplate.Name, mealTemplate.Name );
         Assert.AreEqual( sourceMealTemplate.Description, mealTemplate.Description );
         Assert.AreEqual( sourceMealTemplate.Calories, mealTemplate.Calories );
         Assert.AreEqual( sourceMealTemplate.FoodGroupServings.Count, mealTemplate.FoodGroupServings.Count );
         Assert.AreEqual( sourceMealTemplate.FoodItemServings.Count, mealTemplate.FoodItemServings.Count );

         mealTemplate.FoodItemServings[0].Quantity += 1;
         Assert.AreNotEqual( sourceMealTemplate.Calories, mealTemplate.Calories );

         // Create a new mealTemplate template that is just baby carrots (kind of a snack)
         sourceMealTemplate = new MealTemplate( Guid.NewGuid(), snack, DateTime.Now, "Snack", "This is a snack" );
         sourceMealTemplate.FoodItemServings.Add( new Serving<FoodItem>( babyCarrots, 2.5M ) );

         mealTemplate.InitializeData( sourceMealTemplate );
         Assert.AreEqual( sourceMealTemplate.ID, mealTemplate.ID );
         Assert.AreEqual( sourceMealTemplate.Name, mealTemplate.Name );
         Assert.AreEqual( sourceMealTemplate.Description, mealTemplate.Description );
         Assert.AreEqual( sourceMealTemplate.Calories, mealTemplate.Calories );
         Assert.AreEqual( sourceMealTemplate.FoodGroupServings.Count, mealTemplate.FoodGroupServings.Count );
         Assert.AreEqual( sourceMealTemplate.FoodItemServings.Count, mealTemplate.FoodItemServings.Count );
      }
      #endregion
   }
}

