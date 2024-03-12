using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Georhythmic.BasicComponents
{
    public interface IBasicSprite
    {
        Game Game { get; }

        GraphicsDevice GraphicsDevice { get; }

        bool Enabled { get; set; }

        bool Visible { get; set; }

        string Name { get; set; }

        string Tag { get; set; }

        Texture2D Texture { get; set; }

        Point TextureSize { get; }

        Point Size { get; set; }

        Vector2 Position { get; set; }

        Vector2 CenterPosition { get; set; }

        Vector2 Scale { get; set; }

        Rectangle Destination { get; set; }

        Rectangle Clipping { get; set; }

        Color Tint { get; set; }

        SpriteEffects SpriteEffect { get; set; }

        float Rotation { get; set; }

        Rectangle Bounds { get; }

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void CenterImage();

        void CenterImageHorizontally();

        void CenterImageVertically();
    }
}