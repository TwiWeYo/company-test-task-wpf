using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TaskManager.Model;

namespace TaskManager.Utils;

[ValueConversion(typeof(DateTime), typeof(string))]
public class DateToFormattedStringConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return string.Empty;

        var date = value as DateTime?;
        if (date.HasValue)
            return date.Value.ToString("dd.MM.yyyy");

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


[ValueConversion(typeof(DateTime?), typeof(Visibility))]
public class DateToVisibilityConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var date = value as DateTime?;

        return date.HasValue && date.Value < DateTime.Today ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


[ValueConversion(typeof(Enum), typeof(string))]
public class EnumToDescriptionConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var enumValue = value as Enum;

        return enumValue!.Description();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


[ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
public class EnumToCollectionConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return EnumHelper.GetAllValuesAndDescriptions(value.GetType());
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


[ValueConversion(typeof(Priority), typeof(Brush))]
public class PriorityToColorConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Priority priority)
            return null;

        return priority switch
        {
            Priority.Low => (Brush)Application.Current.TryFindResource("MaterialDesign.Brush.Primary.Light"),
            Priority.Medium => (Brush)Application.Current.TryFindResource("MaterialDesign.Brush.Primary"),
            Priority.High => (Brush)Application.Current.TryFindResource("MaterialDesign.Brush.Primary.Dark"),
            _ => throw new NotImplementedException(),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


public class InverseAndBooleansToBooleanConverter : MarkupExtension, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length > 0)
        {
            foreach (var value in values)
            {
                if (value is bool && (bool)value)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}