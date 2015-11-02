using System;
using System.Globalization;
using System.Windows.Data;

namespace CShell.Framework.Converters
{
    [ValueConversion(typeof(Uri), typeof(string))]  
    public class UriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uri = value as Uri;
            if (uri != null)
                return uri.ToString();
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uriString = value as string;
            if (uriString != null)
            {
                Uri uri;
                if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                    return uri;
            }
            return null;
        }
    }
}
