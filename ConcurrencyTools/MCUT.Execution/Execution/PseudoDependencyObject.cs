using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{

    public class PseudoDependencyProperty
    {
        public readonly string Name;
        public readonly object DefaultValue;    // The default value when the property value hasn't been set.

        public PseudoDependencyProperty(string name, object defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }

    public class TypedPseudoDependencyProperty<T> : PseudoDependencyProperty
    {

        public TypedPseudoDependencyProperty(string name, T defaultValue)
            : base(name, defaultValue)
        {
        }

    }

    /// <summary>
    /// Represents an object that has some of the APIs of a DependencyObject
    /// to allow for setting attached property values etc.
    /// </summary>
    public class PseudoDependencyObject
    {

        // Keyed by an object instance
        private Dictionary<PseudoDependencyProperty, object> _propertyValues;

        public PseudoDependencyObject()
        {
            _propertyValues = new Dictionary<PseudoDependencyProperty, object>();
        }

        public void SetValue<T>(TypedPseudoDependencyProperty<T> dp, T value)
        {
            _propertyValues[dp] = value;
        }

        public T GetValue<T>(TypedPseudoDependencyProperty<T> dp)
        {
            object value;
            if (!_propertyValues.TryGetValue(dp, out value))
                value = dp.DefaultValue;
            return (T)value;
        }

    }
}
