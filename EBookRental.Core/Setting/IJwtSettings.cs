namespace EBookRental.Core.Setting
{
    public interface IJwtSettings
    {
        string Key { get; }
        string Issuer { get; }
        string Audience { get; }
        int ExpireDays { get; }
    }
}
