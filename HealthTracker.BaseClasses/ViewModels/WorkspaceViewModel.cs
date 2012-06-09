using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.BaseClasses.ViewModels
{
   /// <summary>
   /// Base class for view model classes that associated with UI workspaces.
   /// A "workspace" is anything in the UI that could potentially be closed, such as a tab or a window.
   /// </summary>
   public abstract class WorkspaceViewModel : UndoableViewModelBase, IDisposable, INotifyPropertyChanged, IRegionMemberLifetime
   {
      #region Private Data
      private Boolean _workspaceViewRemoved;
      #endregion

      #region Constructors
      public WorkspaceViewModel( IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base()
      {
         this.RegionManager = regionManager;
         this.InteractionService = interactionService;
         this.Logger = logger;

         _workspaceViewRemoved = false;
      }
      #endregion

      #region Public Interface
      private String _title;
      public String Title
      {
         get
         {
            return _title;
         }
         protected set
         {
            _title = value;
            OnPropertyChanged( "Title" );
         }
      }
      #endregion

      #region Protected Interface
      protected IRegionManager RegionManager { get; private set; }
      protected IInteractionService InteractionService { get; private set; }
      protected ILoggerFacade Logger { get; private set; }
      #endregion

      #region Commands
      private RelayCommand _closeCommand;
      public ICommand CloseCommand
      {
         get
         {
            if (_closeCommand == null)
            {
               _closeCommand = new RelayCommand( param => this.OnRequestClose() );
            }

            return _closeCommand;
         }
      }
      #endregion

      #region Events
      public EventHandler RequestClose;

      private void FindAndRemoveViews()
      {
         foreach (IRegion region in this.RegionManager.Regions)
         {
            foreach (UserControl view in region.Views)
            {
               if (view.DataContext == this)
               {
                  region.Remove( view );
                  _workspaceViewRemoved = true;
               }
            }
         }
      }

      protected virtual void OnRequestClose()
      {
         if (RequestClose != null)
         {
            RequestClose( this, EventArgs.Empty );
         }
         FindAndRemoveViews();
         this.Dispose();
      }
      #endregion

      #region IDisposable Methods
      public void Dispose()
      {
         this.OnDispose( true );
         GC.SuppressFinalize( this );
      }

      /// <summary>
      /// Deriving classes should override this method if they have stuff that needs disposing.
      /// </summary>
      /// <param name="disposing"></param>
      protected virtual void OnDispose( Boolean disposing ) { }

      ~WorkspaceViewModel()
      {
         this.OnDispose( false );
      }
      #endregion

      #region INotifyPropertyChanged Members
      /// <summary>
      /// Raised when a property on this object has a new value.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Raises this object's PropertyChanged event.
      /// </summary>
      /// <param name="propertyName">The property that has a new value.</param>
      protected virtual void OnPropertyChanged( string propertyName )
      {
         //this.VerifyPropertyName( propertyName );

         PropertyChangedEventHandler handler = this.PropertyChanged;
         if (handler != null)
         {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs( propertyName );
            handler( this, e );
         }
      }
      #endregion

      #region IDataErrorInfo Methods
      public virtual String Error
      {
         get
         {
            StringBuilder error = new StringBuilder();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties( this ))
            {
               String propertyError = this[property.Name];
               if (!String.IsNullOrEmpty( propertyError ))
               {
                  error.Append( ((error.Length == 0) ? "" : ", ") + propertyError );
               }
            }

            return error.ToString();
         }
      }

      public virtual String this[string propertyName]
      {
         get
         {
            String error = null;

            //error = Model[propertyName];

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            CommandManager.InvalidateRequerySuggested();

            return error;
         }
      }
      #endregion

      #region IRegionMemberLifetime Methods
      public Boolean KeepAlive { get { return !_workspaceViewRemoved; } }
      #endregion
   }
}
