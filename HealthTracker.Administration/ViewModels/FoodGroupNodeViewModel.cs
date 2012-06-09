using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;

namespace HealthTracker.Administration.ViewModels
{
   public class FoodGroupNodeViewModel : DataObjectCollectionNodeViewModel
   {
      #region Constructors
      public FoodGroupNodeViewModel( IDataRepository dataRepository, ICommand childClickCommand )
         : base( DisplayStrings.AdminFoodGroupsTitle, dataRepository, childClickCommand )
      {
         _children =
            new ObservableCollection<TreeNodeViewModel>(
               (from foodGroup in dataRepository.GetAllFoodGroups()
                select new ClickableTreeNodeViewModel( foodGroup.Name, childClickCommand, foodGroup.ID )).ToList() );
         Children = new ReadOnlyObservableCollection<TreeNodeViewModel>( _children );
      }
      #endregion

      #region Event Handlers
      protected override void OnItemAdded( object sender, RepositoryObjectEventArgs e )
      {
         FoodGroup dataObject = e.Item as FoodGroup;

         if (dataObject != null)
         {
            _children.Add( new ClickableTreeNodeViewModel( dataObject.Name, _childClickCommand, dataObject.ID ) );
         }
      }
      #endregion
   }
}
