namespace BandAPI.Services
{
    public interface IPropertyValidationService
    {
        bool HasValidProperties<T>(string fields);
    }
}