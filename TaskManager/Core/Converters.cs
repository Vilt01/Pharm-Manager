using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskManager.Core.Converters
{
    public class PasswordVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible && isVisible)
                return MaterialDesignThemes.Wpf.PackIconKind.Eye;
            return MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}