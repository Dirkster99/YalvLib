﻿using System.Linq.Expressions;

namespace YalvLib.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// INotifyPropertyChange Implementation
    /// Implements the INotifyPropertyChanged interface and 
    /// exposes a RaisePropertyChanged method for derived 
    /// classes to raise the PropertyChange event. The event 
    /// arguments created by this class are cached to prevent 
    /// managed heap fragmentation.
    /// Refs: http://www.codeproject.com/KB/WPF/WPF_NHibernate_Validator.aspx
    /// </summary>
    [Serializable]
    public abstract class BindableObject : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> _EventArgCache;
        private static readonly object _SyncLock = new object();

        #region Constructors
        /// <summary>
        /// Static class constructor to initialize private static dictionary object.
        /// </summary>
        static BindableObject()
        {
            _EventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public BindableObject()
        {
            this.IsPropertyChangedEventEnabled = true;
        }
        #endregion Constructors

        #region Pattern Observable

        /// <summary>
        /// Raised when a public property of this object is set.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Indica se è abilitata la notifica del cambiamento di proprietà
        /// </summary>
        public bool IsPropertyChangedEventEnabled { get; set; }

        /// <summary>
        /// Returns an instance of PropertyChangedEventArgs for 
        /// the specified property name.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to create event args for.
        /// </param>
        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName cannot be null or empty.");

            PropertyChangedEventArgs args;
            lock (BindableObject._SyncLock)
            {
                if (!_EventArgCache.TryGetValue(propertyName, out args))
                {
                    _EventArgCache.Add(propertyName, args = new PropertyChangedEventArgs(propertyName));
                }
            }

            return args;
        }

        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// 
        /// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
        /// where 'this' is derived from <seealso cref="BindableObject"/>
        /// and IsSelected is a property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        public void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
                memberExpression = (MemberExpression)lambda.Body;

            this.RaisePropertyChanged(memberExpression.Member.Name);
        }

        /// <summary>
        /// Attempts to raise the PropertyChanged event, and 
        /// invokes the virtual AfterPropertyChanged method, 
        /// regardless of whether the event was raised or not.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        public void RaisePropertyChanged(string propertyName)
        {
            // Se non è abilitata la notifica del cambiamento di proprietà allora non faccio niente
            if (!this.IsPropertyChangedEventEnabled)
                return;

            this.VerifyProperty(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                // Get the cached event args.
                PropertyChangedEventArgs args =
                    GetPropertyChangedEventArgs(propertyName);

                // Raise the PropertyChanged event.
                handler(this, args);
            }

            this.OnAfterPropertyChanged(propertyName);
        }

        /// <summary>
        /// Derived classes can override this method to
        /// execute logic after a property is set. The 
        /// base implementation does nothing.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        protected virtual void OnAfterPropertyChanged(string propertyName)
        {
        }
        #endregion Observable

        #region Helpers

        [Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            if (propertyName.IndexOf(".") >= 0)
                return;

            // Thanks to Rama Krishna Vavilala for the tip to use TypeDescriptor here, instead of manual
            // reflection, so that custom properties are honored too.
            // http://www.codeproject.com/KB/WPF/podder1.aspx?msg=2381272#xx2381272xx
            bool propertyExists = TypeDescriptor.GetProperties(this).Find(propertyName, false) != null;

            if (!propertyExists)
            {
                // The property could not be found,
                // so alert the developer of the problem.
                string msg = string.Format(
                    "{0} is not a public property of {1}",
                    propertyName,
                    this.GetType().FullName);

                Debug.Fail(msg);
            }
        }

        #endregion Helpers
    }
}
