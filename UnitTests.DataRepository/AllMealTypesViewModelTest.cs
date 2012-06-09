using System;
using System.Linq;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.DataRepository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitTests.DataRepository
{
   [TestClass]
   public class AllMealTypesViewModelTest
   {
      #region Private Helpers
      private Mock<IDataRepository> CreateMockDataRepository( List<MealType> mealTypes )
      {
         var dataRespositoryMock = new Mock<IDataRepository>();
         dataRespositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( (from mealType in mealTypes
                                                         select new MealType( mealType )).ToList() ) );

         return dataRespositoryMock;
      }

      private void AssertViewModelContents( AllMealTypesViewModel viewModel, List<MealType> mealTypes )
      {
         Assert.AreEqual( mealTypes.Count, viewModel.Items.Count );

         foreach (var mealType in viewModel.Items)
         {
            var mealTypeFromRepository = mealTypes.Find( mt => mt.ID == mealType.ID );

            Assert.IsNotNull( mealType );
            Assert.AreEqual( mealTypeFromRepository.Name, mealType.Name );
         }
      }
      #endregion

      #region Tests
      [TestMethod]
      public void AllMealTypesViewModelContainsAllTypes()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTypes() );

         var viewModel = new AllMealTypesViewModel( dataRespositoryMock.Object );

         AssertViewModelContents( viewModel, data.MealTypes() );
         dataRespositoryMock.VerifyAll();
      }

      [TestMethod]
      public void MealTypeRemovedFromViewModelWhenRemovedFromRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTypes() );

         var viewModel = new AllMealTypesViewModel( dataRespositoryMock.Object );
         var mealType = data.MealTypes().Find( mt => mt.ID == MockData.lunchID );
         data.MealTypes().Remove( mealType );
         dataRespositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( mealType ) );

         AssertViewModelContents( viewModel, data.MealTypes() );
      }

      [TestMethod]
      public void MealTypeAddedToViewModelWhenAddedToRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTypes() );

         var viewModel = new AllMealTypesViewModel( dataRespositoryMock.Object );
         var mealType = new MealType( Guid.NewGuid(), "New Type", "For a unit test", DateTime.Now, false );
         data.MealTypes().Add( mealType );
         dataRespositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( mealType ) );

         AssertViewModelContents( viewModel, data.MealTypes() );
      }

      [TestMethod]
      public void MealTypeModifiedInViewModelWhenChangedInRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTypes() );
         var viewModel = new AllMealTypesViewModel( dataRespositoryMock.Object );

         var mealType = data.MealTypes().Find( mt => mt.ID == MockData.lunchID );
         mealType.Name += " Unit Test";
         dataRespositoryMock.Raise( e => e.ItemModified += null, new RepositoryObjectEventArgs( mealType ) );

         AssertViewModelContents( viewModel, data.MealTypes() );
      }
      #endregion
   }
}
