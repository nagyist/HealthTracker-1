using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Regions;
using DataObject = HealthTracker.DataRepository.Models.DataObject;
using Microsoft.Practices.Prism.Logging;

namespace HealthTracker.DataRepository.ViewModels
{
   /// <summary>
   /// Abstract base class for view models that link views to models for data that is in the DataRepository
   /// </summary>
   public abstract class DataRepositoryObjectViewModel : WorkspaceViewModel, IEditableObject, IDataErrorInfo, INavigationAware
   {
      #region Constructors
      public DataRepositoryObjectViewModel( IDataRepository dataRepository, IRegionManager regionManager, IInteractionService interactionService, ILoggerFacade logger )
         : base( regionManager, interactionService, logger )
      {
         Model = null;
         InTransaction = false;
         this.DataRepository = dataRepository;
      }
      #endregion

      #region Protected Interface
      protected RepositoryObjectBase Model { get; set; }
      protected Boolean InTransaction { get; private set; }
      protected IDataRepository DataRepository { get; private set; }

      protected Boolean UserWantsToDelete( String questionToAsk )
      {
         if (this.InteractionService.ShowMessageBox(
            questionToAsk, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes)
         {
            return true;
         }

         return false;
      }


      protected CloseAction DetermineCloseAction( String questionToAskIfDirtyAndCanSave, String questionToAskIfDirtyAndInvalid )
      {
         if (CanSave)
         {
            switch (this.InteractionService.ShowMessageBox(
               questionToAskIfDirtyAndCanSave, DisplayStrings.SaveChangesCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question ))
            {
               case MessageBoxResult.Yes:
                  return CloseAction.SaveAndClose;

               case MessageBoxResult.No:
                  return CloseAction.Close;

               case MessageBoxResult.Cancel:
                  return CloseAction.DoNotClose;
            }
         }
         else
         {
            if (IsDirty && !IsValid)
            {
               switch (this.InteractionService.ShowMessageBox(
                  questionToAskIfDirtyAndInvalid, DisplayStrings.CloseCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ))
               {
                  case MessageBoxResult.No:
                     return CloseAction.DoNotClose;

                  default:
                     return CloseAction.Close;
               }
            }
         }

         return CloseAction.Close;
      }
      #endregion

      #region Protected Virtual Methods
      /// <summary>
      /// Resets various state variables, such as the previous values and the Title
      /// </summary>
      protected virtual void Reset()
      {
         SetCacheValues();
         if (Name != null)
         {
            Title = Name;
         }

         // This will change the IsDirty flag if it was true
         OnPropertyChanged( IsDirtyPropertyName );
      }
      #endregion

      #region Public Interface
      protected String IDPropertyName = "ID";
      public Guid ID { get { return Model.ID; } }

      protected String IsUsedPropertyName = "IsUsed";
      public abstract Boolean IsUsed { get; }

      protected String IsNewPropertyName = "IsNew";
      public abstract Boolean IsNew { get; }

      protected String IsDirtyPropertyName = "IsDirty";
      public virtual Boolean IsDirty
      {
         get
         {
            return this.Name != this._previousName || this.Description != this._previousDescription;
         }
      }

      protected String IsValidPropertyName = "IsValid";
      /// <summary>
      /// Determine if the object is valid or not.
      /// </summary>
      /// <returns>true if the object is valid, false otherwise</returns>
      public virtual Boolean IsValid
      {
         get
         {
            String error = null;

            foreach (String propertyName in ValidatedProperties)
            {
               switch (propertyName)
               {
                  case "Name":
                     error = ValidateName();
                     break;


                  default:
                     Debug.Fail( "Unexpected property: " + propertyName );
                     break;
               }

               if (error != null)
               {
                  return false;
               }
            }

            return Model.IsValid;
         }
      }

      protected const String NamePropertyName = "Name";
      public String Name
      {
         get { return ((DataObject)Model).Name; }
         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<DataRepositoryObjectViewModel, String>( this, NamePropertyName, UndoableActions.Modify, Name, value ) );
            ((DataObject)Model).Name = value;
            OnPropertyChanged( NamePropertyName );
            OnPropertyChanged( IsDirtyPropertyName );
            OnPropertyChanged( IsValidPropertyName );
         }
      }

      protected const String DescriptionPropertyName = "Description";
      public String Description
      {
         get { return ((DataObject)Model).Description; }
         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<DataRepositoryObjectViewModel, String>(
                  this, DescriptionPropertyName, UndoableActions.Modify, Description, value ) );
            ((DataObject)Model).Description = value;
            base.OnPropertyChanged( DescriptionPropertyName );
            base.OnPropertyChanged( IsDirtyPropertyName );
         }
      }
      #endregion

      #region Commands
      private RelayCommand _saveCommand;
      public ICommand SaveCommand
      {
         get
         {
            if (_saveCommand == null)
            {
               _saveCommand = new RelayCommand( param => this.Save(), param => this.CanSave );
            }

            return _saveCommand;
         }
      }

      private RelayCommand _deleteCommand;
      public ICommand DeleteCommand
      {
         get
         {
            if (_deleteCommand == null)
            {
               _deleteCommand = new RelayCommand( param => this.Delete(), param => this.CanDelete );
            }

            return _deleteCommand;
         }
      }

      protected abstract void Save();

      protected virtual Boolean CanSave
      {
         get { return (IsDirty || IsNew) && IsValid; }
      }

      protected abstract void Delete();

      protected virtual Boolean CanDelete
      {
         get { return !IsNew && !IsUsed; }
      }
      #endregion

      #region IDataErrorInfo Methods
      public override String this[string propertyName]
      {
         get
         {
            String error = null;

            error = base[propertyName];

            if (error == null)
            {
               if (propertyName == "Name")
               {
                  error = ValidateName();
               }
            }

            // If the error is still null, check the model
            if (error == null)
            {
               error = Model[propertyName];
            }

            return error;
         }
      }
      #endregion

      #region IEditableObject Methods
      public virtual void BeginEdit()
      {
         if (!InTransaction)
         {
            SetCacheValues();
            InTransaction = true;
         }
      }

      public virtual void CancelEdit()
      {
         if (InTransaction)
         {
            Description = _previousDescription;
            Name = _previousName;
            InTransaction = false;
         }
      }

      public virtual void EndEdit()
      {
         if (InTransaction)
         {
            SetCacheValues();
            this.Save();
            InTransaction = false;
         }
      }
      #endregion

      #region INavigation Aware Methods
      public virtual Boolean IsNavigationTarget( NavigationContext navigationContext )
      {
         String idParameter = navigationContext.Parameters["ID"];
         Guid targetId;

         if (idParameter == null || !Guid.TryParse( idParameter, out targetId ))
         {
            return false;
         }

         return this.ID == targetId;
      }

      public virtual void OnNavigatedTo( NavigationContext navigationContext )
      {
         Reset();
      }

      public virtual void OnNavigatedFrom( NavigationContext navigationContext )
      {
      }
      #endregion

      #region Validations
      private static readonly String[] ValidatedProperties = 
      {
         "Name"
      };

      protected virtual String ValidateName()
      {
         return null;
      }
      #endregion

      #region Private Data
      //
      // Previous Value Cache - used to determine if the current data is dirty
      private String _previousName { get; set; }
      private String _previousDescription { get; set; }
      private void SetCacheValues()
      {
         this._previousName = this.Name;
         this._previousDescription = this.Description;
      }
      #endregion
   }
}
