using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Common
{
    public class FeagureImages
    {
        private static BitmapImage _whiteBishop,
            _whiteKing,
            _whiteKnight,
            _whitePawn,
            _whiteQueen,
            _whiteRook,
            _blackBishop,
            _blackKing,
            _blackKnight,
            _blackPawn,
            _blackQueen,
            _blackRook;

        static Assembly _assembly = Assembly.GetExecutingAssembly();

        static FeagureImages()
        {
            _whiteBishop = LoadImage("Common.Resources.White_Bishop.png");
            _whiteKing = LoadImage("Common.Resources.White_King.png");
            _whiteKnight = LoadImage("Common.Resources.White_Knight.png");
            _whitePawn = LoadImage("Common.Resources.White_Pawn.png");
            _whiteQueen = LoadImage("Common.Resources.White_Queen.png");
            _whiteRook = LoadImage("Common.Resources.White_Rook.png");

            _blackBishop = LoadImage("Common.Resources.Black_Bishop.png");
            _blackKing = LoadImage("Common.Resources.Black_King.png");
            _blackKnight = LoadImage("Common.Resources.Black_Knight.png");
            _blackPawn = LoadImage("Common.Resources.Black_Pawn.png");
            _blackQueen = LoadImage("Common.Resources.Black_Queen.png");
            _blackRook = LoadImage("Common.Resources.Black_Rook.png");
        }

        private static BitmapImage LoadImage(string uri)
        {
            var image = new BitmapImage();
            using (var stream = _assembly.GetManifestResourceStream(uri))
            {
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
            }

            return image;
        }

        public static BitmapImage GetImage(Color color, FeagureType type)
        {
            switch (color)
            {
                case Color.White:
                    switch (type)
                    {
                        case FeagureType.Bishop:
                            return _whiteBishop;
                        case FeagureType.King:
                            return _whiteKing;
                        case FeagureType.Knight:
                            return _whiteKnight;
                        case FeagureType.Pawn:
                            return _whitePawn;
                        case FeagureType.Queen:
                            return _whiteQueen;
                        case FeagureType.Rook:
                            return _whiteRook;
                        default:
                            throw new Exception("No such white feagure");
                    }
                case Color.Black:
                    switch (type)
                    {
                        case FeagureType.Bishop:
                            return _blackBishop;
                        case FeagureType.King:
                            return _blackKing;
                        case FeagureType.Knight:
                            return _blackKnight;
                        case FeagureType.Pawn:
                            return _blackPawn;
                        case FeagureType.Queen:
                            return _blackQueen;
                        case FeagureType.Rook:
                            return _blackRook;
                        default:
                            throw new Exception("No such black feagure");
                    }
                default:
                    throw new Exception("No such image");
            }
        }
    }
}
