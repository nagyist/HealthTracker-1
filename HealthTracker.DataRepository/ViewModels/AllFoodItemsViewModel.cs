using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.DataRepository.ViewModels
{
   /// <summary>
   /// Represents a read-only container of all FoodGroup objects currently in the system.
   /// Used to represent non-modifiable lists of food groups, such as a list of values.
   /// </summary>
   public class AllFoodItemsViewModel : AllDataObjectViewModelBase
   {
      #region Constructors
      public AllFoodItemsViewModel( IDataRepository dataRepository )
      {
         _foodItems = new ObservableCollection<FoodItem>( dataRepository.GetAllFoodItems() );
         Items = new ReadOnlyObservableCollection<FoodItem>( _foodItems );

         _dataRepository = dataRepository;
         _dataRepository.ItemAdded += this.OnFoodItemAddedToRepository;
         _dataRepository.ItemDeleted += this.OnFoodItemRemovedFromRepository;
         _dataRepository.ItemModified += this.OnFoodItemModified;
      }
      #endregion

      #region Public Interface
      private ObservableCollection<FoodItem> _foodItems;
      public ReadOnlyObservableCollection<FoodItem> Items { get; private set; }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         _dataRepository.ItemAdded -= this.OnFoodItemAddedToRepository;
         _dataRepository.ItemDeleted -= this.OnFoodItemRemovedFromRepository;
         _dataRepository.ItemModified -= this.OnFoodItemModified;
      }
      #endregion

      #region Private Event Handlers
      private IDataRepository _dataRepository;

      private void OnFoodItemAddedToRepository( object sender, RepositoryObjectEventArgs e )
      {
         FoodItem foodItem = e.Item as FoodItem;
         if (foodItem != null)
         {
            _foodItems.Add( foodItem );
         }
      }

      private void OnFoodItemRemovedFromRepository( object sender, RepositoryObjectEventArgs e )
      {
         FoodItem foodItem = e.Item as FoodItem;
         if (foodItem != null)
         {
            _foodItems.Remove( _foodItems.ToList().Find( x => x.ID == foodItem.ID ) );
         }
      }

      private void OnFoodItemModified( object sender, RepositoryObjectEventArgs e )
      {
         FoodItem foodItem = e.Item as FoodItem;
         if (foodItem != null)
         {
            var index = _foodItems.IndexOf( _foodItems.ToList().Find( x => x.ID == foodItem.ID ) );
            _foodItems.RemoveAt( index );
            _foodItems.Insert( index, foodItem );
         }
      }
      #endregion
   }
}
