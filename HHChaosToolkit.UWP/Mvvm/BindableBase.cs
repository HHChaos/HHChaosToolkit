using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.ApplicationModel;

namespace HHChaosToolkit.UWP.Mvvm
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        [IgnoreDataMember] public bool IsInDesignMode => DesignMode.DesignModeEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propName = GetPropertyName(propertyExpression);
            if (!string.IsNullOrEmpty(propName))
                RaisePropertyChanged(propName);
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpress)
                return memberExpress.Member.Name;

            return string.Empty;
        }

        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T storage, T value)
        {
            var propName = GetPropertyName(propertyExpression);
            if (!string.IsNullOrEmpty(propName))
                return Set(ref storage, value, propName);
            return false;
        }
    }
}