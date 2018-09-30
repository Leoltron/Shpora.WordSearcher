using System;

namespace Shpora.WordSearcher.Extensions
{
    public static class DirectionExtensions
    {
        public static (int dx, int dy) ToCoordsChange(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (0, -1);
                case Direction.Right:
                    return (1, 0);
                case Direction.Down:
                    return (0, 1);
                case Direction.Left:
                    return (-1, 0);
                default:
                    throw new ArgumentException("Unexpected direction: " + direction);
            }
        }
    }
}
