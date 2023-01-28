using Gma.QrCodeNet.Encoding;

namespace WalletWasabi.Fluent;

public static class RandomQrDesign
{
	public static bool[,] GetRandomQrCode()
	{
		string data = "QR code data " + new Random().Next();
		QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.H);
		QrCode qrCode = encoder.Encode(data);

		// Create a boolean array representing the QR code
		int width = qrCode.Matrix.Width;
		int height = qrCode.Matrix.Height;
		bool[,] qrArray = new bool[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				qrArray[x, y] = qrCode.Matrix[x, y];
			}
		}

		return qrArray;
	}
}
