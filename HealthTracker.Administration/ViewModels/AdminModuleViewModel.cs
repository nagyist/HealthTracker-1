using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.Administration.ViewModels
{
   /// <summary>
   /// View model for the Administration module main window
   /// </summary>
   [Export]
   public class AdminModuleViewModel : WorkspaceViewModel
   {
      #region Constructors
      [ImportingConstructor]
      public AdminModuleViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( regionManager, interactionService, logger )
      {
         _dataRepository = dataRepository;
         
         _adminNodes = new ObservableCollection<TreeNodeViewModel>();
         AdminNodes = new ReadOnlyObservableCollection<TreeNodeViewModel>( _adminNodes );
         _adminNodes.Add( new FoodGroupNodeViewModel( _dataRepository, this.ShowFoodGroupCommand ) );
         _adminNodes.Add( new MealTypeNodeViewModel( _dataRepository, this.ShowMealTypeCommand ) );
         _adminNodes.Add( new FoodItemNodeViewModel( _dataRepository, this.ShowFoodItemCommand ) );
         _adminNodes.Add( new MealTemplateNodeViewModel( _dataRepository, this.ShowMealTemplateCommand ) );

         Title = DisplayStrings.AdminModuleTitle;
      }
      #endregion

      #region Public Interface
      private ObservableCollection<TreeNodeViewModel> _adminNodes;
      public ReadOnlyObservableCollection<TreeNodeViewModel> AdminNodes { get; private set; }
      #endregion

      #region Commands
      private RelayCommand _showMealTypeCommand;
      public ICommand ShowMealTypeCommand
      {
         get
         {
            if (_showMealTypeCommand == null)
            {
               _showMealTypeCommand = new RelayCommand( ShowMealType );
            }

            return _showMealTypeCommand;
         }
      }

      private RelayCommand _showFoodGroupCommand;
      public ICommand ShowFoodGroupCommand
      {
         get
         {
            if (_showFoodGroupCommand == null)
            {
               _showFoodGroupCommand = new RelayCommand( ShowFoodGroup );
            }
            return _showFoodGroupCommand;
         }
      }

      private RelayCommand _showFoodItemCommand;
      public ICommand ShowFoodItemCommand
      {
         get
         {
            if (_showFoodItemCommand == null)
            {
               _showFoodItemCommand = new RelayCommand( ShowFoodItem );
            }
            return _showFoodItemCommand;
         }
      }

      private RelayCommand _showMealTemplateCommand;
      public ICommand ShowMealTemplateCommand
      {
         get
         {
            if (_showMealTemplateCommand == null)
            {
               _showMealTemplateCommand = new RelayCommand( ShowMealTemplate );
            }
            return _showMealTemplateCommand;
         }
      }
      #endregion

      #region Private Helpers
      IDataRepository _dataRepository;

      private void ShowFoodGroup( Object id )
      {
         NavigateToView( ViewNames.FoodGroupView, id );
      }

      private void ShowFoodItem( Object id )
      {
         NavigateToView( ViewNames.FoodItemView, id );
      }

      private void ShowMealType( Object id )
      {
         NavigateToView( ViewNames.MealTypeView, id );
      }

      private void ShowMealTemplate( Object id )
      {
         NavigateToView( ViewNames.MealTemplateView, id );
      }

      private void NavigateToView( String viewName, Object id )
      {
         UriQuery query = new UriQuery();
         if (id != null)
         {
            query.Add( "ID", ((Guid)id).ToString() );
         }

         Uri navigateTo = new Uri( viewName + query.ToString(), UriKind.Relative );
         this.RegionManager.RequestNavigate( RegionNames.AdministrationTabsRegion, navigateTo );
      }
      #endregion
   }
}
