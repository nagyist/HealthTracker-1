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
   public class FoodGroupNodeViewModelTest
   {
      #region Constructors
      public FoodGroupNodeViewModelTest()
      { }
      #endregion

      #region Constructor Tests
      [TestMethod]
      public void FoodGroupNodeViewModelDefault()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodGroupNodeViewModel foodGroupNodeViewModel = new FoodGroupNodeViewModel( dataRepository, null );

         Assert.AreEqual( DisplayStrings.AdminFoodGroupsTitle, foodGroupNodeViewModel.Name );

         // If the counts are the same, and every child is in the repository, then the data should be fine.
         Assert.AreEqual( dataRepository.GetAllFoodGroups().Count, foodGroupNodeViewModel.Children.Count );
         foreach (ClickableTreeNodeViewModel node in foodGroupNodeViewModel.Children)
         {
            FoodGroup foodGroup = dataRepository.GetFoodGroup( (Guid)node.Parameter );
            Assert.IsNotNull( foodGroup );
            Assert.AreEqual( foodGroup.Name, node.Name );
         }
      }
      #endregion

      #region Event Handling Tests
      [TestMethod]
      public void FoodGroupAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodGroupNodeViewModel foodGroupNodeViewModel = new FoodGroupNodeViewModel( dataRepository, null );

         Int32 originalChildCount = foodGroupNodeViewModel.Children.Count;

         var newFoodGroup = new FoodGroup( Guid.NewGuid(), "New Food Group", "Some Description" );
         dataRepository.SaveItem( newFoodGroup );
         Assert.AreEqual( originalChildCount + 1, foodGroupNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in foodGroupNodeViewModel.Children)
         {
            FoodGroup foodGroup = dataRepository.GetFoodGroup( (Guid)node.Parameter );
            Assert.IsNotNull( foodGroup );
            Assert.AreEqual( foodGroup.Name, node.Name );
         }
      }

      [TestMethod]
      public void NonFoodGroupNotAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodGroupNodeViewModel foodGroupNodeViewModel = new FoodGroupNodeViewModel( dataRepository, null );

         Int32 originalChildCount = foodGroupNodeViewModel.Children.Count;

         var newMealType = new MealType( Guid.NewGuid(), "New Meal Type", "Some Description", DateTime.Now, false );
         dataRepository.SaveItem( newMealType );
         Assert.AreEqual( originalChildCount, foodGroupNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in foodGroupNodeViewModel.Children)
         {
            FoodGroup foodGroup = dataRepository.GetFoodGroup( (Guid)node.Parameter );
            Assert.IsNotNull( foodGroup );
            Assert.AreEqual( foodGroup.Name, node.Name );
         }
      }

      [TestMethod]
      public void FoodGroupNodeViewModelRemoveFoodGroup()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         FoodGroupNodeViewModel foodGroupNodeViewModel = new FoodGroupNodeViewModel( dataRepository, null );

         Int32 originalChildCount = foodGroupNodeViewModel.Children.Count;

         FoodGroup removedFoodGroup = dataRepository.GetAllFoodGroups().ElementAt( 0 );
         dataRepository.Remove( removedFoodGroup );
         Assert.AreEqual( originalChildCount - 1, foodGroupNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in foodGroupNodeViewModel.Children)
         {
            FoodGroup foodGroup = dataRepository.GetFoodGroup( (Guid)node.Parameter );
            Assert.IsNotNull( foodGroup );
            Assert.AreEqual( foodGroup.Name, node.Name );
         }
      }

      [TestMethod]
      public void FoodGroupNodeVewModelModifyFoodGroup()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         FoodGroupNodeViewModel foodGroupNodeViewModel = new FoodGroupNodeViewModel( dataRepository, null );
         Int32 originalChildCount = foodGroupNodeViewModel.Children.Count;

         FoodGroup foodGroup = dataRepository.GetAllFoodGroups().ElementAt( 0 );
         String originalName = foodGroup.Name;

         ClickableTreeNodeViewModel foodGroupInTree = null;
         foreach (ClickableTreeNodeViewModel child in foodGroupNodeViewModel.Children)
         {
            child.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
            if ((Guid)child.Parameter == foodGroup.ID)
            {
               foodGroupInTree = child;
            }
         }

         Assert.IsNotNull( foodGroupInTree );
         Assert.AreEqual( originalName, foodGroupInTree.Name );

         foodGroup.Name += "Modified";
         dataRepository.SaveItem( foodGroup );

         Assert.AreEqual( originalName + "Modified", foodGroupInTree.Name );
         Assert.AreEqual( 1, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( foodGroupInTree, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
      }
      #endregion
   }
}
