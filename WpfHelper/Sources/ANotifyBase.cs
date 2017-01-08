using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WpfHelper
{
    public abstract class ANotifyBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _propertyValues = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected T Set<T>(T value, Expression<Func<T>> prop)
        {
            var expr = prop.Body as MemberExpression;

            if (expr != null)
            {
                var member = expr.Member;

                lock (_propertyValues)
                {
                    if (_propertyValues.ContainsKey(member.Name))
                    {
                        var oldValue = _propertyValues[member.Name];
                        if ((value == null) || (oldValue == null) || (!oldValue.Equals(value)))
                        {
                            _propertyValues[member.Name] = value;
                            OnPropertyChanged(member.Name);
                        }
                    }
                    else
                    {
                        _propertyValues.Add(member.Name, value);
                        OnPropertyChanged(member.Name);
                    }
                }
            }
            return value;
        }

        protected T Get<T>(Expression<Func<T>> prop)
        {
            var expr = prop.Body as MemberExpression;

            if (expr != null)
            {
                var member = expr.Member;

                lock (_propertyValues)
                {
                    if (_propertyValues.ContainsKey(member.Name))
                    {
                        return (T)_propertyValues[member.Name];
                    }
                }

                return default(T);
            }

            return default(T);
        }
        protected void RegisterCollectionChanged<T>(ObservableCollection<T> collection)
        {
            collection.CollectionChanged += items_CollectionChanged;
        }

        #region Events

        // Create the OnPropertyChanged method to raise the event
        public void OnPropertyChanged(params string[] propertyNames)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        // Detects changes within an item in an observable collection
        protected virtual void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;
            }
        }

        protected virtual void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion Events
    }
}
