using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Muffle.Data.Models;

namespace Muffle.Converters
{
    public class MessageTypeToTextVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageType messageType)
            {
                return messageType == MessageType.Text;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageTypeToImageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageType messageType)
            {
                return messageType == MessageType.Image;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrEmpty(base64String))
            {
                try
                {
                    // For placeholder data, return null to hide the image
                    if (base64String == System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("PLACEHOLDER_IMAGE_DATA")))
                    {
                        return null;
                    }
                    
                    byte[] imageBytes = System.Convert.FromBase64String(base64String);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str) && str != System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("PLACEHOLDER_IMAGE_DATA"));
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}