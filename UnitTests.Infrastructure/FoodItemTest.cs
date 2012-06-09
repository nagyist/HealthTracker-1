using System;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class FoodItemTest
   {
      [TestMethod]
      public void FoodItemDefault()
      {
         FoodItem foodItem = new FoodItem();

         Assert.IsNotNull( foodItem.ID );
         Assert.IsNull( foodItem.Name );
         Assert.IsNull( foodItem.Description );
         Assert.AreEqual( 0, foodItem.CaloriesPerServing );
         Assert.AreEqual( 0, foodItem.FoodGroupsPerServing.Count );
         Assert.IsFalse( foodItem.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, foodItem["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );

         // Assign a name
         foodItem.Name = "Fred";
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );

         // Assign a food group
         FoodGroup foodGroup = new FoodGroup( Guid.NewGuid(), "Vegetable", "" );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroup, 1 ) );
         Assert.IsTrue( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.IsNull( foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );
      }

      [TestMethod]
      public void FoodItemNameCalories()
      {
         FoodItem foodItem = new FoodItem( Guid.NewGuid(), "Water", "", -1 );

         Assert.IsNotNull( foodItem.ID );
         Assert.AreEqual( "Water", foodItem.Name );
         Assert.IsNull( foodItem.Description );
         Assert.AreEqual( -1, foodItem.CaloriesPerServing );
         Assert.AreEqual( 0, foodItem.FoodGroupsPerServing.Count );
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItem["FoodGroupsPerServing"] );
         Assert.AreEqual( Messages.Error_Negative_Calories, foodItem["CaloriesPerServing"] );

         // Assign a food group
         FoodGroup foodGroup = new FoodGroup( Guid.NewGuid(), "Water", "" );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroup, 1 ) );
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.IsNull( foodItem["FoodGroupsPerServing"] );
         Assert.AreEqual( Messages.Error_Negative_Calories, foodItem["CaloriesPerServing"] );

         // Provide non-negative Calories
         foodItem.CaloriesPerServing = 0;
         Assert.IsTrue( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.IsNull( foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );
      }

      [TestMethod]
      public void FoodItemNameDescriptionCalories()
      {
         FoodItem foodItem = new FoodItem( Guid.NewGuid(), "Apple", "It fell on Newton's head.", 60 );

         Assert.IsNotNull( foodItem.ID );
         Assert.AreEqual( "Apple", foodItem.Name );
         Assert.AreEqual( "It fell on Newton's head.", foodItem.Description );
         Assert.AreEqual( 60, foodItem.CaloriesPerServing );
         Assert.AreEqual( 0, foodItem.FoodGroupsPerServing.Count );
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );

         // Assign a food group
         FoodGroup foodGroup = new FoodGroup( Guid.NewGuid(), "Fruit", "" );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroup, 1 ) );
         Assert.IsTrue( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.IsNull( foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );
      }

      [TestMethod]
      public void FoodItemIDNameDescriptionCalories()
      {
         FoodItem foodItem = new FoodItem( new Guid( "5326e353-fb06-41c6-a30e-e75a9f1efdc6" ), "Orange", "You're round and juicy and sweet!", 40 );

         Assert.AreEqual( new Guid( "5326e353-fb06-41c6-a30e-e75a9f1efdc6" ), foodItem.ID );
         Assert.AreEqual( "Orange", foodItem.Name );
         Assert.AreEqual( "You're round and juicy and sweet!", foodItem.Description );
         Assert.AreEqual( 40, foodItem.CaloriesPerServing );
         Assert.AreEqual( 0, foodItem.FoodGroupsPerServing.Count );
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );

         // Assign a food group
         FoodGroup foodGroup = new FoodGroup( Guid.NewGuid(), "Fruit", "" );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroup, 1 ) );
         Assert.IsTrue( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.IsNull( foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );
      }

      [TestMethod]
      public void FoodItemInvalidFoodGroup()
      {
         FoodItem foodItem = new FoodItem( Guid.NewGuid(), "Apple", "It fell on Newton's head.", 60 );

         Assert.IsNotNull( foodItem.ID );
         Assert.AreEqual( "Apple", foodItem.Name );
         Assert.AreEqual( "It fell on Newton's head.", foodItem.Description );
         Assert.AreEqual( 60, foodItem.CaloriesPerServing );
         Assert.AreEqual( 0, foodItem.FoodGroupsPerServing.Count );
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );

         // Assign a food group that is valid and one that is not valid
         FoodGroup foodGroup = new FoodGroup( Guid.NewGuid(), "Fruit", "" );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( foodGroup, 1 ) );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( null, 0 ) );
         Assert.IsFalse( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         // Should contain two messages seperated by double spaces.
         Assert.IsTrue( foodItem["FoodGroupsPerServing"].Contains( Messages.Error_No_Quantity ) );
         Assert.IsTrue( foodItem["FoodGroupsPerServing"].Contains( Messages.Error_No_ServingItem ) );
         Assert.AreEqual( Messages.Error_No_ServingItem.Length + Messages.Error_No_Quantity.Length + 2, foodItem["FoodGroupsPerServing"].Length );
         Assert.IsNull( foodItem["CaloriesPerServing"] );

         foodItem.FoodGroupsPerServing.RemoveAt( 1 );
         Assert.IsTrue( foodItem.IsValid );
         Assert.IsNull( foodItem["Name"] );
         Assert.IsNull( foodItem["FoodGroupsPerServing"] );
         Assert.IsNull( foodItem["CaloriesPerServing"] );
      }

      [TestMethod]
      public void InitializeData()
      {
         FoodGroup fruit = new FoodGroup( Guid.NewGuid(), "Fruit", "" );
         FoodGroup meat = new FoodGroup( Guid.NewGuid(), "Meat", "" );
         FoodGroup vegetable = new FoodGroup( Guid.NewGuid(), "Vegetable", "" );

         FoodItem sourceFoodItem = new FoodItem( Guid.NewGuid(), "Banana Burger", "Hamburger meat between banana slices", 235 );
         sourceFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( fruit, 2 ) );
         sourceFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1 ) );
         Assert.AreEqual( 2, sourceFoodItem.FoodGroupsPerServing.Count );

         FoodItem foodItem = new FoodItem();
         Assert.IsNotNull( foodItem.ID );
         Assert.AreNotEqual( sourceFoodItem.ID, foodItem.ID );
         Assert.IsNull( foodItem.Name );
         Assert.IsNull( foodItem.Description );
         Assert.AreEqual( 0, foodItem.FoodGroupsPerServing.Count );
         Assert.AreEqual( 0, foodItem.CaloriesPerServing );

         foodItem.InitializeData( sourceFoodItem );
         Assert.AreEqual( sourceFoodItem.ID, foodItem.ID );
         Assert.AreEqual( sourceFoodItem.Name, foodItem.Name );
         Assert.AreEqual( sourceFoodItem.Description, foodItem.Description );
         Assert.AreEqual( sourceFoodItem.FoodGroupsPerServing.Count, foodItem.FoodGroupsPerServing.Count );

         Serving<FoodGroup> serving = foodItem.FoodGroupsPerServing.Find( fg => fg.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( fruit, serving.Entity );
         Assert.AreEqual( 2, serving.Quantity );

         serving = foodItem.FoodGroupsPerServing.Find( fg => fg.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( meat, serving.Entity );
         Assert.AreEqual( 1, serving.Quantity );

         // Create a new source food item, and use it to init the target item
         sourceFoodItem = new FoodItem( Guid.NewGuid(), "Frog Soup", "Frogs, Aritichokes, and stuff", 250 );
         sourceFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( meat, 1 ) );
         sourceFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( vegetable, 2 ) );
         sourceFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( fruit, 0.5M ) );

         foodItem.InitializeData( sourceFoodItem );
         Assert.AreEqual( sourceFoodItem.ID, foodItem.ID );
         Assert.AreEqual( sourceFoodItem.Name, foodItem.Name );
         Assert.AreEqual( sourceFoodItem.Description, foodItem.Description );
         Assert.AreEqual( sourceFoodItem.FoodGroupsPerServing.Count, foodItem.FoodGroupsPerServing.Count );

         serving = foodItem.FoodGroupsPerServing.Find( fg => fg.Entity.Name == "Meat" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( meat, serving.Entity );
         Assert.AreEqual( 1, serving.Quantity );

         serving = foodItem.FoodGroupsPerServing.Find( fg => fg.Entity.Name == "Vegetable" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( vegetable, serving.Entity );
         Assert.AreEqual( 2, serving.Quantity );

         serving = foodItem.FoodGroupsPerServing.Find( fg => fg.Entity.Name == "Fruit" );
         Assert.IsNotNull( serving );
         Assert.AreEqual( fruit, serving.Entity );
         Assert.AreEqual( 0.5M, serving.Quantity );

         // We should also be able to take a Food Item and make something else out of it, such as a category
         FoodGroup category = new FoodGroup();
         category.InitializeData( sourceFoodItem );
         Assert.AreEqual( sourceFoodItem.ID, category.ID );
         Assert.AreEqual( sourceFoodItem.Name, category.Name );
         Assert.AreEqual( sourceFoodItem.Description, category.Description );
      }
   }
}
