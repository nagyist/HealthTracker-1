using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HealthTracker.DataRepository.Models;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.Infrastructure;

namespace HealthTracker.DataRepository.ViewModels
{
   /// <summary>
   /// Models a Serving
   /// </summary>
   public class ServingViewModel<T> : UndoableViewModelBase, IDataErrorInfo, IEditableObject, INotifyPropertyChanged where T : DataObject
   {
      #region Constructors
      public ServingViewModel()
         : this( default( T ), 0 )
      { }

      public ServingViewModel( T entity, Decimal quantity )
         : this( new Serving<T>( entity, quantity ) )
      { }

      public ServingViewModel( Serving<T> serving )
         : base()
      {
         Model = serving;

         _previousEntity = serving.Entity;
         _previousQuantity = serving.Quantity;
      }
      #endregion

      #region Public Interface
      public Serving<T> Model { get; private set; }

      protected String EntityPropertyName = "Entity";
      public T Entity
      {
         get
         {
            return Model.Entity;
         }

         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<ServingViewModel<T>, T>( this, EntityPropertyName, UndoableActions.Modify, Entity, value ) );
            Model.Entity = value;
            OnPropertyChanged( EntityPropertyName );
            OnPropertyChanged( IsValidPropertyName );
            OnPropertyChanged( IsDirtyPropertyName );
         }
      }

      protected String QuantityPropertyName = "Quantity";
      public Decimal Quantity
      {
         get
         {
            return Model.Quantity;
         }

         set
         {
            AddUndoableValue(
               new UndoablePropertyValue<ServingViewModel<T>, Decimal>( this, QuantityPropertyName, UndoableActions.Modify, Quantity, value ) );
            Model.Quantity = value;
            OnPropertyChanged( QuantityPropertyName );
            OnPropertyChanged( IsValidPropertyName );
            OnPropertyChanged( IsDirtyPropertyName );
         }
      }

      protected String IsValidPropertyName = "IsValid";
      public Boolean IsValid
      {
         get
         {
            return Model.IsValid;
         }
      }

      protected String IsDirtyPropertyName = "IsDirty";
      public Boolean IsDirty
      {
         get
         {
            return (Model.Quantity != _previousQuantity) || (Model.Entity != _previousEntity);
         }
      }

      public void ResetPreviousValueCache()
      {
         _previousEntity = Model.Entity;
         _previousQuantity = Model.Quantity;
      }
      #endregion

      #region Commands
      // None...
      #endregion

      #region IDataErrorInfo Methods
      public String Error
      {
         get { return Model.Error; }
      }

      public String this[string propertyName]
      {
         get
         {
            String error = null;

            error = Model[propertyName];

            return error;
         }
      }
      #endregion

      #region IEditableObject Methods
      private T _backupEntity;
      private Decimal _backupQuantity;
      public void BeginEdit()
      {
         if (!InTransaction)
         {
            InTransaction = true;
            _backupEntity = Entity;
            _backupQuantity = Quantity;
         }
      }

      public void CancelEdit()
      {
         if (InTransaction)
         {
            InTransaction = false;
            Entity = _backupEntity;
            Quantity = _backupQuantity;
         }
      }

      public void EndEdit()
      {
         if (InTransaction)
         {
            InTransaction = false;
            _backupEntity = default( T );
            _backupQuantity = default( Decimal );
         }
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
      protected void OnPropertyChanged( string propertyName )
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

      #region Private Data
      private Boolean InTransaction { get; set; }

      private T _previousEntity;
      private Decimal _previousQuantity;
      #endregion
   }
}
