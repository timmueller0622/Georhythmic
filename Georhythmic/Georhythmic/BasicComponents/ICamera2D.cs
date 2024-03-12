using Microsoft.Xna.Framework;

namespace Georhythmic.BasicComponents
{
    public interface ICamera2D
    {
        Vector2 Position { get; set; }

        Matrix View { get; }
    }
}
