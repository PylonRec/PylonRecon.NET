namespace PylonRecon.Helpers;

public interface IValueConverter<TFrom, TTo>
{
    TTo Convert(TFrom from);
    TFrom ConvertBack(TTo backFrom);
}