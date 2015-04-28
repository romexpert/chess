using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Image = System.Windows.Controls.Image;

namespace Common
{
    public class Chessman : Image
    {
        private Point _currentPosition;
        private FeagureType _type;
        private Color _color;
        private Func<Point, IEnumerable<Point>> _possibleMoves;
        private Func<Point, IEnumerable<Point>> _hits;
        private static Color _kingUnderCheckColor = Color.None;

        internal Chessman(Point position, FeagureType type, Color color)
        {
            Width = Height = 70;
            
            _currentPosition = position;

            InitFeagure(type, color);

            Gameboard.AddFeagure(position, this);
        }

        void InitFeagure(FeagureType type, Color color)
        {
            Source = FeagureImages.GetImage(color, type);
            _possibleMoves = RuleProvider.GetMoveRule(color, type);
            _hits = RuleProvider.GetHitRule(color, type);
            _type = type;
            _color = color;
        }

        internal Point CurrentPosition
        {
            get { return _currentPosition; }
        }

        internal Color Color
        {
            get { return _color; }
        }

        internal FeagureType Type
        {
            get { return _type; }
        }

        public bool IsTurn
        {
            get { return Game.Current.IsTurn(_color); }
        }

        public bool HasPosition(int x, int y)
        {
            return _currentPosition == new Point(x, y);
        }

        public bool CanGo(Point point, bool checkKing = false)
        {
            var canGo = _possibleMoves(_currentPosition).Contains(point);
            if (!canGo)
                return false;

            if (_type == FeagureType.King)
            {
                canGo = !Gameboard.IsKingBecamaUnderAttack(point, _color);
                if (canGo && Gameboard.Busy(point) == ColorHelper.GetOpposite(_color))
                {
                    canGo = !Gameboard.IsProtectedPoint(point);
                }
            }

            if (checkKing && _type != FeagureType.King && Gameboard.IsKingUnderAttack(_color, point, this))
                return false;
            
            return canGo;
        }

        internal bool CanEscape()
        {
            var moves = _possibleMoves(_currentPosition).Where(_ => _.X >= 0 && _.Y >= 0 && _.X < Gameboard.BoardSize && _.Y < Gameboard.BoardSize);

            var points = moves.Where(_ => CanGo(_, true)).ToList();

            if (!points.Any())
                return false;

            Debug.WriteLine("Can excape to {0} {1}", points.First().X, points.First().Y);

            return true;
        }

        public bool CanHit(Point point)
        {
            return _hits(_currentPosition).Contains(point);
        }

        public void Go(Point point)
        {
            var previousPoint = _currentPosition;
            _currentPosition = point;
            Gameboard.MoveFeagure(point, this);

            if (_type == FeagureType.Pawn)
            {
                if(_color == Common.Color.White && point.Y == 0 || _color == Color.Black && point.Y == Gameboard.BoardSize - 1)
                    throw new RequireExchangeException{ Color = _color };
            }

            if(Gameboard.CheckFatality(_color))
                throw new FatalityException { WinColor = _color };

            Trace.TraceInformation("{0} {1} from {2} goto {3}", _color, _type, previousPoint, point);
        }

        public void Mutate(FeagureType type)
        {
            if(_type != FeagureType.Pawn)
                throw new Exception("Impossible to mutate.");

            InitFeagure(type, _color);
        }
    }
}