using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GFMWakeUpHelper.App.Converters;

public class ListStringToVisibilityConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<string> list)
        {
            return string.Join(" / ", list);
        }
        throw new NotImplementedException();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            return stringValue.Split(" / ");
        }
        throw new NotImplementedException();
    }
}

public class DateTimeToVisibilityConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}