using Emr.Application.Common.Interfaces;
using QRCoder;

namespace Emr.Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    public string GenerateQrCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new Base64QRCode(qrCodeData);
        
        var qrCodeImage = qrCode.GetGraphic(20);
        return $"data:image/png;base64,{qrCodeImage}";
    }

    public byte[] GenerateQrCodeImage(string data, int size = 300)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        
        return qrCode.GetGraphic(size / 20); // Pixel size calculation
    }
}