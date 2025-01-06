namespace DiscountCodeSystem.Core.Entity
{
    public record GenerateRequest(ushort Count, byte Length);
    public record GenerateResponse(bool Result,  List<string>? Codes = null);
    public record UseCodeRequest(string Code);
    public record UseCodeResponse(byte Result);
    public record ValidationError(string Field, string Message);
}