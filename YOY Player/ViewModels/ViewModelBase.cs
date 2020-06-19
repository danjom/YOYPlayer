using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.ViewModels
{
    public class VerifiedViewModel : ViewModelBase
    {
        #region Validation

        private readonly Dictionary<string, Binder> ruleMap = new Dictionary<string, Binder>();

        public void AddRule<T>(Expression<Func<T>> expression, Func<bool> ruleDelegate, string errorMessage)
        {
            var name = GetPropertyName(expression);

            ruleMap.Add(name, new Binder(ruleDelegate, errorMessage));
        }

        protected override void Set<T>(Expression<Func<T>> path, T value, bool forceUpdate)
        {
            ruleMap[GetPropertyName(path)].IsDirty = true;
            base.Set<T>(path, value, forceUpdate);
        }

        public bool HasErrors
        {
            get
            {
                var values = ruleMap.Values.ToList();
                values.ForEach(rule => rule.Update());

                return values.Any(rule => rule.HasError);
            }
        }

        public string Error
        {
            get => string.Join(Environment.NewLine, ruleMap.Values.Where(r => r.HasError).Select(r => r.Error));
        }

        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update();
                    return ruleMap[columnName].Error;
                }
                return null;
            }
        }

        private class Binder
        {
            private readonly Func<bool> ruleDelegate;
            private readonly string message;

            internal Binder(Func<bool> ruleDelegate, string message)
            {
                this.ruleDelegate = ruleDelegate;
                this.message = message;

                IsDirty = true;
            }

            internal string Error { get; set; }
            internal bool HasError { get; set; }

            internal bool IsDirty { get; set; }

            internal void Update()
            {
                if (!IsDirty)
                    return;

                Error = null;
                HasError = false;
                try
                {
                    if (!ruleDelegate())
                    {
                        Error = message;
                        HasError = true;
                    }
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    HasError = true;
                }
            }
        }

        #endregion
    }

    public class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        #region Notifier

        public event PropertyChangedEventHandler PropertyChanged;
        internal void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Advanced ViewModel

        private Dictionary<string, object> propertyValueMap;

        protected ViewModelBase()
        {
            propertyValueMap = new Dictionary<string, object>();
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> path)
        {
            string propertyName = GetPropertyName(path);
            Notify(propertyName);
        }

        #region Get

        protected T Get<T>(Expression<Func<T>> path)
        {
            return Get(path, default(T));
        }

        protected virtual T Get<T>(Expression<Func<T>> path, T defaultValue)
        {
            var propertyName = GetPropertyName(path);
            if (propertyValueMap.ContainsKey(propertyName))
            {
                return (T)propertyValueMap[propertyName];
            }
            else
            {
                propertyValueMap.Add(propertyName, defaultValue);
                return defaultValue;
            }
        }

        #endregion

        #region Set

        protected void Set<T>(Expression<Func<T>> path, T value)
        {
            Set(path, value, false);
        }

        protected virtual void Set<T>(Expression<Func<T>> path, T value, bool forceUpdate)
        {
            var oldValue = Get(path);
            var propertyName = GetPropertyName(path);

            if (!object.Equals(value, oldValue) || forceUpdate)
            {
                propertyValueMap[propertyName] = value;
                OnPropertyChanged(path);
            }
        }

        #endregion

        protected static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Expression body = expression.Body;
            MemberExpression memberExpression = body as MemberExpression;
            if (memberExpression == null)
            {
                memberExpression = (MemberExpression)((UnaryExpression)body).Operand;
            }
            return memberExpression.Member.Name;
        }

        #endregion
    }
}