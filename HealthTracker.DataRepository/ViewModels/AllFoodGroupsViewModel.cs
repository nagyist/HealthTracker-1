using System.Collections.ObjectModel;
using System.Linq;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.DataRepository.ViewModels
{
   /// <summary>
   /// Represents a read-only container of all FoodGroup objects currently in the system.
   /// Used to represent non-modifiable lists of food groups, such as a list of values.
   /// </summary>
   public class AllFoodGroupsViewModel : AllDataObjectViewModelBase
   {
      #region Constructors
      public AllFoodGroupsViewModel( IDataRepository dataRepository )
      {
         _foodGroups = new ObservableCollection<FoodGroup>( dataRepository.GetAllFoodGroups() );
         Items = new ReadOnlyObservableCollection<FoodGroup>( _foodGroups );

         _dataRepository = dataRepository;
         _dataRepository.ItemAdded += this.OnFoodGroupAdded;
         _dataRepository.ItemDeleted += this.OnFoodGroupRemoved;
         _dataRepository.ItemModified += this.OnFoodGroupModified;
      }
      #endregion

      #region Public Interface
      private ObservableCollection<FoodGroup> _foodGroups;
      public ReadOnlyObservableCollection<FoodGroup> Items { get; private set; }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         _dataRepository.ItemAdded -= this.OnFoodGroupAdded;
         _dataRepository.ItemDeleted -= this.OnFoodGroupRemoved;
         _dataRepository.ItemModified -= this.OnFoodGroupModified;
      }
      #endregion

      #region Private Event Handlers
      private IDataRepository _dataRepository;

      private void OnFoodGroupRemoved( object sender, RepositoryObjectEventArgs e )
      {
         FoodGroup foodGroup = e.Item as FoodGroup;
         if (foodGroup != null)
         {
            _foodGroups.Remove( _foodGroups.ToList().Find( x => x.ID == foodGroup.ID ) );
         }
      }

      private void OnFoodGroupAdded( object sender, RepositoryObjectEventArgs e )
      {
         FoodGroup foodGroup = e.Item as FoodGroup;
         if (foodGroup != null)
         {
            _foodGroups.Add( foodGroup );
         }
      }

      private void OnFoodGroupModified( object sender, RepositoryObjectEventArgs e )
      {
         FoodGroup foodGroup = e.Item as FoodGroup;
         if (foodGroup != null)
         {
            var index = _foodGroups.IndexOf( _foodGroups.ToList().Find( x => x.ID == foodGroup.ID ) );
            _foodGroups.RemoveAt( index );
            _foodGroups.Insert( index, foodGroup );
         }
      }
      #endregion
   }
}
