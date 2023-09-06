using System;
using Gtk;
using Gdk;
using Tetris;
using Key = Gdk.Key;
using static Gtk.Orientation;
using Timeout = GLib.Timeout;

public class TetrisWindow : Gtk.Window
{
    private GameState gameState;
    private DrawingArea drawingArea;
    private Label scoreLabel;
    private Label highestScoreLabel;
    private Button pauseButton;
    private Button restartButton;
    private HashSet<Key> pressedKeys = new HashSet<Key>();
    private static uint gameSpeed = 500; // Dictates game speed
    private int checkpoint = 15; // Checkpoint when speed increases (15 lines as it starts and then it gets higher)

    private bool isPaused = false;
    private uint gameTimeoutId; // Variable to store the timeout source ID
    private uint gameTimeoutId2;


    public TetrisWindow() : base("Tetris")
    {
        
        // Set up the main window
        SetDefaultSize(685, 660);

        // Initialize the game state
        gameState = new GameState();

        // Create a DrawingArea to render the game board
        drawingArea = new DrawingArea();
        drawingArea.Drawn += OnDrawingAreaExposed;
        
        // Create an HBox for all the other boxes we are going to use
        Box container = new HBox();

        // Create tetris box where game will be played
        Box tetris = new Box(Horizontal, 0);
        tetris.PackStart(drawingArea, true, true, 0);
        tetris.BorderWidth = 3;

        // Create box for buttons and labels
        Box accessibles = new Box(Vertical, 1);
        
        pauseButton = new Button("PAUSE"); // Pause button
        pauseButton.Clicked += OnPauseClicked;
        accessibles.PackStart(pauseButton, false, false, 0);

        restartButton = new Button("RESTART"); // Restart button
        restartButton.Clicked += OnRestartClicked;
        accessibles.PackStart(restartButton, false, false, 0);

        scoreLabel = new Label(); // Label where score is displayed
        accessibles.PackStart(scoreLabel, false, false, 0);

        highestScoreLabel = new Label(); // Label where score is displayed
        accessibles.PackStart(highestScoreLabel, false, false, 0);

        container.Add(tetris);

        container.Add(accessibles);

        Add(container);

        gameTimeoutId2 = Timeout.Add(40, onTimeoutHandle);
        gameTimeoutId = Timeout.Add(gameSpeed, onTimeoutMain);

        ShowAll();
    }

    // Create Glib timeout methods
    bool onTimeoutHandle(){
        HandlePressedKeys();
        drawingArea.QueueDraw();
        return true;
    }

    bool onTimeoutMain(){

        gameState.MoveTetrominoDown();

        if (gameState.Score>=checkpoint)
        {
            gameSpeed = (uint)(gameSpeed/1.5);
            checkpoint = checkpoint + checkpoint + 5;

            GLib.Source.Remove(gameTimeoutId);

            gameTimeoutId = Timeout.Add(gameSpeed, onTimeoutMain);

        }

        // If game is over message dialog is displayed which telkl you your score
        if (gameState.GameOver)
        {
            var gameOverMsg = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.None, "YOU LOST, your score was " + gameState.Score * 100 + "")
            {
                Title = "GAME OVER"
            };
            gameOverMsg.AddButton("EXIT", ResponseType.Ok);
            ResponseType response = (ResponseType)gameOverMsg.Run();

            gameOverMsg.Destroy();
            Application.Quit();

        }

