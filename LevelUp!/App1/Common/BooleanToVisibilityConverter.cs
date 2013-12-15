using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace levelupspace.Common
{
    /// <summary>
    /// Конвертер значений, который преобразует значение true в значение <see cref="Visibility.Visible"/> и значение false в
    /// значение <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
