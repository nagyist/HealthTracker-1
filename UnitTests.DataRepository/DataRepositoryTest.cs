using System;
using System.Collections.ObjectModel;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Support;
using HealthTracker.Configuration.Interfaces;
using Moq;

namespace UnitTests.DataRepository
{
   /// <summary>
   /// Summary description for RepositoryTest
   /// </summary>
   [TestClass]
   public class DataRepositoryTest
   {
      #region Contructor Tests
      [TestMethod]
      public void RepositoryConstructAndLoadFullTest()
      {
         FoodGroup foodGroup;
         IDataRepository repository = new HealthTracker.DataRepository.Services.DataRepository( "../../../UnitTests.Support/Test Data/FullTestData.xml" );

         Assert.IsNotNull( repository );

         // Verify that the Food Groups loaded properly.  Spot check some of the actual data.
         Assert.AreEqual( 7, repository.GetAllFoodGroups().Count );
         foreach (FoodGroup group in repository.GetAllFoodGroups())
         {
            Assert.IsTrue( group.IsValid );
         }

         foodGroup = repository.GetAllFoodGroups()[0];
         Assert.AreEqual( new Guid( "269203a1-c066-4bca-a1ca-b80dc0cb72f3" ), foodGroup.ID );
         Assert.AreEqual( "Vegetable", foodGroup.Name );
         Assert.AreEqual( "Vegetables are good for you.  You should eat lots of them.", foodGroup.Description );

         foodGroup = repository.GetAllFoodGroups()[2];
         Assert.AreEqual( new Guid( "e024273b-a01e-4fb4-a535-e3dc632be5e4" ), foodGroup.ID );
         Assert.AreEqual( "Meat", foodGroup.Name );
         Assert.AreEqual( "Lean meat is generally the best.", foodGroup.Description );

         foodGroup = repository.GetAllFoodGroups()[5];
         Assert.AreEqual( new Guid( "57ee9b15-46e8-4be4-a22a-2cbd8e8f359b" ), foodGroup.ID );
         Assert.AreEqual( "Water", foodGroup.Name );
         Assert.IsNull( foodGroup.Description );


         // Verify the Food Items loaded properly.  Spot check some of the data.
         Assert.AreEqual( 5, repository.GetAllFoodItems().Count );
         foreach (FoodItem item in repository.GetAllFoodItems())
         {
            Assert.IsTrue( item.IsValid );
         }
         FoodItem foodItem = repository.GetAllFoodItems()[0];
         Assert.AreEqual( new Guid( "4082072e-d6fd-4522-9535-d2fd6528a6be" ), foodItem.ID );
         Assert.AreEqual( "Deluxe Bacon Cheese Burger", foodItem.Name );
         Assert.AreEqual( "Ground up cow, topped with curdled milk and salted pig fat.  Add lettuce, tomato, and onion for health.",
                          foodItem.Description );
         Assert.AreEqual( 650, foodItem.CaloriesPerServing );
         Assert.AreEqual( 4, foodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Meat", foodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 1.5M, foodItem.FoodGroupsPerServing[0].Quantity );
         Assert.AreEqual( "Vegetable", foodItem.FoodGroupsPerServing[3].Entity.Name );
         Assert.AreEqual( 0.5M, foodItem.FoodGroupsPerServing[3].Quantity );

         foodItem = repository.GetAllFoodItems()[2];
         Assert.AreEqual( new Guid( "46d04859-f95d-4ed6-9697-3e8a15c0bc91" ), foodItem.ID );
         Assert.AreEqual( "Glass of Skim Milk", foodItem.Name );
         Assert.IsNull( foodItem.Description );
         Assert.AreEqual( 90, foodItem.CaloriesPerServing );
         Assert.AreEqual( 1, foodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Dairy", foodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 2, foodItem.FoodGroupsPerServing[0].Quantity );

         foodItem = repository.GetAllFoodItems()[4];
         Assert.AreEqual( new Guid( "d29701af-a487-466d-b752-34a6ae7269cd" ), foodItem.ID );
         Assert.AreEqual( "Baby Carrots", foodItem.Name );
         Assert.IsNull( foodItem.Description );
         Assert.AreEqual( 40, foodItem.CaloriesPerServing );
         Assert.AreEqual( 1, foodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Vegetable", foodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 1, foodItem.FoodGroupsPerServing[0].Quantity );


         // Verify the Meal Types are loaded properly.  Limited data, so check it all.
         Assert.AreEqual( 4, repository.GetAllMealTypes().Count );
         foreach (MealType mealType in repository.GetAllMealTypes())
         {
            Assert.IsTrue( mealType.IsValid );
         }

         MealType testMealType = repository.GetAllMealTypes()[0];
         Assert.AreEqual( new Guid( "47ab9db8-83a3-41d8-bce7-81f42d45fbc9" ), testMealType.ID );
         Assert.AreEqual( "Breakfast", testMealType.Name );
         Assert.AreEqual( "The most important meal of the day.", testMealType.Description );

         testMealType = repository.GetAllMealTypes()[1];
         Assert.AreEqual( new Guid( "176b6819-8aae-4bf7-a3d9-83ad4d39ea2e" ), testMealType.ID );
         Assert.AreEqual( "Lunch", testMealType.Name );
         Assert.IsNull( testMealType.Description );

         testMealType = repository.GetAllMealTypes()[2];
         Assert.AreEqual( new Guid( "a832aa99-722a-4619-87f0-7214db172221" ), testMealType.ID );
         Assert.AreEqual( "Dinner", testMealType.Name );
         Assert.IsNull( testMealType.Description );

         testMealType = repository.GetAllMealTypes()[3];
         Assert.AreEqual( new Guid( "c466b59b-ccba-4929-97f9-b3bc274dea04" ), testMealType.ID );
         Assert.AreEqual( "Snack", testMealType.Name );
         Assert.AreEqual( "Limit these and make them healthy.", testMealType.Description );

         // Verify that the Meal Templates have loaded properly
         Assert.AreEqual( 2, repository.GetAllMealTemplates().Count );
         foreach (MealTemplate currentMealTemplate in repository.GetAllMealTemplates())
         {
            Assert.IsTrue( currentMealTemplate.IsValid );
         }

         MealTemplate mealTemplate = repository.GetAllMealTemplates()[0];
         Assert.AreEqual( "Cheeseburger Lunch", mealTemplate.Name );
         Assert.AreEqual( "A typical cheese burger based lunch.", mealTemplate.Description );
         Assert.AreEqual( "Lunch", mealTemplate.TypeOfMeal.Name );
         Assert.AreEqual( 1240, mealTemplate.Calories );
         Assert.AreEqual( DateTime.Parse( "2010-07-16T19:20-06:00" ), mealTemplate.DateAndTimeOfMeal );
         Assert.AreEqual( 3, mealTemplate.FoodItemServings.Count );
         Assert.AreEqual( 4, mealTemplate.FoodGroupServings.Count );

         Assert.AreEqual( 2.25M, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Meat" ).Quantity );
         Assert.AreEqual( 1.5M, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Grain" ).Quantity );
         Assert.AreEqual( 6.5M, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );
         Assert.AreEqual( 1.75M, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Vegetable" ).Quantity );

         mealTemplate = repository.GetAllMealTemplates()[1];
         Assert.AreEqual( "Fruity Breakfast", mealTemplate.Name );
         Assert.AreEqual( "Big fruit salad and a glass of milk", mealTemplate.Description );
         Assert.AreEqual( "Breakfast", mealTemplate.TypeOfMeal.Name );
         Assert.AreEqual( 315, mealTemplate.Calories );
         Assert.AreEqual( DateTime.Parse( "2010-07-16T19:20-06:00" ), mealTemplate.DateAndTimeOfMeal );
         Assert.AreEqual( 2, mealTemplate.FoodItemServings.Count );
         Assert.AreEqual( 2, mealTemplate.FoodGroupServings.Count );

         Assert.AreEqual( 3, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Fruit" ).Quantity );
         Assert.AreEqual( 2, mealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );

         // Verify that the meals are loaded properly
         Assert.AreEqual( 7, repository.GetAllMeals().Count );
         foreach (Meal currentMeal in repository.GetAllMeals())
         {
            Assert.IsTrue( currentMeal.IsValid );
         }

         var meal = repository.GetAllMeals()[3];
         Assert.AreEqual( "Breakfast #2", meal.Name );
         Assert.AreEqual( "Some Notes", meal.Description );
         Assert.AreEqual( "Breakfast", meal.TypeOfMeal.Name );
         Assert.AreEqual( 442.5M, meal.Calories );
         Assert.AreEqual( DateTime.Parse( "2010-07-17T19:20-06:00" ), meal.DateAndTimeOfMeal );
         Assert.AreEqual( 2, meal.FoodItemServings.Count );
         Assert.AreEqual( 2, meal.FoodGroupServings.Count );

         Assert.AreEqual( 3.5M, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Fruit" ).Quantity );
         Assert.AreEqual( 4, meal.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );
      }

      [TestMethod]
      public void RepositoryConstructAndLoadNoMealTypesAndTemplates()
      {
         IDataRepository repository = new HealthTracker.DataRepository.Services.DataRepository( "../../../UnitTests.Support/Test Data/NoMealTestFile.xml" );

         Assert.IsNotNull( repository );

         // Verify that the Food Groups loaded properly.  Spot check some of the actual data.
         Assert.AreEqual( 6, repository.GetAllFoodGroups().Count );
         foreach (FoodGroup foodGroup in repository.GetAllFoodGroups())
         {
            Assert.IsTrue( foodGroup.IsValid );
         }

         FoodGroup testFoodGroup = repository.GetAllFoodGroups()[0];
         Assert.AreEqual( new Guid( "269203a1-c066-4bca-a1ca-b80dc0cb72f3" ), testFoodGroup.ID );
         Assert.AreEqual( "Vegetable", testFoodGroup.Name );
         Assert.AreEqual( "Vegetables are good for you.  You should eat lots of them.",
                          testFoodGroup.Description );

         testFoodGroup = repository.GetAllFoodGroups()[2];
         Assert.AreEqual( new Guid( "e024273b-a01e-4fb4-a535-e3dc632be5e4" ), testFoodGroup.ID );
         Assert.AreEqual( "Meat", testFoodGroup.Name );
         Assert.AreEqual( "Lean meat is generally the best.", testFoodGroup.Description );

         testFoodGroup = repository.GetAllFoodGroups()[5];
         Assert.AreEqual( new Guid( "57ee9b15-46e8-4be4-a22a-2cbd8e8f359b" ), testFoodGroup.ID );
         Assert.AreEqual( "Water", testFoodGroup.Name );
         Assert.IsNull( testFoodGroup.Description );


         // Verify the Food Items loaded properly.  Spot check some of the data.
         Assert.AreEqual( 5, repository.GetAllFoodItems().Count );
         foreach (FoodItem foodItem in repository.GetAllFoodItems())
         {
            Assert.IsTrue( foodItem.IsValid );
         }

         FoodItem testFoodItem = repository.GetAllFoodItems()[0];
         Assert.AreEqual( new Guid( "4082072e-d6fd-4522-9535-d2fd6528a6be" ), testFoodItem.ID );
         Assert.AreEqual( "Deluxe Bacon Cheese Burger", testFoodItem.Name );
         Assert.AreEqual( "Ground up cow, topped with curdled milk and salted pig fat.  Add lettuce, tomato, and onion for health.",
                          testFoodItem.Description );
         Assert.AreEqual( 650, testFoodItem.CaloriesPerServing );
         Assert.AreEqual( 4, testFoodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Meat", testFoodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 1.5M, testFoodItem.FoodGroupsPerServing[0].Quantity );
         Assert.AreEqual( "Vegetable", testFoodItem.FoodGroupsPerServing[3].Entity.Name );
         Assert.AreEqual( 0.5M, testFoodItem.FoodGroupsPerServing[3].Quantity );

         testFoodItem = repository.GetAllFoodItems()[2];
         Assert.AreEqual( new Guid( "46d04859-f95d-4ed6-9697-3e8a15c0bc91" ), testFoodItem.ID );
         Assert.AreEqual( "Glass of Skim Milk", testFoodItem.Name );
         Assert.IsNull( testFoodItem.Description );
         Assert.AreEqual( 90, testFoodItem.CaloriesPerServing );
         Assert.AreEqual( 1, testFoodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Dairy", testFoodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 2, testFoodItem.FoodGroupsPerServing[0].Quantity );

         testFoodItem = repository.GetAllFoodItems()[4];
         Assert.AreEqual( new Guid( "d29701af-a487-466d-b752-34a6ae7269cd" ), testFoodItem.ID );
         Assert.AreEqual( "Baby Carrots", testFoodItem.Name );
         Assert.IsNull( testFoodItem.Description );
         Assert.AreEqual( 40, testFoodItem.CaloriesPerServing );
         Assert.AreEqual( 1, testFoodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Vegetable", testFoodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 1, testFoodItem.FoodGroupsPerServing[0].Quantity );


         // Verify the Meal Types list is allocated, but empty
         Assert.AreEqual( 0, repository.GetAllMealTypes().Count );

         // Verify that the Meal Templates is allocated but empty
         Assert.AreEqual( 0, repository.GetAllMealTemplates().Count );
      }

      [TestMethod]
      public void RepositoryConstructAndLoadNoFoodItems()
      {
         IDataRepository repository = new HealthTracker.DataRepository.Services.DataRepository( "../../../UnitTests.Support/Test Data/NoFoodItemsTestData.xml" );

         Assert.IsNotNull( repository );

         // Verify that the Food Groups loaded properly.  Spot check some of the actual data.
         Assert.AreEqual( 6, repository.GetAllFoodGroups().Count );
         foreach (FoodGroup foodGroup in repository.GetAllFoodGroups())
         {
            Assert.IsTrue( foodGroup.IsValid );
         }

         FoodGroup testFoodGroup = repository.GetAllFoodGroups()[0];
         Assert.AreEqual( new Guid( "269203a1-c066-4bca-a1ca-b80dc0cb72f3" ), testFoodGroup.ID );
         Assert.AreEqual( "Vegetable", testFoodGroup.Name );
         Assert.AreEqual( "Vegetables are good for you.  You should eat lots of them.", testFoodGroup.Description );

         testFoodGroup = repository.GetAllFoodGroups()[2];
         Assert.AreEqual( new Guid( "e024273b-a01e-4fb4-a535-e3dc632be5e4" ), testFoodGroup.ID );
         Assert.AreEqual( "Meat", testFoodGroup.Name );
         Assert.AreEqual( "Lean meat is generally the best.", testFoodGroup.Description );

         testFoodGroup = repository.GetAllFoodGroups()[5];
         Assert.AreEqual( new Guid( "57ee9b15-46e8-4be4-a22a-2cbd8e8f359b" ), testFoodGroup.ID );
         Assert.AreEqual( "Water", testFoodGroup.Name );
         Assert.IsNull( testFoodGroup.Description );


         // Verify the Food Items list is allocated but empty
         Assert.AreEqual( 0, repository.GetAllFoodItems().Count );

         // Verify the Meal Types are loaded properly.  Limited data, so check it all.
         Assert.AreEqual( 4, repository.GetAllMealTypes().Count );
         foreach (MealType mealType in repository.GetAllMealTypes())
         {
            Assert.IsTrue( mealType.IsValid );
         }

         MealType testMealType = repository.GetAllMealTypes()[0];
         Assert.AreEqual( new Guid( "47ab9db8-83a3-41d8-bce7-81f42d45fbc9" ), testMealType.ID );
         Assert.AreEqual( "Breakfast", testMealType.Name );
         Assert.AreEqual( "The most important meal of the day.", testMealType.Description );

         testMealType = repository.GetAllMealTypes()[1];
         Assert.AreEqual( new Guid( "176b6819-8aae-4bf7-a3d9-83ad4d39ea2e" ), testMealType.ID );
         Assert.AreEqual( "Lunch", testMealType.Name );
         Assert.IsNull( testMealType.Description );

         testMealType = repository.GetAllMealTypes()[2];
         Assert.AreEqual( new Guid( "a832aa99-722a-4619-87f0-7214db172221" ), testMealType.ID );
         Assert.AreEqual( "Dinner", testMealType.Name );
         Assert.IsNull( testMealType.Description );

         testMealType = repository.GetAllMealTypes()[3];
         Assert.AreEqual( new Guid( "c466b59b-ccba-4929-97f9-b3bc274dea04" ), testMealType.ID );
         Assert.AreEqual( "Snack", testMealType.Name );
         Assert.AreEqual( "Limit these and make them healthy.", testMealType.Description );

         // Verify that the Meal Templates list is allocated but empty
         Assert.AreEqual( 0, repository.GetAllMealTemplates().Count );
      }

      [TestMethod]
      public void RepositoryConstructAndLoadNoRootNode()
      {
         IDataRepository repository = new HealthTracker.DataRepository.Services.DataRepository( "../../../UnitTests.Support/Test Data/NoRootNodeTestData.xml" );

         Assert.IsNotNull( repository );

         Assert.AreEqual( 0, repository.GetAllFoodGroups().Count );
         Assert.AreEqual( 0, repository.GetAllFoodItems().Count );
         Assert.AreEqual( 0, repository.GetAllMealTemplates().Count );
         Assert.AreEqual( 0, repository.GetAllMealTypes().Count );
      }

      [TestMethod]
      public void RepositoryConstructAndLoadBadIDs()
      {
         IDataRepository repository = new HealthTracker.DataRepository.Services.DataRepository( "../../../UnitTests.Support/Test Data/MissingDataTestData.xml" );

         Assert.IsNotNull( repository );

         // Verify that the Food Groups loaded properly.  Actual data is checked elsewhere.
         Assert.AreEqual( 5, repository.GetAllFoodGroups().Count );
         foreach (FoodGroup foodGroup in repository.GetAllFoodGroups())
         {
            Assert.IsTrue( foodGroup.IsValid );
         }

         // Verify the Food Items loaded properly.  Spot check some of the data.
         Assert.AreEqual( 5, repository.GetAllFoodItems().Count );
         foreach (FoodItem foodItem in repository.GetAllFoodItems())
         {
            // Items that are associated with the missing food group (vegitables)
            // should be invalid.
            if (foodItem.Name == "Baby Carrots" || foodItem.Name == "Deluxe Bacon Cheese Burger")
            {
               Assert.IsFalse( foodItem.IsValid );
               Assert.AreEqual( Messages.Error_No_ServingItem, foodItem["FoodGroupsPerServing"] );
            }
            else
            {
               Assert.IsTrue( foodItem.IsValid );
            }
         }

         FoodItem testFoodItem = repository.GetAllFoodItems()[0];
         Assert.AreEqual( new Guid( "4082072e-d6fd-4522-9535-d2fd6528a6be" ), testFoodItem.ID );
         Assert.AreEqual( "Deluxe Bacon Cheese Burger", testFoodItem.Name );
         Assert.AreEqual( "Ground up cow, topped with curdled milk and salted pig fat.  Add lettuce, tomato, and onion for health.",
                          testFoodItem.Description );
         Assert.AreEqual( 650, testFoodItem.CaloriesPerServing );
         Assert.AreEqual( 4, testFoodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Meat", testFoodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 1.5M, testFoodItem.FoodGroupsPerServing[0].Quantity );
         Assert.IsNull( testFoodItem.FoodGroupsPerServing[3].Entity );
         Assert.AreEqual( 0.5M, testFoodItem.FoodGroupsPerServing[3].Quantity );

         testFoodItem = repository.GetAllFoodItems()[2];
         Assert.AreEqual( new Guid( "46d04859-f95d-4ed6-9697-3e8a15c0bc91" ), testFoodItem.ID );
         Assert.AreEqual( "Glass of Skim Milk", testFoodItem.Name );
         Assert.IsNull( testFoodItem.Description );
         Assert.AreEqual( 90, testFoodItem.CaloriesPerServing );
         Assert.AreEqual( 1, testFoodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( "Dairy", testFoodItem.FoodGroupsPerServing[0].Entity.Name );
         Assert.AreEqual( 2, testFoodItem.FoodGroupsPerServing[0].Quantity );

         testFoodItem = repository.GetAllFoodItems()[4];
         Assert.AreEqual( new Guid( "d29701af-a487-466d-b752-34a6ae7269cd" ), testFoodItem.ID );
         Assert.AreEqual( "Baby Carrots", testFoodItem.Name );
         Assert.IsNull( testFoodItem.Description );
         Assert.AreEqual( 40, testFoodItem.CaloriesPerServing );
         Assert.AreEqual( 1, testFoodItem.FoodGroupsPerServing.Count );
         Assert.IsNull( testFoodItem.FoodGroupsPerServing[0].Entity );
         Assert.AreEqual( 1, testFoodItem.FoodGroupsPerServing[0].Quantity );

         // Verify that the Meal Templates have loaded properly
         Assert.AreEqual( 1, repository.GetAllMealTemplates().Count );
         foreach (MealTemplate meal in repository.GetAllMealTemplates())
         {
            // In this case, the one and only currentMealTemplate we have set up should
            // not be valid.  It should not have a type
            Assert.IsFalse( meal.IsValid );
            Assert.AreEqual( Messages.Error_No_MealType, meal["TypeOfMeal"] );
         }

         MealTemplate testMealTemplate = repository.GetAllMealTemplates()[0];
         Assert.AreEqual( "Cheeseburger Lunch", testMealTemplate.Name );
         Assert.IsNull( testMealTemplate.Description );
         Assert.IsNull( testMealTemplate.TypeOfMeal );
         Assert.AreEqual( 1240, testMealTemplate.Calories );
         Assert.AreEqual( DateTime.Parse( "2010-07-16T19:20-06:00" ), testMealTemplate.DateAndTimeOfMeal );
         Assert.AreEqual( 3, testMealTemplate.FoodItemServings.Count );
         Assert.AreEqual( 4, testMealTemplate.FoodGroupServings.Count );

         Assert.AreEqual( 2.25M, testMealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Meat" ).Quantity );
         Assert.AreEqual( 1.5M, testMealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Grain" ).Quantity );
         Assert.AreEqual( 6.5M, testMealTemplate.FoodGroupServings.Find( fg => fg.Entity.Name == "Dairy" ).Quantity );
         Assert.AreEqual( 1.75M, testMealTemplate.FoodGroupServings.Find( fg => fg.Entity == null ).Quantity );
      }
      #endregion

      #region Food Group Tests
      [TestMethod]
      public void FindFoodGroup()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Try to find a food group that does not exist, should be null
         FoodGroup item = dataRepository.FindFoodGroup( fg => fg.Name == "Does Not Exist" );
         Assert.IsNull( item );

         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.FindFoodGroup( fg => fg.Name == "Meat" );
         Assert.AreEqual( "Meat", item.Name );
         FoodGroup sameItem = dataRepository.FindFoodGroup( fg => fg.Name == "Meat" );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Chicken";
         Assert.AreEqual( "Chicken", item.Name );
         Assert.AreEqual( "Meat", sameItem.Name );
      }

      [TestMethod]
      public void GetFoodGroup()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Request a food group that does not exist, should be null
         FoodGroup item = dataRepository.GetFoodGroup( Guid.NewGuid() );
         Assert.IsNull( item );

         // Request a food group that exists, show that a deep copy is returned
         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.GetFoodGroup( FullTestData.FruitID );
         Assert.AreEqual( "Fruit", item.Name );
         Assert.AreEqual( FullTestData.FruitID, item.ID );
         FoodGroup sameItem = dataRepository.GetFoodGroup( FullTestData.FruitID );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Juicy Sex Organs";
         Assert.AreEqual( "Juicy Sex Organs", item.Name );
         Assert.AreEqual( "Fruit", sameItem.Name );
      }

      [TestMethod]
      public void GetAllFoodGroups()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         ReadOnlyCollection<FoodGroup> allFoodGroups = dataRepository.GetAllFoodGroups();

         Assert.AreEqual( 7, allFoodGroups.Count );
      }

      [TestMethod]
      public void SaveFoodGroup()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Create one new food group, and get a deep copy of one out of the repository.
         // When the new one is saved, a food group should be added to the repository.
         // When the deep copy of the existing one is modified, the modifications should not be applied to the repository
         // until the copy is saved, at which a new food group should not be created, but the food group object with the same
         // ID should have the modifications applied to it.

         // TODO: This needs to change
         FoodGroup stuffing = new FoodGroup( new Guid( "5ff4f935-00b9-482b-a86a-3ffd53de9dda" ), "Stuffing", "The stuff that goes in a turkey" );
         FoodGroup repositoryMeat = dataRepository.FindFoodGroup( fg => fg.ID == FullTestData.MeatID );
         FoodGroup meat = new FoodGroup();
         meat.InitializeData( repositoryMeat );

         Assert.IsNotNull( meat );
         Assert.AreEqual( meat.ID, FullTestData.MeatID );
         Assert.AreEqual( meat.Name, "Meat" );
         Assert.AreEqual( meat.Description, "Lean meat is generally the best." );

         // Saving the one that already exists should result in the existing item being changed.
         Int32 origFoodGroupCount = dataRepository.GetAllFoodGroups().Count;
         meat.Name = "Meat and Poultry";
         meat.Description = "Eat more chicken.";
         repositoryMeat = dataRepository.FindFoodGroup( fg => fg.ID == FullTestData.MeatID );
         Assert.AreNotEqual( meat.Name, repositoryMeat.Name );
         Assert.AreNotEqual( meat.Description, repositoryMeat.Description );
         dataRepository.SaveItem( meat );
         repositoryMeat = dataRepository.FindFoodGroup( fg => fg.ID == FullTestData.MeatID );
         Assert.AreEqual( origFoodGroupCount, dataRepository.GetAllFoodGroups().Count );
         Assert.AreEqual( meat.Name, repositoryMeat.Name );
         Assert.AreEqual( meat.Description, repositoryMeat.Description );

         // Saving the one that does not exist, should result in the new item being added.
         dataRepository.SaveItem( stuffing );
         Assert.AreEqual( origFoodGroupCount + 1, dataRepository.GetAllFoodGroups().Count );
         FoodGroup repositoryStuffing = dataRepository.FindFoodGroup( fg => fg.ID == new Guid( "5ff4f935-00b9-482b-a86a-3ffd53de9dda" ) );
         Assert.AreEqual( stuffing.Name, repositoryStuffing.Name );
         Assert.AreEqual( stuffing.Description, repositoryStuffing.Description );
      }

      [TestMethod]
      public void DeleteFoodGroup()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Make a deep copy of a food group.
         FoodGroup meat = new FoodGroup();
         meat.InitializeData( dataRepository.FindFoodGroup( fg => fg.Name == "Meat" ) );

         // Delete the food group.  Show that the food group has been deleted, but none of the
         // other data repository items have not been effected.
         Int32 origFoodGroupCount = dataRepository.GetAllFoodGroups().Count;
         Int32 origFoodItemCount = dataRepository.GetAllFoodItems().Count;
         Int32 origMealTypeCount = dataRepository.GetAllMealTypes().Count;
         Int32 origMealTemplateCount = dataRepository.GetAllMealTemplates().Count;
         Int32 origMealCount = dataRepository.GetAllMeals().Count;
         dataRepository.Remove( meat );
         Assert.AreEqual( origFoodGroupCount - 1, dataRepository.GetAllFoodGroups().Count );
         Assert.AreEqual( origFoodItemCount, dataRepository.GetAllFoodItems().Count );
         Assert.AreEqual( origMealTypeCount, dataRepository.GetAllMealTypes().Count );
         Assert.AreEqual( origMealTemplateCount, dataRepository.GetAllMealTemplates().Count );
         Assert.AreEqual( origMealCount, dataRepository.GetAllMeals().Count );
         Assert.IsNotNull( meat.ID );
         Assert.IsNull( dataRepository.FindFoodGroup( f => f.ID == meat.ID ) );
      }
      #endregion

      #region FoodItem Tests
      [TestMethod]
      public void FindFoodItem()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Try to find a food group that does not exist, should be null
         FoodItem item = dataRepository.FindFoodItem( x => x.Name == "Does Not Exist" );
         Assert.IsNull( item );

         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.FindFoodItem( x => x.Name == "Fruit Salad" );
         Assert.AreEqual( "Fruit Salad", item.Name );
         FoodItem sameItem = dataRepository.FindFoodItem( x => x.Name == "Fruit Salad" );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Sex Organ Salad";
         Assert.AreEqual( "Sex Organ Salad", item.Name );
         Assert.AreEqual( "Fruit Salad", sameItem.Name );
      }

      [TestMethod]
      public void GetFoodItem()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Request a food group that does not exist, should be null
         FoodItem item = dataRepository.GetFoodItem( Guid.NewGuid() );
         Assert.IsNull( item );

         // Request a food group that exists, show that a deep copy is returned
         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         Assert.AreEqual( "Deluxe Bacon Cheese Burger", item.Name );
         Assert.AreEqual( FullTestData.CheeseBurgerID, item.ID );
         FoodItem sameItem = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Greasy and Bad for You";
         Assert.AreEqual( "Greasy and Bad for You", item.Name );
         Assert.AreEqual( "Deluxe Bacon Cheese Burger", sameItem.Name );
      }

      [TestMethod]
      public void GetAllFoodItems()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         ReadOnlyCollection<FoodItem> allFoodItems = dataRepository.GetAllFoodItems();

         Assert.AreEqual( 5, allFoodItems.Count );
      }

      [TestMethod]
      public void SaveFoodItem()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Get some food groups that we can use
         FoodGroup meat = dataRepository.FindFoodGroup( f => f.ID == FullTestData.MeatID );
         FoodGroup vegetables = dataRepository.FindFoodGroup( f => f.ID == FullTestData.VegetableID );

         // Create one new food item, and get a deep copy of one out of the repository.
         // When the new one is saved, a food item should be added to the repository.
         // When the deep copy of the existing one is modified, the modifications should not be applied to the repository
         // until the copy is saved, at which a new food item should not be created, but the food item object with the same
         // ID should have the modifications applied to it.
         FoodItem dublinCoddle = new FoodItem( new Guid( "f6ffa83c-5f7a-4961-8571-801f436c0eb9" ), "Dublin Coddle", "Poormans food", 250 );
         dublinCoddle.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1 ) );
         dublinCoddle.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetables, 2 ) );

         FoodItem repositoryChzBurger = dataRepository.FindFoodItem( f => f.ID == FullTestData.CheeseBurgerID );
         Assert.IsNotNull( repositoryChzBurger );
         FoodItem chzBurger = new FoodItem();
         chzBurger.InitializeData( repositoryChzBurger );
         Int32 origFoodItemCount = dataRepository.GetAllFoodItems().Count;

         // Verify the food groups, etc.
         Assert.AreEqual( repositoryChzBurger.CaloriesPerServing, chzBurger.CaloriesPerServing );
         Assert.AreEqual( repositoryChzBurger.Name, chzBurger.Name );
         Assert.AreEqual( repositoryChzBurger.Description, chzBurger.Description );
         Assert.AreEqual( repositoryChzBurger.FoodGroupsPerServing.Count, chzBurger.FoodGroupsPerServing.Count );

         // Remove the vegetables and bacon, and verify the difference
         chzBurger.Description = "Ground up cow, topped with curdled milk.";
         chzBurger.Name = "Cheese Burger";
         chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ).Quantity = 1;
         chzBurger.FoodGroupsPerServing.Remove( chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.VegetableID ) );
         chzBurger.CaloriesPerServing -= 140;

         repositoryChzBurger = dataRepository.FindFoodItem( f => f.ID == FullTestData.CheeseBurgerID );
         Assert.AreNotEqual( repositoryChzBurger.CaloriesPerServing, chzBurger.CaloriesPerServing );
         Assert.AreNotEqual( repositoryChzBurger.Name, chzBurger.Name );
         Assert.AreNotEqual( repositoryChzBurger.Description, chzBurger.Description );
         Assert.AreNotEqual( repositoryChzBurger.FoodGroupsPerServing.Count, chzBurger.FoodGroupsPerServing.Count );
         Assert.AreNotEqual(
            repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ),
            chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ) );
         Assert.IsNotNull( repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.VegetableID ) );
         Assert.IsNull( chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.VegetableID ) );

         // Save the cheese burger, and verify the changes are saved
         dataRepository.SaveItem( chzBurger );
         repositoryChzBurger = dataRepository.FindFoodItem( f => f.ID == FullTestData.CheeseBurgerID );
         Assert.AreEqual( repositoryChzBurger.CaloriesPerServing, chzBurger.CaloriesPerServing );
         Assert.AreEqual( repositoryChzBurger.Name, chzBurger.Name );
         Assert.AreEqual( repositoryChzBurger.Description, chzBurger.Description );
         Assert.AreEqual( repositoryChzBurger.FoodGroupsPerServing.Count, chzBurger.FoodGroupsPerServing.Count );
         Assert.AreEqual(
            repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ).Quantity,
            chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ).Quantity );
         Assert.IsNull( repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.VegetableID ) );
         Assert.IsNull( chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.VegetableID ) );

         Assert.AreEqual( origFoodItemCount, dataRepository.GetAllFoodItems().Count );

         // Add some fruit and verify the difference
         chzBurger.Name = "Hawian Cheese Burger";
         chzBurger.Description = "Ground up cow, topped with curdled milk, smoked pig, and pineapple.";
         chzBurger.FoodGroupsPerServing.Add( new Serving<FoodGroup>(
            dataRepository.FindFoodGroup( fg => fg.ID == FullTestData.FruitID ), 0.5M ) );
         chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ).Quantity += 1;
         chzBurger.CaloriesPerServing += 200;

         repositoryChzBurger = dataRepository.FindFoodItem( f => f.ID == FullTestData.CheeseBurgerID );
         Assert.AreNotEqual( repositoryChzBurger.CaloriesPerServing, chzBurger.CaloriesPerServing );
         Assert.AreNotEqual( repositoryChzBurger.Name, chzBurger.Name );
         Assert.AreNotEqual( repositoryChzBurger.Description, chzBurger.Description );
         Assert.AreNotEqual( repositoryChzBurger.FoodGroupsPerServing.Count, chzBurger.FoodGroupsPerServing.Count );
         Assert.AreNotEqual(
            repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ),
            chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ) );
         Assert.IsNull( repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.FruitID ) );
         Assert.IsNotNull( chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.FruitID ) );

         // Save the cheese burger, and verify the changes are saved
         dataRepository.SaveItem( chzBurger );
         repositoryChzBurger = dataRepository.FindFoodItem( f => f.ID == FullTestData.CheeseBurgerID );
         Assert.AreEqual( repositoryChzBurger.CaloriesPerServing, chzBurger.CaloriesPerServing );
         Assert.AreEqual( repositoryChzBurger.Name, chzBurger.Name );
         Assert.AreEqual( repositoryChzBurger.Description, chzBurger.Description );
         Assert.AreEqual( repositoryChzBurger.FoodGroupsPerServing.Count, chzBurger.FoodGroupsPerServing.Count );
         Assert.AreEqual(
            repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ).Quantity,
            chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.MeatID ).Quantity );
         Assert.IsNotNull( repositoryChzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.FruitID ) );
         Assert.IsNotNull( chzBurger.FoodGroupsPerServing.Find( f => f.Entity.ID == FullTestData.FruitID ) );

         Assert.AreEqual( origFoodItemCount, dataRepository.GetAllFoodItems().Count );

         // Save the new food item
         dataRepository.SaveItem( dublinCoddle );
         Assert.AreEqual( origFoodItemCount + 1, dataRepository.GetAllFoodItems().Count );
         FoodItem repositoryCoddle = dataRepository.FindFoodItem( f => f.ID == new Guid( "f6ffa83c-5f7a-4961-8571-801f436c0eb9" ) );
         Assert.IsNotNull( repositoryCoddle );
         Assert.AreEqual( "Dublin Coddle", repositoryCoddle.Name );
         Assert.AreEqual( "Poormans food", repositoryCoddle.Description );
         Assert.AreEqual( 250, repositoryCoddle.CaloriesPerServing );
         Assert.AreEqual( 2, repositoryCoddle.FoodGroupsPerServing.Count );
      }

      [TestMethod]
      public void DeleteFoodItem()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Make a deep copy of a food item.
         FoodItem fruitSalad = new FoodItem();
         fruitSalad.InitializeData( dataRepository.FindFoodItem( f => f.Name == "Fruit Salad" ) );

         // Delete the food item.  Show that the food item has been deleted, but none of the
         // other data repository items have not been effected.
         Int32 origFoodGroupCount = dataRepository.GetAllFoodGroups().Count;
         Int32 origFoodItemCount = dataRepository.GetAllFoodItems().Count;
         Int32 origMealTypeCount = dataRepository.GetAllMealTypes().Count;
         Int32 origMealTemplateCount = dataRepository.GetAllMealTemplates().Count;
         Int32 origMealCount = dataRepository.GetAllMeals().Count;
         dataRepository.Remove( fruitSalad );
         Assert.AreEqual( origFoodGroupCount, dataRepository.GetAllFoodGroups().Count );
         Assert.AreEqual( origFoodItemCount - 1, dataRepository.GetAllFoodItems().Count );
         Assert.AreEqual( origMealTypeCount, dataRepository.GetAllMealTypes().Count );
         Assert.AreEqual( origMealTemplateCount, dataRepository.GetAllMealTemplates().Count );
         Assert.AreEqual( origMealCount, dataRepository.GetAllMeals().Count );
         Assert.IsNotNull( fruitSalad.ID );
         Assert.IsNull( dataRepository.FindFoodItem( f => f.ID == fruitSalad.ID ) );
      }
      #endregion

      #region Meal Tests
      [TestMethod]
      public void FindMeal()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Try to find a currentMealTemplate that does not exist, should be null
         var item = dataRepository.FindMeal( x => x.Name == "Does Not Exist" );
         Assert.IsNull( item );

         // Find a currentMealTemplate that does exist, show that a deep copy is returned
         item = dataRepository.FindMeal( x => x.Name == "Breakfast #4" );
         Assert.AreEqual( "Breakfast #4", item.Name );
         var sameItem = dataRepository.FindMeal( x => x.Name == "Breakfast #4" );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found currentMealTemplate do not affect changes to what is logically
         // the same currentMealTemplate returned by the second find (IOW, show they are deep copies).
         item.Name = "tutti-frutti";
         Assert.AreEqual( "tutti-frutti", item.Name );
         Assert.AreEqual( "Breakfast #4", sameItem.Name );
      }

      [TestMethod]
      public void GetMeal()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Request a meal that does not exist, should be null
         var item = dataRepository.GetMeal( Guid.NewGuid() );
         Assert.IsNull( item );

         // Request a food group that exists, show that a deep copy is returned
         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.GetMeal( new Guid( "53edb94e-3b15-497e-9907-34de41c9bc8d" ) );
         Assert.AreEqual( "Lunch with Bob", item.Name );
         Assert.AreEqual( new Guid( "53edb94e-3b15-497e-9907-34de41c9bc8d" ), item.ID );
         var sameItem = dataRepository.GetMeal( new Guid( "53edb94e-3b15-497e-9907-34de41c9bc8d" ) );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Whatever, does not matter much";
         Assert.AreEqual( "Whatever, does not matter much", item.Name );
         Assert.AreEqual( "Lunch with Bob", sameItem.Name );
      }

      [TestMethod]
      public void GetAllMeals()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         var allMeals = dataRepository.GetAllMeals();

         Assert.AreEqual( 7, allMeals.Count );
      }

      [TestMethod]
      public void GetAllMealsForDate()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         var mealsForDay = dataRepository.GetAllMealsForDate( new DateTime( 2010, 7, 18 ) );
         Assert.AreEqual( 2, mealsForDay.Count );
         // TODO: Verify the meals

         mealsForDay = dataRepository.GetAllMealsForDate( new DateTime( 2010, 7, 19 ) );
         Assert.AreEqual( 1, mealsForDay.Count );
         // TODO: Verify the meals

         mealsForDay = dataRepository.GetAllMealsForDate( new DateTime( 2010, 7, 21 ) );
         Assert.AreEqual( 0, mealsForDay.Count );
      }

      [TestMethod]
      public void SaveMeal()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Get some food items that we can use for the new meal.
         FoodItem fruitSalad = dataRepository.FindFoodItem( f => f.ID == FullTestData.FruitSaladID );
         FoodItem glassOfMilk = dataRepository.FindFoodItem( f => f.ID == FullTestData.GlassOfMilkID );
         FoodItem glassOfWater = dataRepository.FindFoodItem( f => f.ID == FullTestData.GlassOfWaterID );

         MealType breakfast = dataRepository.FindMealType( f => f.ID == FullTestData.BreakfastID );

         // Create one new meal, and get a deep copy of one out of the repository.
         // When the new one is saved, a meal should be added to the repository.
         // When the deep copy of the existing one is modified, the modifications should not be applied to the repository
         // until the copy is saved, at which time a new meal should not be created, but the meal object with the same
         // ID should have the modifications applied to it.
         Guid breakfastMealID = new Guid( "12ed06f0-2251-44c3-bced-b656e6fbe558" );
         Guid lunchMealID = new Guid( "53edb94e-3b15-497e-9907-34de41c9bc8d" );
         Meal fruitBreakfast = new Meal( breakfastMealID, breakfast, DateTime.Now, "Fruit Breakfast", "Fruit Salad and some milk" );
         fruitBreakfast.FoodItemServings.Add( new Serving<FoodItem>( fruitSalad, 1 ) );
         fruitBreakfast.FoodItemServings.Add( new Serving<FoodItem>( glassOfMilk, 1 ) );
         Assert.IsTrue( fruitBreakfast.IsValid );

         Meal repositoryChzBurgerLunch = dataRepository.FindMeal( m => m.ID == lunchMealID );
         Assert.IsNotNull( repositoryChzBurgerLunch );
         Meal chzBurgerLunch = new Meal();
         chzBurgerLunch.InitializeData( repositoryChzBurgerLunch );
         Int32 origMealCount = dataRepository.GetAllMeals().Count;

         // Verifiy the copies are the same
         Assert.AreEqual( repositoryChzBurgerLunch.ID, chzBurgerLunch.ID );
         Assert.AreEqual( repositoryChzBurgerLunch.Name, chzBurgerLunch.Name );
         Assert.AreEqual( repositoryChzBurgerLunch.Description, chzBurgerLunch.Description );
         Assert.AreEqual( repositoryChzBurgerLunch.Calories, chzBurgerLunch.Calories );
         Assert.AreEqual( repositoryChzBurgerLunch.FoodItemServings.Count, chzBurgerLunch.FoodItemServings.Count );

         // Replace the glasses of milk with a glass of water, and only have one burger
         chzBurgerLunch.FoodItemServings.Remove( chzBurgerLunch.FoodItemServings.Find( f => f.Entity.ID == FullTestData.GlassOfMilkID ) );
         chzBurgerLunch.FoodItemServings.Add( new Serving<FoodItem>( glassOfWater, 1.5M ) );
         chzBurgerLunch.FoodItemServings.Find( f => f.Entity.ID == FullTestData.CheeseBurgerID ).Quantity = 1;
         chzBurgerLunch.Description = "A typical cheese burger lunch, made a bit healthier";

         repositoryChzBurgerLunch = dataRepository.FindMeal( m => m.ID == lunchMealID );
         Assert.AreEqual( repositoryChzBurgerLunch.ID, chzBurgerLunch.ID );
         Assert.AreEqual( repositoryChzBurgerLunch.Name, chzBurgerLunch.Name );
         Assert.AreNotEqual( repositoryChzBurgerLunch.Description, chzBurgerLunch.Description );
         Assert.AreNotEqual( repositoryChzBurgerLunch.Calories, chzBurgerLunch.Calories );

         // Save the changes and verify the changes were saved
         dataRepository.SaveItem( chzBurgerLunch );
         repositoryChzBurgerLunch = dataRepository.FindMeal( m => m.ID == lunchMealID );
         Assert.AreEqual( chzBurgerLunch.Name, repositoryChzBurgerLunch.Name );
         Assert.AreEqual( chzBurgerLunch.Description, repositoryChzBurgerLunch.Description );
         Assert.AreEqual( chzBurgerLunch.FoodItemServings.Count, repositoryChzBurgerLunch.FoodItemServings.Count );
         Assert.AreEqual( chzBurgerLunch.Calories, repositoryChzBurgerLunch.Calories );
         Assert.AreEqual( origMealCount, dataRepository.GetAllMeals().Count );

         // Save the breakfast
         dataRepository.SaveItem( fruitBreakfast );
         Assert.AreEqual( origMealCount + 1, dataRepository.GetAllMeals().Count );
         var repositoryFruitBreakfast = dataRepository.FindMeal( f => f.ID == breakfastMealID );
         Assert.IsNotNull( repositoryFruitBreakfast );
         Assert.AreEqual( fruitBreakfast.ID, repositoryFruitBreakfast.ID );
         Assert.AreEqual( fruitBreakfast.Name, repositoryFruitBreakfast.Name );
         Assert.AreEqual( fruitBreakfast.Description, repositoryFruitBreakfast.Description );
         Assert.AreEqual( fruitBreakfast.FoodItemServings.Count, repositoryFruitBreakfast.FoodItemServings.Count );
         Assert.AreEqual( fruitBreakfast.Calories, repositoryFruitBreakfast.Calories );
      }

      [TestMethod]
      public void DeleteMeal()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Make a deep copy of a meal
         var chzBurgerMealID = new Guid( "FEB4A1B2-796F-11E0-BB69-31B74724019B" );
         var chzBurgerMeal = new Meal();
         chzBurgerMeal.InitializeData( dataRepository.FindMeal( m => m.ID == chzBurgerMealID ) );
         Assert.IsNotNull( chzBurgerMeal );

         // Delete the meal.  Show that the meal has been deleted, but none of the
         // other data repository items have not been effected.
         Int32 origFoodGroupCount = dataRepository.GetAllFoodGroups().Count;
         Int32 origFoodItemCount = dataRepository.GetAllFoodItems().Count;
         Int32 origMealTypeCount = dataRepository.GetAllMealTypes().Count;
         Int32 origMealTemplateCount = dataRepository.GetAllMealTemplates().Count;
         Int32 origMealCount = dataRepository.GetAllMeals().Count;
         dataRepository.Remove( chzBurgerMeal );
         Assert.AreEqual( origFoodGroupCount, dataRepository.GetAllFoodGroups().Count );
         Assert.AreEqual( origFoodItemCount, dataRepository.GetAllFoodItems().Count );
         Assert.AreEqual( origMealTypeCount, dataRepository.GetAllMealTypes().Count );
         Assert.AreEqual( origMealTemplateCount, dataRepository.GetAllMealTemplates().Count );
         Assert.AreEqual( origMealCount - 1, dataRepository.GetAllMeals().Count );
         Assert.IsNotNull( chzBurgerMeal.ID );
         Assert.IsNull( dataRepository.FindMealTemplate( mt => mt.ID == chzBurgerMeal.ID ) );
      }
      #endregion

      #region MealTemplate Tests
      [TestMethod]
      public void FindMealTemplate()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Try to find a food group that does not exist, should be null
         MealTemplate item = dataRepository.FindMealTemplate( x => x.Name == "Does Not Exist" );
         Assert.IsNull( item );

         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.FindMealTemplate( x => x.Name == "Cheeseburger Lunch" );
         Assert.AreEqual( "Cheeseburger Lunch", item.Name );
         MealTemplate sameItem = dataRepository.FindMealTemplate( x => x.Name == "Cheeseburger Lunch" );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "My Guttbomb";
         Assert.AreEqual( "My Guttbomb", item.Name );
         Assert.AreEqual( "Cheeseburger Lunch", sameItem.Name );
      }

      [TestMethod]
      public void GetMealTemplate()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Request a food group that does not exist, should be null
         MealTemplate item = dataRepository.GetMealTemplate( Guid.NewGuid() );
         Assert.IsNull( item );

         // Request a food group that exists, show that a deep copy is returned
         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.GetMealTemplate( FullTestData.FruitSaladBreakfastID );
         Assert.AreEqual( "Fruity Breakfast", item.Name );
         Assert.AreEqual( FullTestData.FruitSaladBreakfastID, item.ID );
         MealTemplate sameItem = dataRepository.GetMealTemplate( FullTestData.FruitSaladBreakfastID );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Something healthy, for a change";
         Assert.AreEqual( "Something healthy, for a change", item.Name );
         Assert.AreEqual( "Fruity Breakfast", sameItem.Name );
      }

      [TestMethod]
      public void GetAllMealTemplates()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         ReadOnlyCollection<MealTemplate> allMealTemplates = dataRepository.GetAllMealTemplates();

         Assert.AreEqual( 2, allMealTemplates.Count );
      }

      [TestMethod]
      public void SaveMealTemplate()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Get some food items that we can use for the new Meal Template.
         FoodItem fruitSalad = dataRepository.FindFoodItem( f => f.ID == FullTestData.FruitSaladID );
         FoodItem glassOfMilk = dataRepository.FindFoodItem( f => f.ID == FullTestData.GlassOfMilkID );
         FoodItem glassOfWater = dataRepository.FindFoodItem( f => f.ID == FullTestData.GlassOfWaterID );

         MealType breakfast = dataRepository.FindMealType( f => f.ID == FullTestData.BreakfastID );

         // Create one new Meal Template, and get a deep copy of one out of the repository.
         // When the new one is saved, a meal template should be added to the repository.
         // When the deep copy of the existing one is modified, the modifications should not be applied to the repository
         // until the copy is saved, at which a new meal template should not be created, but the meal template object with
         // the same ID should have the modifications applied to it.
         MealTemplate fruitBreakfast = new MealTemplate( new Guid( "63a29a24-d2cb-40b1-9826-b355943817ab" ),
            breakfast, DateTime.Now, "Fruit Breakfast", "Fruit Salad and some milk" );
         fruitBreakfast.FoodItemServings.Add( new Serving<FoodItem>( fruitSalad, 1 ) );
         fruitBreakfast.FoodItemServings.Add( new Serving<FoodItem>( glassOfMilk, 1 ) );
         Assert.IsTrue( fruitBreakfast.IsValid );

         MealTemplate repositoryChzBurgerLunch = dataRepository.FindMealTemplate( m => m.ID == FullTestData.CheeseBurgerLunchID );
         Assert.IsNotNull( repositoryChzBurgerLunch );
         MealTemplate chzBurgerLunch = new MealTemplate();
         chzBurgerLunch.InitializeData( repositoryChzBurgerLunch );
         Int32 origMealTemplateCount = dataRepository.GetAllMealTemplates().Count;

         // Verifiy the copies are the same
         Assert.AreEqual( repositoryChzBurgerLunch.ID, chzBurgerLunch.ID );
         Assert.AreEqual( repositoryChzBurgerLunch.Name, chzBurgerLunch.Name );
         Assert.AreEqual( repositoryChzBurgerLunch.Description, chzBurgerLunch.Description );
         Assert.AreEqual( repositoryChzBurgerLunch.Calories, chzBurgerLunch.Calories );
         Assert.AreEqual( repositoryChzBurgerLunch.FoodItemServings.Count, chzBurgerLunch.FoodItemServings.Count );

         // Replace the glasses of milk with a glass of water, and only have one burger
         chzBurgerLunch.FoodItemServings.Remove( chzBurgerLunch.FoodItemServings.Find( f => f.Entity.ID == FullTestData.GlassOfMilkID ) );
         chzBurgerLunch.FoodItemServings.Add( new Serving<FoodItem>( glassOfWater, 1.5M ) );
         chzBurgerLunch.FoodItemServings.Find( f => f.Entity.ID == FullTestData.CheeseBurgerID ).Quantity = 1;
         chzBurgerLunch.Description = "A typical cheese burger lunch, made a bit healthier";

         repositoryChzBurgerLunch = dataRepository.FindMealTemplate( m => m.ID == FullTestData.CheeseBurgerLunchID );
         Assert.AreEqual( repositoryChzBurgerLunch.ID, chzBurgerLunch.ID );
         Assert.AreEqual( repositoryChzBurgerLunch.Name, chzBurgerLunch.Name );
         Assert.AreNotEqual( repositoryChzBurgerLunch.Description, chzBurgerLunch.Description );
         Assert.AreNotEqual( repositoryChzBurgerLunch.Calories, chzBurgerLunch.Calories );

         // Save the changes and verify the changes were saved
         dataRepository.SaveItem( chzBurgerLunch );
         repositoryChzBurgerLunch = dataRepository.FindMealTemplate( m => m.ID == FullTestData.CheeseBurgerLunchID );
         Assert.AreEqual( chzBurgerLunch.Name, repositoryChzBurgerLunch.Name );
         Assert.AreEqual( chzBurgerLunch.Description, repositoryChzBurgerLunch.Description );
         Assert.AreEqual( chzBurgerLunch.FoodItemServings.Count, repositoryChzBurgerLunch.FoodItemServings.Count );
         Assert.AreEqual( chzBurgerLunch.Calories, repositoryChzBurgerLunch.Calories );
         Assert.AreEqual( origMealTemplateCount, dataRepository.GetAllMealTemplates().Count );

         // Save the breakfast
         dataRepository.SaveItem( fruitBreakfast );
         Assert.AreEqual( origMealTemplateCount + 1, dataRepository.GetAllMealTemplates().Count );
         MealTemplate repositoryFruitBreakfast = dataRepository.FindMealTemplate( f => f.ID == new Guid( "63a29a24-d2cb-40b1-9826-b355943817ab" ) );
         Assert.IsNotNull( repositoryFruitBreakfast );
         Assert.AreEqual( fruitBreakfast.ID, repositoryFruitBreakfast.ID );
         Assert.AreEqual( fruitBreakfast.Name, repositoryFruitBreakfast.Name );
         Assert.AreEqual( fruitBreakfast.Description, repositoryFruitBreakfast.Description );
         Assert.AreEqual( fruitBreakfast.FoodItemServings.Count, repositoryFruitBreakfast.FoodItemServings.Count );
         Assert.AreEqual( fruitBreakfast.Calories, repositoryFruitBreakfast.Calories );
      }

      [TestMethod]
      public void DeleteMealTemplate()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Make a deep copy of a currentMealTemplate template
         MealTemplate chzBurger = new MealTemplate();
         chzBurger.InitializeData( dataRepository.FindMealTemplate( mt => mt.Name == "Cheeseburger Lunch" ) );
         Assert.IsNotNull( chzBurger );

         // Delete the currentMealTemplate template.  Show that the currentMealTemplate template has been deleted, but none of the
         // other data repository items have not been effected.
         Int32 origFoodGroupCount = dataRepository.GetAllFoodGroups().Count;
         Int32 origFoodItemCount = dataRepository.GetAllFoodItems().Count;
         Int32 origMealTypeCount = dataRepository.GetAllMealTypes().Count;
         Int32 origMealTemplateCount = dataRepository.GetAllMealTemplates().Count;
         Int32 origMealCount = dataRepository.GetAllMeals().Count;
         dataRepository.Remove( chzBurger );
         Assert.AreEqual( origFoodGroupCount, dataRepository.GetAllFoodGroups().Count );
         Assert.AreEqual( origFoodItemCount, dataRepository.GetAllFoodItems().Count );
         Assert.AreEqual( origMealTypeCount, dataRepository.GetAllMealTypes().Count );
         Assert.AreEqual( origMealTemplateCount - 1, dataRepository.GetAllMealTemplates().Count );
         Assert.AreEqual( origMealCount, dataRepository.GetAllMeals().Count );
         Assert.IsNotNull( chzBurger.ID );
         Assert.IsNull( dataRepository.FindMealTemplate( mt => mt.ID == chzBurger.ID ) );
      }
      #endregion

      #region MealType Tests
      [TestMethod]
      public void FindMealType()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Try to find a food group that does not exist, should be null
         MealType item = dataRepository.FindMealType( x => x.Name == "Does Not Exist" );
         Assert.IsNull( item );

         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.FindMealType( x => x.Name == "Lunch" );
         Assert.AreEqual( "Lunch", item.Name );
         MealType sameItem = dataRepository.FindMealType( x => x.Name == "Lunch" );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "Mid-Day Meal";
         Assert.AreEqual( "Mid-Day Meal", item.Name );
         Assert.AreEqual( "Lunch", sameItem.Name );
      }

      [TestMethod]
      public void GetMealType()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Request a food group that does not exist, should be null
         MealType item = dataRepository.GetMealType( Guid.NewGuid() );
         Assert.IsNull( item );

         // Request a food group that exists, show that a deep copy is returned
         // Find a food group that does exist, show that a deep copy is returned
         item = dataRepository.GetMealType( FullTestData.BreakfastID );
         Assert.AreEqual( "Breakfast", item.Name );
         Assert.AreEqual( FullTestData.BreakfastID, item.ID );
         Assert.AreEqual( DateTime.Today.AddHours( 6.0 ).TimeOfDay, item.DefaultTimeOfMeal.TimeOfDay );
         Assert.AreEqual( true, item.UseDefaultMealTime );
         MealType sameItem = dataRepository.GetMealType( FullTestData.BreakfastID );
         Assert.AreEqual( item.ID, sameItem.ID );
         Assert.AreNotEqual( item, sameItem );  // Show deep copy

         // Verify that changes to the first found food group do not affect changes to what is logically
         // the same food group returned by the second find (IOW, show they are deep copies).
         item.Name = "First Meal of the Day";
         Assert.AreEqual( "First Meal of the Day", item.Name );
         Assert.AreEqual( "Breakfast", sameItem.Name );
      }

      [TestMethod]
      public void GetAllMealTypes()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         ReadOnlyCollection<MealType> allMealTypes = dataRepository.GetAllMealTypes();

         Assert.AreEqual( 4, allMealTypes.Count );
      }

      [TestMethod]
      public void SaveMealType()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         MealType forthMeal = new MealType( new Guid( "18bbf0b4-9f2e-4477-97c9-d2b9419fe57f" ), "Forth Meal", "Going to Taco Bell", DateTime.Now, false );

         MealType repositoryBreakfast = dataRepository.FindMealType( m => m.ID == FullTestData.BreakfastID );
         MealType breakfast = new MealType();
         breakfast.InitializeData( repositoryBreakfast );

         Int32 origMealTypeCount = dataRepository.GetAllMealTypes().Count;

         // Verfiy the breakfasts are the same
         Assert.AreEqual( breakfast.ID, repositoryBreakfast.ID );
         Assert.AreEqual( breakfast.Name, repositoryBreakfast.Name );
         Assert.AreEqual( breakfast.Description, repositoryBreakfast.Description );
         Assert.AreEqual( breakfast.DefaultTimeOfMeal, repositoryBreakfast.DefaultTimeOfMeal );
         Assert.AreEqual( breakfast.UseDefaultMealTime, repositoryBreakfast.UseDefaultMealTime );

         // Change the breakfast, verify the repository has not changed
         breakfast.Name = "Big Breakfast";
         breakfast.Description = "Bacon, Eggs, Pancakes, stuff like that";
         breakfast.DefaultTimeOfMeal = DateTime.Now.AddHours( 2.5 );
         breakfast.UseDefaultMealTime = false;

         repositoryBreakfast = dataRepository.FindMealType( m => m.ID == FullTestData.BreakfastID );
         Assert.AreEqual( breakfast.ID, repositoryBreakfast.ID );
         Assert.AreNotEqual( breakfast.Name, repositoryBreakfast.Name );
         Assert.AreNotEqual( breakfast.Description, repositoryBreakfast.Description );
         Assert.AreNotEqual( breakfast.DefaultTimeOfMeal, repositoryBreakfast.DefaultTimeOfMeal );
         Assert.AreNotEqual( breakfast.UseDefaultMealTime, repositoryBreakfast.UseDefaultMealTime );


         Assert.AreEqual( origMealTypeCount, dataRepository.GetAllMealTypes().Count );

         // Save the change, verify the repository item has been changed, and no new currentMealTemplate types have been added
         dataRepository.SaveItem( breakfast );
         repositoryBreakfast = dataRepository.FindMealType( m => m.ID == FullTestData.BreakfastID );
         Assert.AreEqual( breakfast.ID, repositoryBreakfast.ID );
         Assert.AreEqual( breakfast.Name, repositoryBreakfast.Name );
         Assert.AreEqual( breakfast.Description, repositoryBreakfast.Description );
         Assert.AreEqual( breakfast.DefaultTimeOfMeal, repositoryBreakfast.DefaultTimeOfMeal );
         Assert.AreEqual( breakfast.UseDefaultMealTime, repositoryBreakfast.UseDefaultMealTime );


         Assert.AreEqual( origMealTypeCount, dataRepository.GetAllMealTypes().Count );

         // Save the new Meal type
         dataRepository.SaveItem( forthMeal );
         MealType repositoryForthMeal = dataRepository.FindMealType( m => m.ID == forthMeal.ID );
         Assert.IsNotNull( repositoryForthMeal );
         Assert.AreNotEqual( forthMeal, repositoryForthMeal );
         Assert.AreEqual( forthMeal.ID, repositoryForthMeal.ID );
         Assert.AreEqual( forthMeal.Name, repositoryForthMeal.Name );
         Assert.AreEqual( forthMeal.Description, repositoryForthMeal.Description );
         Assert.AreEqual( forthMeal.DefaultTimeOfMeal, repositoryForthMeal.DefaultTimeOfMeal );
         Assert.AreEqual( forthMeal.UseDefaultMealTime, repositoryForthMeal.UseDefaultMealTime );

         Assert.AreEqual( origMealTypeCount + 1, dataRepository.GetAllMealTypes().Count );
      }

      [TestMethod]
      public void DeleteMealType()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         HealthTracker.DataRepository.Services.DataRepository dataRepository = new HealthTracker.DataRepository.Services.DataRepository( configurationMock.Object );

         // Make a deep copy of a currentMealTemplate type
         MealType lunch = new MealType();
         lunch.InitializeData( dataRepository.FindMealType( mt => mt.Name == "Lunch" ) );
         Assert.IsNotNull( lunch );

         // Delete the meal type.  Show that the meal type has been deleted, but none of the
         // other data repository items have not been effected.
         Int32 origFoodGroupCount = dataRepository.GetAllFoodGroups().Count;
         Int32 origFoodItemCount = dataRepository.GetAllFoodItems().Count;
         Int32 origMealTypeCount = dataRepository.GetAllMealTypes().Count;
         Int32 origMealTemplateCount = dataRepository.GetAllMealTemplates().Count;
         Int32 origMealCount = dataRepository.GetAllMeals().Count;
         dataRepository.Remove( lunch );
         Assert.AreEqual( origFoodGroupCount, dataRepository.GetAllFoodGroups().Count );
         Assert.AreEqual( origFoodItemCount, dataRepository.GetAllFoodItems().Count );
         Assert.AreEqual( origMealTypeCount - 1, dataRepository.GetAllMealTypes().Count );
         Assert.AreEqual( origMealTemplateCount, dataRepository.GetAllMealTemplates().Count );
         Assert.AreEqual( origMealCount, dataRepository.GetAllMeals().Count );
         Assert.IsNotNull( lunch.ID );
         Assert.IsNull( dataRepository.FindMealTemplate( mt => mt.ID == lunch.ID ) );
      }
      #endregion
   }
}
