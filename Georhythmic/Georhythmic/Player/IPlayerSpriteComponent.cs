using Microsoft.Xna.Framework;

namespace Georhythmic.Player
{
    public interface IPlayerSpriteComponent
    {
        bool Enabled { get; set; } //if set false, component will not be included in game loop
        void Initialize(); //initialize the component
        void Update(GameTime gameTime); //update with gametime that can express behavior
        Player Sprite { get; set; } //player object as property
    }
}
