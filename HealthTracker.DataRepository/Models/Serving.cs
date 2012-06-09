using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Properties;

namespace HealthTracker.DataRepository.Models
{
   public class Serving<T> : IDataErrorInfo
   {
      #region Constructors
      public Serving()
         : this( default( T ), 0 )
      {
      }

      public Serving( T entity, Decimal quantity )
      {
         Entity = entity;
         Quantity = quantity;
      }

      public Serving( Serving<T> serving )
         : this( serving.Entity, serving.Quantity )
      {
      }
      #endregion

      #region properties
      public T Entity { get; set; }
      public Decimal Quantity { get; set; }
      #endregion

      #region IDataErrorInfo Memebers
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
            if (PropertyName == propertyNameEntity)
            {
               return ValidateItem();
            }
            else if (PropertyName == propertyNameQuantity)
            {
               return ValidateQuantity();
            }

            return null;
         }
      }

      #endregion

      #region Validations
      private const String propertyNameEntity = "Entity";
      private const String propertyNameQuantity = "Quantity";

      static readonly String[] ValidatedProperties = 
      {
         propertyNameEntity,
         propertyNameQuantity
      };

      public virtual Boolean IsValid
      {
         get
         {
            String error = null;

            foreach (String propertyName in ValidatedProperties)
            {
               switch (propertyName)
               {
                  case propertyNameEntity:
                     error = ValidateItem();
                     break;

                  case propertyNameQuantity:
                     error = ValidateQuantity();
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

            return true;
         }
      }

      private String ValidateItem()
      {
         if (EqualityComparer<T>.Default.Equals( Entity, default( T ) ))
         {
            return Messages.Error_No_ServingItem;
         }

         return null;
      }

      private String ValidateQuantity()
      {
         if (Quantity <= 0)
         {
            return Messages.Error_No_Quantity;
         }

         return null;
      }
      #endregion
   }
}
