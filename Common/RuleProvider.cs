using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Common
{
    class RuleProvider
    {
        static Dictionary<Tuple<Color, FeagureType>, Func<Point, IEnumerable<Point>>> _moveRules = new Dictionary<Tuple<Color, FeagureType>, Func<Point, IEnumerable<Point>>>();
        static Dictionary<Tuple<Color, FeagureType>, Func<Point, IEnumerable<Point>>> _hitRules = new Dictionary<Tuple<Color, FeagureType>, Func<Point, IEnumerable<Point>>>();
        private static Color _kingUnderCheckColor = Color.None;

        static RuleProvider()
        {
            _moveRules.Add(Tuple.Create(Color.White, FeagureType.Bishop), WhiteBishopMoves);
            _moveRules.Add(Tuple.Create(Color.White, FeagureType.King), WhiteKingMoves);
            _moveRules.Add(Tuple.Create(Color.White, FeagureType.Knight), WhiteKnightMoves);
            _moveRules.Add(Tuple.Create(Color.White, FeagureType.Pawn), WhitePawnMoves);
            _moveRules.Add(Tuple.Create(Color.White, FeagureType.Queen), WhiteQueenMoves);
            _moveRules.Add(Tuple.Create(Color.White, FeagureType.Rook), WhiteRookMoves);

            _moveRules.Add(Tuple.Create(Color.Black, FeagureType.Bishop), BlackBishopMoves);
            _moveRules.Add(Tuple.Create(Color.Black, FeagureType.King), BlackKingMoves);
            _moveRules.Add(Tuple.Create(Color.Black, FeagureType.Knight), BlackKnightMoves);
            _moveRules.Add(Tuple.Create(Color.Black, FeagureType.Pawn), BlackPawnMoves);
            _moveRules.Add(Tuple.Create(Color.Black, FeagureType.Queen), BlackQueenMoves);
            _moveRules.Add(Tuple.Create(Color.Black, FeagureType.Rook), BlackRookMoves);

            _hitRules.Add(Tuple.Create(Color.White, FeagureType.Bishop), WhiteBishopMoves);
            _hitRules.Add(Tuple.Create(Color.White, FeagureType.King), WhiteKingMoves);
            _hitRules.Add(Tuple.Create(Color.White, FeagureType.Knight), WhiteKnightMoves);
            _hitRules.Add(Tuple.Create(Color.White, FeagureType.Pawn), WhitePawnHitPoints);
            _hitRules.Add(Tuple.Create(Color.White, FeagureType.Queen), WhiteQueenMoves);
            _hitRules.Add(Tuple.Create(Color.White, FeagureType.Rook), WhiteRookMoves);

            _hitRules.Add(Tuple.Create(Color.Black, FeagureType.Bishop), BlackBishopMoves);
            _hitRules.Add(Tuple.Create(Color.Black, FeagureType.King), BlackKingMoves);
            _hitRules.Add(Tuple.Create(Color.Black, FeagureType.Knight), BlackKnightMoves);
            _hitRules.Add(Tuple.Create(Color.Black, FeagureType.Pawn), BlackPawnHitPoints);
            _hitRules.Add(Tuple.Create(Color.Black, FeagureType.Queen), BlackQueenMoves);
            _hitRules.Add(Tuple.Create(Color.Black, FeagureType.Rook), BlackRookMoves);
        }


        internal static Func<Point, IEnumerable<Point>> GetMoveRule(Color color, FeagureType type)
        {
            return _moveRules[Tuple.Create(color, type)];
        }

        internal static Func<Point, IEnumerable<Point>> GetHitRule(Color color, FeagureType type)
        {
            return _hitRules[Tuple.Create(color, type)];
        }

        static IEnumerable<Point> WhiteBishopMoves(Point currentPoint)
        {
            return BishopMoves(currentPoint, Color.White);
        }

        static IEnumerable<Point> BlackBishopMoves(Point currentPoint)
        {
            return BishopMoves(currentPoint, Color.Black);
        }

        static IEnumerable<Point> BishopMoves(Point currentPoint, Color color)
        {
            var result = new List<Point>();

            for (int rightX = currentPoint.X + 1, lowerY = currentPoint.Y + 1;
                rightX < Gameboard.BoardSize || lowerY < Gameboard.BoardSize;
                rightX++, lowerY++)
            {
                if (!AddMoveIfNeeded(result, rightX, lowerY, color))
                    break;
            }

            for (int leftX = currentPoint.X - 1, lowerY = currentPoint.Y + 1;
                leftX >= 0 || lowerY < Gameboard.BoardSize;
                leftX--, lowerY++)
            {
                if (!AddMoveIfNeeded(result, leftX, lowerY, color))
                    break;
            }

            for (int leftX = currentPoint.X - 1, upperY = currentPoint.Y - 1;
                leftX >= 0 || upperY >= 0;
                leftX--, upperY--)
            {
                if (!AddMoveIfNeeded(result, leftX, upperY, color))
                    break;
            }

            for (int rightX = currentPoint.X + 1, upperY = currentPoint.Y - 1;
                rightX < Gameboard.BoardSize || upperY >= 0;
                rightX++, upperY--)
            {
                if (!AddMoveIfNeeded(result, rightX, upperY, color))
                    break;
            }

            return result;
        }

        static IEnumerable<Point> WhiteKingMoves(Point currentPoint)
        {
            return KingMoves(currentPoint, Color.White);
        }

        static IEnumerable<Point> BlackKingMoves(Point currentPoint)
        {
            return KingMoves(currentPoint, Color.Black);
        }

        static IEnumerable<Point> KingMoves(Point currentPoint, Color color)
        {
            IEnumerable<Point> points = new[]
            {
                new Point(currentPoint.X, currentPoint.Y - 1)
                , new Point(currentPoint.X + 1, currentPoint.Y - 1)
                , new Point(currentPoint.X + 1, currentPoint.Y)
                , new Point(currentPoint.X + 1, currentPoint.Y + 1)
                , new Point(currentPoint.X, currentPoint.Y + 1)
                , new Point(currentPoint.X - 1, currentPoint.Y + 1)
                , new Point(currentPoint.X - 1, currentPoint.Y)
                , new Point(currentPoint.X - 1, currentPoint.Y - 1)
            };

            //TODO: not under attack
            points = points.Where(_ => Gameboard.Busy(_) != color).ToList();

            return points;
        }

        static IEnumerable<Point> WhiteKnightMoves(Point currentPoint)
        {
            return KnightMoves(currentPoint, Color.White);
        }

        static IEnumerable<Point> BlackKnightMoves(Point currentPoint)
        {
            return KnightMoves(currentPoint, Color.Black);
        }

        static IEnumerable<Point> KnightMoves(Point currentPoint, Color color)
        {
            var points = new[]
            {
                new Point(currentPoint.X + 1, currentPoint.Y - 2)
                , new Point(currentPoint.X + 2, currentPoint.Y - 1)
                , new Point(currentPoint.X + 2, currentPoint.Y + 1)
                , new Point(currentPoint.X + 1, currentPoint.Y + 2)

                , new Point(currentPoint.X - 1, currentPoint.Y - 2)
                , new Point(currentPoint.X - 2, currentPoint.Y - 1)
                , new Point(currentPoint.X - 2, currentPoint.Y + 1)
                , new Point(currentPoint.X - 1, currentPoint.Y + 2)
            };

            return points.Where(_ => Gameboard.Busy(_) != color);
        }

        static IEnumerable<Point> WhiteQueenMoves(Point currentPoint)
        {
            return QueenMoves(currentPoint, Color.White);
        }

        static IEnumerable<Point> BlackQueenMoves(Point currentPoint)
        {
            return QueenMoves(currentPoint, Color.Black);
        }

        static IEnumerable<Point> QueenMoves(Point currentPoint, Color color)
        {
            return RookMoves(currentPoint, color).Concat(BishopMoves(currentPoint, color));
        }

        static IEnumerable<Point> WhiteRookMoves(Point currentPoint)
        {
            return RookMoves(currentPoint, Color.White);
        }

        static IEnumerable<Point> BlackRookMoves(Point currentPoint)
        {
            return RookMoves(currentPoint, Color.Black);
        }

        static IEnumerable<Point> RookMoves(Point currentPoint, Color color)
        {
            var result = new List<Point>();

            for (var xRight = currentPoint.X + 1; xRight < Gameboard.BoardSize; xRight++)
            {
                if (!AddMoveIfNeeded(result, xRight, currentPoint.Y, color))
                    break;
            }

            for (var xLeft = currentPoint.X - 1; xLeft >= 0; xLeft--)
            {
                if (!AddMoveIfNeeded(result, xLeft, currentPoint.Y, color))
                    break;
            }

            for (var yBottom = currentPoint.Y + 1; yBottom < Gameboard.BoardSize; yBottom++)
            {
                if (!AddMoveIfNeeded(result, currentPoint.X, yBottom, color))
                    break;
            }

            for (var yUpper = currentPoint.Y - 1; yUpper >= 0; yUpper--)
            {
                if (!AddMoveIfNeeded(result, currentPoint.X, yUpper, color))
                    break;
            }

            return result;
        }

        /// <summary>
        /// Check and add move for long run feagures (bishop, rook, queen)
        /// </summary>
        /// <returns>If true can continue, if false need to break cycle</returns>
        static bool AddMoveIfNeeded(List<Point> result, int x, int y, Color color)
        {
            var point = new Point(x, y);
            var busyColor = Gameboard.Busy(point);

            if (busyColor == color)
                return false;

            result.Add(point);

            if (busyColor != Color.None && Gameboard.BusyType(point) == FeagureType.King)
                return true;

            return busyColor == Color.None;
        }

        static IEnumerable<Point> WhitePawnMoves(Point currentPoint)
        {
            var result = new List<Point>();

            //Forward move
            if (currentPoint.Y == 6)
            {
                var firstStep = new Point(currentPoint.X, 5);
                if (Gameboard.Busy(firstStep) == Color.None)
                {
                    result.Add(firstStep);

                    var secondStep = new Point(currentPoint.X, 4);
                    if (Gameboard.Busy(secondStep) == Color.None)
                        result.Add(secondStep);
                }
            }
            else
            {
                var step = new Point(currentPoint.X, currentPoint.Y - 1);

                if (Gameboard.Busy(step) == Color.None)
                    result.Add(step);
            }

            //Kill move
            var rightKill = new Point(currentPoint.X + 1, currentPoint.Y - 1);
            if(Gameboard.Busy(rightKill) == Color.Black)
                result.Add(rightKill);
            var leftKill = new Point(currentPoint.X - 1, currentPoint.Y - 1);
            if(Gameboard.Busy(leftKill) == Color.Black)
                result.Add(leftKill);

            return result;
        }


        static IEnumerable<Point> BlackPawnMoves(Point currentPoint)
        {
            var result = new List<Point>();

            //Forward move
            if (currentPoint.Y == 1)
            {
                var firstStep = new Point(currentPoint.X, 2);
                if (Gameboard.Busy(firstStep) == Color.None)
                {
                    result.Add(firstStep);

                    var secondStep = new Point(currentPoint.X, 3);
                    if (Gameboard.Busy(secondStep) == Color.None)
                        result.Add(secondStep);
                }
            }
            else
            {
                var step = new Point(currentPoint.X, currentPoint.Y + 1);

                if (Gameboard.Busy(step) == Color.None)
                    result.Add(step);
            }

            //Kill move
            var rightKill = new Point(currentPoint.X + 1, currentPoint.Y + 1);
            if (Gameboard.Busy(rightKill) == Color.White)
                result.Add(rightKill);
            var leftKill = new Point(currentPoint.X - 1, currentPoint.Y + 1);
            if (Gameboard.Busy(leftKill) == Color.White)
                result.Add(leftKill);

            return result;
        }



        static IEnumerable<Point> WhitePawnHitPoints(Point currentPoint)
        {
            var result = new List<Point>();

            var rightKill = new Point(currentPoint.X + 1, currentPoint.Y - 1);
            result.Add(rightKill);

            var leftKill = new Point(currentPoint.X - 1, currentPoint.Y - 1);
            result.Add(leftKill);

            return result;
        }


        static IEnumerable<Point> BlackPawnHitPoints(Point currentPoint)
        {
            var result = new List<Point>();

            var rightKill = new Point(currentPoint.X + 1, currentPoint.Y + 1);
            result.Add(rightKill);

            var leftKill = new Point(currentPoint.X - 1, currentPoint.Y + 1);
            result.Add(leftKill);

            return result;
        }
    }
}