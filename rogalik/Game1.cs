using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rogalik.Framework;
using rogalik.Rendering;

namespace rogalik;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Renderer _renderer;
    private World _world;
    private SpriteFont _font;

    // 1
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    // 2 within the base.Initialize
    protected override void Initialize()
    {
        // should always be the first line
        base.Initialize();
        _world = new World();
        _renderer = new Renderer(new (1920, 1080), _spriteBatch, _world);
        
        _graphics.PreferredBackBufferWidth = _renderer.windowSize.x;
        _graphics.PreferredBackBufferHeight = _renderer.windowSize.y;
        _graphics.ToggleFullScreen();
        _graphics.ApplyChanges();
    }

    // 3 
    protected override void LoadContent()
    {
        R.contentManager = Content;
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Input.Update(gameTime);
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(transformMatrix: _renderer.GetTransformMatrix(), samplerState: SamplerState.PointClamp);
        _renderer.DrawCells();
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.End();
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _renderer.DrawUI();
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}