using HangmanWpf.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace HangmanWpf.Views.Converters;

public class ImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string imagePath || string.IsNullOrWhiteSpace(imagePath))
        {
            return null;
        }

        var resolvedPath = Path.IsPathRooted(imagePath)
            ? imagePath
            : PathHelpers.GetRelativePath(imagePath);

        if (!File.Exists(resolvedPath))
        {
            return null;
        }

        return new BitmapImage(new Uri(resolvedPath, UriKind.Absolute));
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
