using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Interfaces;

namespace HealthTracker.Infrastructure
{
   public class UndoablePropertyValue<ObjectType, PropertyType> : IUndoRedo
   {
      #region Constructors
      public UndoablePropertyValue(ObjectType instance, String propertyName, UndoableActions action, PropertyType previousValue, PropertyType newValue)
         : this (instance, propertyName, action, previousValue, newValue, propertyName)
      {
      }

      public UndoablePropertyValue( ObjectType instance, String propertyName, UndoableActions action, PropertyType previousValue, PropertyType newValue, String name )
      {
         _instance = instance;
         _propertyName = propertyName;
         _action = action;
         _previousValue = previousValue;
         _newValue = newValue;
         Name = name;
      }

      #endregion

      #region Public Interface
      public String Name { get; private set; }

      public void Undo()
      {
         IList<PropertyType> theList;

         switch (_action)
         {
            case UndoableActions.Add:
               theList = _instance.GetType().GetProperty( _propertyName ).GetValue( _instance, null ) as IList<PropertyType>;
               theList.Remove( _newValue );
               break;

            case UndoableActions.Modify:
               _instance.GetType().GetProperty( _propertyName ).SetValue( _instance, _previousValue, null );
               break;

            case UndoableActions.Remove:
               theList = _instance.GetType().GetProperty( _propertyName ).GetValue( _instance, null ) as IList<PropertyType>;
               theList.Add( _previousValue );
               break;
         }
      }

      public void Redo()
      {
         IList<PropertyType> theList;

         switch (_action)
         {
            case UndoableActions.Add:
               theList = _instance.GetType().GetProperty( _propertyName ).GetValue( _instance, null ) as IList<PropertyType>;
               theList.Add( _newValue );
               break;

            case UndoableActions.Modify:
               _instance.GetType().GetProperty( _propertyName ).SetValue( _instance, _newValue, null );
               break;

            case UndoableActions.Remove:
               theList = _instance.GetType().GetProperty( _propertyName ).GetValue( _instance, null ) as IList<PropertyType>;
               theList.Remove( _previousValue );
               break;
         }
      }
      #endregion

      #region Private Memebers
      // Previous value, used during undo to restore value
      private PropertyType _previousValue;

      // The new value that was set, used during redo
      private PropertyType _newValue;

      // The object whose property is being changed
      private ObjectType _instance;

      // The name of the property that is being changed
      private String _propertyName;

      // The action that was taken
      private UndoableActions _action;
      #endregion
   }
}
