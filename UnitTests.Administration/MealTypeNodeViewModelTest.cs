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
   public class MealTypeNodeViewModelTest
   {
      #region Constructors
      public MealTypeNodeViewModelTest()
      { }
      #endregion

      #region Constructor Tests
      [TestMethod]
      public void MealTypeNodeViewModelDefault()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTypeNodeViewModel mealTypeNodeViewModel = new MealTypeNodeViewModel( dataRepository, null );

         Assert.AreEqual( DisplayStrings.AdminMealTypesTitle, mealTypeNodeViewModel.Name );

         // If the counts are the same, the parameter is set to the ID, and every child is in the repository,
         // then the data should be fine.
         Assert.AreEqual( dataRepository.GetAllMealTypes().Count, mealTypeNodeViewModel.Children.Count );
         foreach (ClickableTreeNodeViewModel node in mealTypeNodeViewModel.Children)
         {
            MealType mealType = dataRepository.GetMealType( (Guid)node.Parameter );
            Assert.IsNotNull( mealType );
            Assert.AreEqual( mealType.Name, node.Name );
         }
      }
      #endregion

      #region Event Handling Tests
      [TestMethod]
      public void MealTypeAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTypeNodeViewModel mealTypeNodeViewModel = new MealTypeNodeViewModel( dataRepository, null );

         Int32 originalChildCount = mealTypeNodeViewModel.Children.Count;

         MealType newMealType = new MealType( Guid.NewGuid(), "New Meal Type", "Some Description", DateTime.Now, false );
         dataRepository.SaveItem( newMealType );
         Assert.AreEqual( originalChildCount + 1, mealTypeNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealTypeNodeViewModel.Children)
         {
            MealType mealType = dataRepository.GetMealType( (Guid)node.Parameter );
            Assert.IsNotNull( mealType );
            Assert.AreEqual( mealType.Name, node.Name );
         }
      }

      [TestMethod]
      public void NonMealTypeNotAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTypeNodeViewModel mealTypeNodeViewModel = new MealTypeNodeViewModel( dataRepository, null );

         Int32 originalChildCount = mealTypeNodeViewModel.Children.Count;

         var newFoodGroup = new FoodGroup( Guid.NewGuid(), "New Food Group", "Some Description" );
         Assert.IsTrue( newFoodGroup.IsValid );
         dataRepository.SaveItem( newFoodGroup );
         Assert.AreEqual( originalChildCount, mealTypeNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealTypeNodeViewModel.Children)
         {
            MealType mealType = dataRepository.GetMealType( (Guid)node.Parameter );
            Assert.IsNotNull( mealType );
            Assert.AreEqual( mealType.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealTypeNodeViewModelRemoveMealType()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTypeNodeViewModel mealTypeNodeViewModel = new MealTypeNodeViewModel( dataRepository, null );

         Int32 originalChildCount = mealTypeNodeViewModel.Children.Count;

         MealType removedMealType = dataRepository.GetAllMealTypes().ElementAt( 0 );
         dataRepository.Remove( removedMealType );
         Assert.AreEqual( originalChildCount - 1, mealTypeNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealTypeNodeViewModel.Children)
         {
            MealType mealType = dataRepository.GetMealType( (Guid)node.Parameter );
            Assert.IsNotNull( mealType );
            Assert.AreEqual( mealType.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealTypeNodeVewModelModifyMealType()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         MealTypeNodeViewModel mealTypeNodeViewModel = new MealTypeNodeViewModel( dataRepository, null );
         Int32 originalChildCount = mealTypeNodeViewModel.Children.Count;

         MealType mealType = dataRepository.GetAllMealTypes().ElementAt( 0 );
         String originalName = mealType.Name;

         ClickableTreeNodeViewModel mealTypeInTree = null;
         foreach (ClickableTreeNodeViewModel child in mealTypeNodeViewModel.Children)
         {
            child.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
            if ((Guid)child.Parameter == mealType.ID)
            {
               mealTypeInTree = child;
            }
         }

         Assert.IsNotNull( mealTypeInTree );
         Assert.AreEqual( originalName, mealTypeInTree.Name );

         mealType.Name += "Modified";
         dataRepository.SaveItem( mealType );

         Assert.AreEqual( originalName + "Modified", mealTypeInTree.Name );
         Assert.AreEqual( 1, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( mealTypeInTree, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
      }
      #endregion
   }
}
