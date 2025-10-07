using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Converters;

public class IsActiveMultiParameterConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // 确保我们收到了预期的两个参数
        if (values.Count == 2 && values[0] is Song song)
        {
            // CheckBox 的 IsChecked 属性是 nullable bool (bool?)
            bool isChecked = values[1] != null ? (bool)values[1] : !song.IsActive;

            // 返回一个包含 Song 对象和选中状态的元组
            return new Tuple<Song, bool>(song, isChecked);
        }

        return null;
    }

    public object? ConvertBack(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}