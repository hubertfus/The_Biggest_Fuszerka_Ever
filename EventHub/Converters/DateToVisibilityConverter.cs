using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EventHub.Converters  
{
    public class DateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is DateTime eventDate)
            {
                DateTime eventDay = eventDate.Date;
                DateTime today = DateTime.Today;

                return eventDay >= today ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}