        return true;

    }

    private void OnDrawingAreaExposed(object sender, DrawnArgs args)
    {
        var drawable = drawingArea.Window;
        var context = Gdk.CairoHelper.Create(drawable);

        // Clear the drawing area
        context.Operator = Cairo.Operator.Clear;
        context.Paint();
        context.Operator = Cairo.Operator.Over;

        context.SetSourceRGB(0.8, 0.8, 0.8); // Light gray background color
        context.Rectangle(0, 0, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
        context.Fill();

        const int cellSize = 30;
        const int borderSize = 2;

        // Render the game grid
        for (int r = 0; r < gameState.GameGrid.Rows; r++)
        {
            for (int c = 0; c < gameState.GameGrid.Columns; c++)
            {
                int id = gameState.GameGrid[r, c];

                // Calculate the position and size of the cell
                int x = c * cellSize;
                int y = r * cellSize;

                // Draw the cell based on the id
                if (id != 0)
                {
                    switch (id)
                    {
                        case 1:
                            context.SetSourceRGB(0.0, 0.902, 0.902);
                            break;
                        case 2:
                            context.SetSourceRGB(0.0, 0.478, 0.667);
                            break;
                        case 3:
                            context.SetSourceRGB(1.0, 0.376, 0.0);
                            break;
                        case 4:
                            context.SetSourceRGB(1.0, 0.745, 0.0);
                            break;
                        case 5:
                            context.SetSourceRGB(0.0, 0.698, 0.294);
                            break;
                        case 6:
                            context.SetSourceRGB(0.565, 0.0, 0.651);
                            break;
                        case 7:
                            context.SetSourceRGB(0.902, 0.0, 0.0);
                            break;

                        default:
                            break;
                    }


                    // Draw a rectangle for the cell
                    context.Rectangle(x + borderSize/3, y + borderSize/3, cellSize - 2 * borderSize/3, cellSize - 2 * borderSize/3);

                    context.Fill();
                }
            }
        }

        ((IDisposable)context.Target).Dispose();

        // Render the current tetromino  on top of the grid
        var currentTetromino = gameState.CurrentTetromino;
        switch (currentTetromino.Id)
        {
            case 1:
                context.SetSourceRGB(0.0, 0.902, 0.902);
                break;
            case 2:
                context.SetSourceRGB(0.0, 0.478, 0.667);
                break;
            case 3:
                context.SetSourceRGB(1.0, 0.376, 0.0);
                break;
            case 4:
                context.SetSourceRGB(1.0, 0.745, 0.0);
                break;
            case 5:
                context.SetSourceRGB(0.0, 0.698, 0.294);
                break;
            case 6:
                context.SetSourceRGB(0.565, 0.0, 0.651);
                break;
            case 7:
                context.SetSourceRGB(0.902, 0.0, 0.0);
                break;

            default:
                break;
        }

        foreach (var position in currentTetromino.TilePositions())
        {
            int x = position.Column * cellSize;
            int y = position.Row * cellSize;

            // Draw a rectangle for the current tetromino cell
            context.Rectangle(x + borderSize, y + borderSize, cellSize - 2 * borderSize, cellSize - 2 * borderSize);

            context.Fill();
        }

        // Update the score labels
        scoreLabel.Text = $"Score: {gameState.Score*100}";

        highestScoreLabel.Text = $"Highscore: {gameState.HighestScore*100}";

        // Dispose of the Cairo context when done
        ((IDisposable)context.Target).Dispose();
    }

    
    // Handler for when the pause button is clicked

    private void OnPauseClicked(object sender, EventArgs e){
        
        isPaused = !isPaused;

        if(isPaused){
            GLib.Source.Remove(gameTimeoutId);
            GLib.Source.Remove(gameTimeoutId2);
            pauseButton.Label = "RESUME";
        } else {

            gameTimeoutId = Timeout.Add(gameSpeed, onTimeoutMain);
            gameTimeoutId2 = Timeout.Add(40, onTimeoutHandle);
            pauseButton.Label = "PAUSE";
        }

    }

    // Handler for when the restart button is clicked
    private void OnRestartClicked(object sender, EventArgs e){

        gameState = new GameState();
        drawingArea.QueueDraw();
    }


   // Handle key press event
    private void HandlePressedKeys()
    {
        if (pressedKeys.Contains(Key.Left))
        {
            gameState.moveTetrominoLeft();
        }
        else if (pressedKeys.Contains(Key.Right))
        {
            gameState.moveTetrominoRight();
        }
        else if (pressedKeys.Contains(Key.Down))
        {
            gameState.MoveTetrominoDown();
        }
        else if (pressedKeys.Contains(Key.Up))
        {
            gameState.rotateTetrominoClockwise();
        }

    }

    // Handle key press event
    protected override bool OnKeyPressEvent(EventKey e)
    {

        pressedKeys.Add(e.Key);

        return true;
    }

    // Handle key release event
    protected override bool OnKeyReleaseEvent(EventKey e)
    {
        pressedKeys.Remove(e.Key);
        return true;
    }

    protected override bool OnDeleteEvent(Event e) {
        Application.Quit();
        return true;
    }

    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();
        var mainWindow = new TetrisWindow();
        mainWindow.Resizable = false;
        Application.Run();
    }

}
