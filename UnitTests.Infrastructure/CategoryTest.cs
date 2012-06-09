using System;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Infrastructure
{
   [TestClass]
   public class CategoryTest
   {
      [TestMethod]
      public void CategoryDefault()
      {
         Category category = new Category();
         Assert.IsNotNull( category.ID, "ID should never be null" );
         Assert.IsNull( category.Name, "Name should be null" );
         Assert.IsNull( category.Description, "Description should be null" );
         Assert.IsFalse( category.IsValid, "Should be invalid" );
         Assert.AreEqual( category["Name"], Messages.Error_No_Name );
         Assert.IsNull( category["Description"], "No error message should exist" );
         Assert.IsNull( category["ID"], "No error message should exist" );
      }

      [TestMethod]
      public void CategoryDefaultAssignName()
      {
         Category category = new Category()
         {
            Name = "This is a test"
         };
         Assert.IsNotNull( category.ID, "ID should never be null" );
         Assert.AreEqual( category.Name, "This is a test", "Name should match" );
         Assert.IsNull( category.Description, "Description should be null" );
         Assert.IsTrue( category.IsValid, "Should be valid" );
         Assert.IsNull( category["Name"], "No error message should exist" );
         Assert.IsNull( category["Description"], "No error message should exist" );
         Assert.IsNull( category["ID"], "No error message should exist" );
      }

      [TestMethod]
      public void CategoryIDNameDescription()
      {
         Category category = new Category( new Guid( "3ba109a0-5450-426c-b065-70cc009f1763" ), "The Name", "The Description" );
         Assert.AreEqual( new Guid( "3ba109a0-5450-426c-b065-70cc009f1763" ), category.ID );
         Assert.AreEqual( "The Name", category.Name );
         Assert.AreEqual( "The Description", category.Description );
         Assert.IsTrue( category.IsValid, "Should be valid" );
         Assert.IsNull( category["Name"], "No error message should exist" );
         Assert.IsNull( category["Description"], "No error message should exist" );
         Assert.IsNull( category["ID"], "No error message should exist" );
      }

      [TestMethod]
      public void InitializeData()
      {
         Category sourceCategory = new Category( Guid.NewGuid(), "Source Name", "A description of the source category" );

         Category category = new Category();
         Assert.IsNotNull( category.ID );
         Assert.AreNotEqual( sourceCategory.ID, category.ID );
         Assert.IsNull( category.Name );
         Assert.IsNull( category.Description );

         category.InitializeData( sourceCategory );
         Assert.AreEqual( sourceCategory.ID, category.ID );
         Assert.AreEqual( sourceCategory.Name, category.Name );
         Assert.AreEqual( sourceCategory.Description, category.Description );

         sourceCategory = new Category( Guid.NewGuid(), "Another Name", "Yet another description" );
         category.InitializeData( sourceCategory );
         Assert.AreEqual( sourceCategory.ID, category.ID );
         Assert.AreEqual( sourceCategory.Name, category.Name );
         Assert.AreEqual( sourceCategory.Description, category.Description );
      }
   }
}
