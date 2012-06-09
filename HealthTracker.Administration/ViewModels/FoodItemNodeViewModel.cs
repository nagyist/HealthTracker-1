using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.Infrastructure.Properties;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.Administration.ViewModels
{
   public class FoodItemNodeViewModel : DataObjectCollectionNodeViewModel
   {
      #region Constructors
      public FoodItemNodeViewModel( IDataRepository dataRepository, ICommand childClickCommand )
         : base( DisplayStrings.AdminFoodItemsTitle, dataRepository, childClickCommand )
      {
         _children =
            new ObservableCollection<TreeNodeViewModel>(
            (from foodItem in dataRepository.GetAllFoodItems()
             select new ClickableTreeNodeViewModel( foodItem.Name, childClickCommand, foodItem.ID )).ToList() );
         Children = new ReadOnlyObservableCollection<TreeNodeViewModel>( _children );
      }
      #endregion

      #region Event Handlers
      protected override void OnItemAdded( object sender, RepositoryObjectEventArgs e )
      {
         FoodItem dataObject = e.Item as FoodItem;

         if (dataObject != null)
         {
            _children.Add( new ClickableTreeNodeViewModel( dataObject.Name, _childClickCommand, dataObject.ID ) );
         }
      }
      #endregion
   }
}
