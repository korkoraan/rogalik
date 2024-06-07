using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rogalik.Framework;
using rogalik.Rendering;
using Myra;
using Myra.Graphics2D.UI;

namespace rogalik;

public class Game1 : Game
{
    public enum State
    {
        idle,
        usingUI,
        gameplay
    }
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Renderer renderer;
    public Desktop desktop;
    // somebody could just reset the world at any given time
    // but I'm so tired this evening
    public World world;
    public readonly Input input;
    public State state { get; private set; }

    // 1
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        input = new Input(this);
    }

    // 2 within the base.Initialize
    protected override void Initialize()
    {
        // should always be the first line
        base.Initialize();
        world = new World(this);
        renderer.Init();
        input.Init();
        
        _graphics.PreferredBackBufferWidth = renderer.windowSize.X;
        _graphics.PreferredBackBufferHeight = renderer.windowSize.Y;
        // _graphics.ToggleFullScreen();
        _graphics.ApplyChanges();
    }

    // 3 
    protected override void LoadContent()
    {
        base.LoadContent();
        R.contentManager = Content;
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        MyraEnvironment.Game = this;
        
        var fontSystem = new FontSystem();
        //TODO: can't load fonts
        // fontSystem.AddFont(File.ReadAllBytes(path));
        
        renderer = new Renderer(this, new (1920, 1080), _spriteBatch, fontSystem);
        desktop = new Desktop();
        renderer.InitUI(desktop);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Q))
            Exit();

        input.Update(gameTime);
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        if(!world.initialized) return;
        _spriteBatch.Begin(transformMatrix: renderer.GetTransformMatrix(), samplerState: SamplerState.PointClamp);
        renderer.DrawVisibleArea();
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.End();
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        desktop.Render();
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    public void Pause()
    {
        world.Pause();
    }

    public void Resume()
    {
        world.Resume();
    }
}