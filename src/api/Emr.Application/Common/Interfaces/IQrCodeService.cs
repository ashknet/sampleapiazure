namespace Emr.Application.Common.Interfaces;

public interface IQrCodeService
{
    string GenerateQrCode(string data);
    byte[] GenerateQrCodeImage(string data, int size = 300);
}