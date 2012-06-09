using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Administration.ViewModels;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;

namespace UnitTests.Administration
{
   [TestClass]
   public class FoodItemNodeViewModelTest
   {
      #region Constructors
      public FoodItemNodeViewModelTest()
      { }
      #endregion

      #region Constructor Tests
      [TestMethod]
      public void FoodItemNodeViewModelDefault()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodItemNodeViewModel foodItemNodeViewModel = new FoodItemNodeViewModel( dataRepository, null );

         Assert.AreEqual( DisplayStrings.AdminFoodItemsTitle, foodItemNodeViewModel.Name );

         // If the counts are the same, and every child is in the repository, then the data should be fine.
         Assert.AreEqual( dataRepository.GetAllFoodItems().Count, foodItemNodeViewModel.Children.Count );
         foreach (ClickableTreeNodeViewModel node in foodItemNodeViewModel.Children)
         {
            FoodItem foodItem = dataRepository.GetFoodItem( (Guid)node.Parameter );
            Assert.IsNotNull( foodItem );
            Assert.AreEqual( foodItem.Name, node.Name );
         }
      }
      #endregion

      #region Event Handling Tests
      [TestMethod]
      public void FoodItemAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodItemNodeViewModel foodItemNodeViewModel = new FoodItemNodeViewModel( dataRepository, null );

         Int32 originalChildCount = foodItemNodeViewModel.Children.Count;

         FoodItem newFoodItem = new FoodItem( Guid.NewGuid(), "New Food Item", "Some Description", 92 );
         newFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( dataRepository.GetAllFoodGroups()[0], 1 ) );
         dataRepository.SaveItem( newFoodItem );
         Assert.AreEqual( originalChildCount + 1, foodItemNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in foodItemNodeViewModel.Children)
         {
            FoodItem foodItem = dataRepository.GetFoodItem( (Guid)node.Parameter );
            Assert.IsNotNull( foodItem );
            Assert.AreEqual( foodItem.Name, node.Name );
         }
      }

      [TestMethod]
      public void NonFoodItemNotAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodItemNodeViewModel foodItemNodeViewModel = new FoodItemNodeViewModel( dataRepository, null );

         Int32 originalChildCount = foodItemNodeViewModel.Children.Count;

         var newMealType = new MealType( Guid.NewGuid(), "New Meal Type", "Some Description", DateTime.Now, false );
         Assert.IsTrue( newMealType.IsValid );
         dataRepository.SaveItem( newMealType );
         Assert.AreEqual( originalChildCount, foodItemNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in foodItemNodeViewModel.Children)
         {
            FoodItem foodItem = dataRepository.GetFoodItem( (Guid)node.Parameter );
            Assert.IsNotNull( foodItem );
            Assert.AreEqual( foodItem.Name, node.Name );
         }
      }

      [TestMethod]
      public void FoodItemNodeViewModelRemoveFoodItem()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodItemNodeViewModel foodItemNodeViewModel = new FoodItemNodeViewModel( dataRepository, null );

         Int32 originalChildCount = foodItemNodeViewModel.Children.Count;

         FoodItem removedFoodItem = dataRepository.GetAllFoodItems().ElementAt( 0 );
         dataRepository.Remove( removedFoodItem );
         Assert.AreEqual( originalChildCount - 1, foodItemNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in foodItemNodeViewModel.Children)
         {
            FoodItem foodItem = dataRepository.GetFoodItem( (Guid)node.Parameter );
            Assert.IsNotNull( foodItem );
            Assert.AreEqual( foodItem.Name, node.Name );
         }
      }

      [TestMethod]
      public void FoodItemNodeVewModelModifyFoodItem()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodItemNodeViewModel foodItemNodeViewModel = new FoodItemNodeViewModel( dataRepository, null );
         Int32 originalChildCount = foodItemNodeViewModel.Children.Count;

         FoodItem foodItem = dataRepository.GetAllFoodItems().ElementAt( 0 );
         String originalName = foodItem.Name;

         ClickableTreeNodeViewModel foodItemInTree = null;
         foreach (ClickableTreeNodeViewModel child in foodItemNodeViewModel.Children)
         {
            child.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
            if ((Guid)child.Parameter == foodItem.ID)
            {
               foodItemInTree = child;
            }
         }

         Assert.IsNotNull( foodItemInTree );
         Assert.AreEqual( originalName, foodItemInTree.Name );

         foodItem.Name += "Modified";
         dataRepository.SaveItem( foodItem );

         Assert.AreEqual( originalName + "Modified", foodItemInTree.Name );
         Assert.AreEqual( 1, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( foodItemInTree, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
      }
      #endregion
   }
}
