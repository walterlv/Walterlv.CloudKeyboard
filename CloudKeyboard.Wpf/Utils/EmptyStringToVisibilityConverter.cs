using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Walterlv.CloudTyping.Utils
{
    public class EmptyStringToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public EmptyStringToVisibilityConverter() => IsVisibleWhenEmpty = false;

        public EmptyStringToVisibilityConverter(bool isVisibleWhenEmpty) => IsVisibleWhenEmpty = isVisibleWhenEmpty;

        [ConstructorArgument("isVisibleWhenEmpty")]
        public bool IsVisibleWhenEmpty { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var whenEmpty = IsVisibleWhenEmpty ? Visibility.Visible : Visibility.Collapsed;
            var whenNotEmpty = IsVisibleWhenEmpty ? Visibility.Collapsed : Visibility.Visible;
            return string.IsNullOrEmpty(value as string)
                ? whenEmpty
                : whenNotEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
