using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HealthTracker.DataRepository.Models
{
   /// <summary>
   /// Base class for all objects that will be stored in a first class collection in 
   /// the data repository (FoodGroups, MealTypes, FoodItems, etc).
   /// </summary>
   public abstract class RepositoryObjectBase : IDataErrorInfo
   {
      #region Constructors
      public RepositoryObjectBase( Guid id )
      {
         ID = id;
      }

      public RepositoryObjectBase()
         : this( Guid.NewGuid() )
      {
      }
      #endregion

      #region Public Interface
      public Guid ID { get; private set; }
      public virtual Boolean IsValid { get { return true; } }

      public virtual void InitializeData( RepositoryObjectBase source )
      {
         this.ID = source.ID;
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
                  error.Append( ((error.Length == 0) ? "" : "  ") + propertyError );
               }
            }

            return error.ToString();
         }
      }

      public virtual String this[String PropertyName]
      {
         get
         {
            // Could probalby add a validation for a NULL ID, but that is not 
            // really something that is possible.
            return null;
         }
      }
      #endregion
   }
}
