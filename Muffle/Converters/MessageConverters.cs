using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Muffle.Data.Models;

namespace Muffle.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }
    }

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
                    // Check if this is placeholder data (starts with our placeholder prefix)
                    var decodedBytes = System.Convert.FromBase64String(base64String);
                    var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);
                    
                    if (decodedString.StartsWith("IMAGE_DATA_FOR_"))
                    {
                        // This is placeholder data, don't try to display as image
                        return null;
                    }
                    
                    // Try to create ImageSource from actual image data
                    return ImageSource.FromStream(() => new MemoryStream(decodedBytes));
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
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                try
                {
                    // Check if this is placeholder data
                    var decodedBytes = System.Convert.FromBase64String(str);
                    var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);
                    
                    // Don't show placeholder data as images
                    return !decodedString.StartsWith("IMAGE_DATA_FOR_");
                }
                catch
                {
                    // If base64 decode fails, it's not our data format
                    return false;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}