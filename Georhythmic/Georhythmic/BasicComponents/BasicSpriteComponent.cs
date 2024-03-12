using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Georhythmic.BasicComponents
{
    public class BasicSpriteComponent : DrawableGameComponent, IBasicSprite
    {
        private string name;

        private string tag;

        private string imageName;

        private Texture2D image;

        private Vector2 position;

        private Vector2 pivot;

        private Rectangle destination;

        private Rectangle clipping;

        private float rotation = 0f;

        private Color color = Color.White;

        private SpriteEffects spriteEffect = SpriteEffects.None;

        private SpriteBatch spriteBatch;

        private BlendState blendState = BlendState.AlphaBlend;

        private Effect effect = null;

        private Matrix view = Matrix.Identity;

        private ICamera2D camera;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == null || value == "")
                {
                    throw new InvalidOperationException("Ungültiger Name");
                }
                name = value;
            }
        }

        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (value == null || value == "")
                {
                    throw new InvalidOperationException("Ungültiger Tag");
                }

                tag = value;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                if (clipping.Width <= 0 || clipping.Height <= 0)
                {
                    clipping.Width = image.Width;
                    clipping.Height = image.Height;
                }
                else
                {
                    if (clipping.Y + clipping.Height > image.Height)
                    {
                        clipping.Height = image.Height - clipping.Y;
                    }

                    if (clipping.X + clipping.Width > image.Width)
                    {
                        clipping.Width = image.Width - clipping.X;
                    }
                }

                Vector2 scale = Scale;
                if (scale.X <= 0f)
                {
                    scale.X = 1f;
                }

                if (scale.Y <= 0f)
                {
                    scale.Y = 1f;
                }

                pivot.X = clipping.Width / 2f;
                pivot.Y = clipping.Height / 2f;
                destination.Width = (int)(clipping.Width * scale.X);
                destination.Height = (int)(clipping.Height * scale.Y);
            }
        }

        public Point TextureSize
        {
            get
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Kein Bild zugewiesen");
                }

                return new Point(image.Width, image.Height);
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                destination.X = (int)value.X;
                destination.Y = (int)value.Y;
            }
        }

        public Vector2 CenterPosition
        {
            get
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Die Bildmitte kann erst abgefragt werden, wenn ein Image zugewiesen oder geladen ist");
                }

                return new Vector2(position.X + destination.Width / 2f, position.Y + destination.Height / 2f);
            }
            set
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Die Bildmitte darf erst gesetzt werden, wenn ein Image zugewiesen oder geladen ist");
                }

                Position = new Vector2(value.X - destination.Width / 2f, value.Y - destination.Height / 2f);
            }
        }

        public Vector2 Scale
        {
            get
            {
                return new Vector2(destination.Width / (float)clipping.Width, destination.Height / (float)clipping.Height);
            }
            set
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Ein Skalierungsfaktor darf erst gesetzt werden, wenn ein Image zugewiesen oder geladen ist");
                }

                if (value.X <= 0f || value.Y <= 0f)
                {
                    throw new InvalidOperationException("Der Skalierungsfaktor muss größer sein als Null");
                }

                destination.Width = (int)(clipping.Width * value.X);
                destination.Height = (int)(clipping.Height * value.Y);
            }
        }

        public Point Size
        {
            get
            {
                return new Point(destination.Width, destination.Height);
            }
            set
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Die Bildgröße darf erst gesetzt werden, wenn ein Image zugewiesen oder geladen ist");
                }

                if (value.X <= 0 || value.Y <= 0)
                {
                    throw new InvalidOperationException("Size muss größer sein als Null");
                }

                destination.Width = value.X;
                destination.Height = value.Y;
            }
        }

        public Vector2 SizeAsVector2 => new Vector2(destination.Width, destination.Height);

        public Rectangle Destination
        {
            get
            {
                return destination;
            }
            set
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Die Destination darf erst gesetzt werden, wenn ein Image zugewiesen oder geladen ist");
                }

                if (value.Width <= 1 || value.Height <= 1)
                {
                    throw new InvalidOperationException("Breite und Höhe müssen mindestens den Wert 1 haben");
                }

                position = new Vector2(value.X, value.Y);
                destination = value;
            }
        }

        public Rectangle Clipping
        {
            get
            {
                return clipping;
            }
            set
            {
                if (image == null)
                {
                    throw new InvalidOperationException("Clipping darf erst gesetzt werden, wenn ein Image geladen oder zugewiesen ist");
                }

                if (value.X < 0 || value.Y < 0)
                {
                    throw new InvalidOperationException("Der Wert darf nicht kleiner als Null sein");
                }

                if (value.Width <= 0 || value.Height <= 0)
                {
                    throw new InvalidOperationException("Breite und Höhe des Rechtecks müssen Größer als Null sein");
                }

                if (value.Width > image.Width - value.X || value.Height > image.Height - value.Y)
                {
                    throw new InvalidOperationException("Das Rechteck ist größer als das Bild");
                }

                Vector2 scale = Scale;
                clipping = value;
                pivot.X = clipping.Width / 2f;
                pivot.Y = clipping.Height / 2f;
                destination = new Rectangle((int)position.X, (int)position.Y, (int)(clipping.Width * scale.X), (int)(clipping.Height * scale.Y));
            }
        }

        public float Rotation
        {
            get
            {
                return MathHelper.ToDegrees(rotation);
            }
            set
            {
                rotation = MathHelper.ToRadians(value % 360f);
            }
        }

        public float RadiansRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value % ((float)Math.PI * 2f);
            }
        }

        public Vector2 Up
        {
            get
            {
                if (rotation == 0f)
                {
                    return -Vector2.UnitY;
                }

                return Vector2.Normalize(Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation))));
            }
            set
            {
                float num = MathHelper.ToDegrees((float)Math.Acos(Vector2.Dot(-Vector2.UnitY, Vector2.Normalize(value))));
                if (value.X < 0f)
                {
                    num = 180f + (180f - num);
                }

                Rotation = num;
            }
        }

        public Vector2 Down
        {
            get
            {
                return Vector2.Negate(Up);
            }
            set
            {
                Up = Vector2.Negate(value);
            }
        }

        public Vector2 Right
        {
            get
            {
                if (rotation == 0f)
                {
                    return Vector2.UnitX;
                }

                return Vector2.Normalize(Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation))));
            }
            set
            {
                float num = MathHelper.ToDegrees((float)Math.Acos(Vector2.Dot(Vector2.UnitX, Vector2.Normalize(value))));
                if (value.Y < 0f)
                {
                    num = 180f + (180f - num);
                }

                Rotation = num;
            }
        }

        public Vector2 Left
        {
            get
            {
                return Vector2.Negate(Right);
            }
            set
            {
                Right = Vector2.Negate(value);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                if (rotation == 0f)
                {
                    return destination;
                }

                Vector2 vector = new Vector2(destination.Width / 2f, -destination.Height / 2f);
                Vector2 vector2 = new Vector2(destination.Width / 2f, destination.Height / 2f);
                Vector2 value = Vector2.Transform(vector, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)));
                Vector2 value2 = Vector2.Transform(vector2, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)));
                value.X = Math.Abs(value.X);
                value.Y = Math.Abs(value.Y);
                value2.X = Math.Abs(value2.X);
                value2.Y = Math.Abs(value2.Y);
                Vector2 vector3 = Vector2.Max(value, value2);
                return new Rectangle((int)(destination.Center.X - vector3.X), (int)(destination.Center.Y - vector3.Y), (int)(vector3.X * 2f), (int)(vector3.Y * 2f));
            }
        }

        public Color Tint
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public SpriteEffects SpriteEffect
        {
            get
            {
                return spriteEffect;
            }
            set
            {
                spriteEffect = value;
            }
        }

        public BlendState BlendState
        {
            get
            {
                return blendState;
            }
            set
            {
                blendState = value;
            }
        }

        public Matrix View
        {
            get
            {
                return view;
            }
            set
            {
                bool flag = false;
                view = value;
            }
        }

        public ICamera2D Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        public Effect Effect
        {
            get
            {
                return effect;
            }
            set
            {
                effect = value;
            }
        }

        public BasicSpriteComponent(Game game, string name)
            : base(game)
        {
            this.name = name;
            if (Game.GraphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            }
        }

        public BasicSpriteComponent(Game game, string name, string imageName)
            : base(game)
        {
            this.name = name;
            this.imageName = imageName;
            if (Game.GraphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(Game.GraphicsDevice);
                image = game.Content.Load<Texture2D>(imageName);
                destination.Width = image.Width;
                destination.Height = image.Height;
                clipping.Width = image.Width;
                clipping.Height = image.Height;
                pivot = new Vector2(clipping.Width / 2f, clipping.Height / 2f);
            }
        }

        public BasicSpriteComponent(Game game, string name, Texture2D image)
            : base(game)
        {
            this.name = name;
            this.image = image;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            destination.Width = image.Width;
            destination.Height = image.Height;
            clipping.Width = image.Width;
            clipping.Height = image.Height;
            pivot = new Vector2(clipping.Width / 2f, clipping.Height / 2f);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (image == null && imageName != null)
            {
                image = Game.Content.Load<Texture2D>(imageName);
                clipping.Width = image.Width;
                clipping.Height = image.Height;
                destination.Width = image.Width;
                destination.Height = image.Height;
                pivot = new Vector2(clipping.Width / 2f, clipping.Height / 2f);
            }

            if (spriteBatch == null)
            {
                spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
            {
                return;
            }

            if (image != null)
            {
                if (camera != null)
                {
                    view = camera.View;
                }

                spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null, null, null, effect, view);
                spriteBatch.Draw(image, new Rectangle((int)(destination.X + destination.Width / 2f), (int)(destination.Y + destination.Height / 2f), destination.Width, destination.Height), clipping, color, rotation, pivot, spriteEffect, 0f);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void CenterImageVertically()
        {
            if (image == null)
            {
                throw new InvalidOperationException("Die Methode darf erst aufgerufen werden, wenn ein Image zugewiesen oder geladen ist");
            }

            Position = new Vector2(Position.X, (Game.GraphicsDevice.Viewport.Height - destination.Height) / 2f);
        }

        public void CenterImageHorizontally()
        {
            if (image == null)
            {
                throw new InvalidOperationException("Die darf erst aufgerufen werden, wenn ein Image zugewiesen oder geladen ist");
            }

            Position = new Vector2((Game.GraphicsDevice.Viewport.Width - destination.Width) / 2f, Position.Y);
        }

        public void CenterImage()
        {
            CenterImageHorizontally();
            CenterImageVertically();
        }

        public void UpPointAt(Vector2 target)
        {
            Up = Vector2.Normalize(target - CenterPosition);
        }

        public void DownPointAt(Vector2 target)
        {
            Down = Vector2.Normalize(target - CenterPosition);
        }

        public void RightPointAt(Vector2 target)
        {
            Right = Vector2.Normalize(target - CenterPosition);
        }

        public void LeftPointAt(Vector2 target)
        {
            Left = Vector2.Normalize(target - CenterPosition);
        }

        public void RotateAround(Vector2 point, float angle)
        {
            CenterPosition = point + Vector2.Transform(CenterPosition - point, Matrix.CreateRotationZ(MathHelper.ToRadians(angle)));
            Rotation += angle;
        }

        public void Move(float x, float y)
        {
            Move(new Vector2(x, y));
        }

        public void Move(Vector2 shift)
        {
            Position += shift;
        }

        public void MoveCenter(float x, float y)
        {
            MoveCenter(new Vector2(x, y));
        }

        public void MoveCenter(Vector2 shift)
        {
            CenterPosition += shift;
        }

        [SpecialName]
        Game IBasicSprite.get_Game()
        {
            return Game;
        }

        [SpecialName]
        GraphicsDevice IBasicSprite.get_GraphicsDevice()
        {
            return GraphicsDevice;
        }

        [SpecialName]
        void IBasicSprite.set_Enabled(bool value)
        {
            Enabled = value;
        }

        [SpecialName]
        void IBasicSprite.set_Visible(bool value)
        {
            Visible = value;
        }
    }
}
