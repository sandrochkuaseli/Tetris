namespace Tetris {
    public class GameState{
        private Tetromino? currentTetromino; // Indicates the current tetromino that is falling

        public Tetromino CurrentTetromino{
            get => currentTetromino!;

            private set {
                currentTetromino = value;
                currentTetromino.reset();
            }
        }

        public GameGrid GameGrid { get;}
        public TetrominoQueue TetrominoQueue { get;}

        public bool GameOver {get; private set;}

        public int Score {get; set;}

        public int HighestScore { get; set;}

        public GameState(){
            GameGrid = new GameGrid(22, 10);
            TetrominoQueue = new TetrominoQueue();
            CurrentTetromino = TetrominoQueue.getAndUpdate();
            LoadHighestScore();
        }

        // This checks if the tetromino fits or not
        private bool tetrominoFits(){
            foreach(Position p in CurrentTetromino.TilePositions()){
                if(!GameGrid.isEmpty(p.Row, p.Column)){
                    return false;
                }
            }

            return true;
        }

        // Rotates tetromino clockwise
        public void rotateTetrominoClockwise(){
            CurrentTetromino.rotateClockwise();

            if (!tetrominoFits()){
                CurrentTetromino.rotateCounterClockwise();

            }
        }

        // When moving tetrominos to either side, we first move it, then we check if it fits or not. 
        // If it fits it stays, if it does not fit it goes back where it was
        public void moveTetrominoLeft(){
            CurrentTetromino.Move(0, -1);

            if (!tetrominoFits()){
                CurrentTetromino.Move(0, 1);
            }
        }

        // Same happens when moving tetromino to the right as when we move it to the left
        public void moveTetrominoRight(){
            CurrentTetromino.Move(0, 1);

            if (!tetrominoFits()){
                CurrentTetromino.Move(0, -1);
            }
        }

        // Checks if the game has ended (if the stack reached the top)
        private bool isGameOver(){
            return !(GameGrid.isRowEmpty(0) && GameGrid.isRowEmpty(1));
        }

        // Makes all the neccessary checks and places the tetromino if it needs to be
        private void placeTetromino(){
            foreach (Position p in CurrentTetromino.TilePositions()){
                GameGrid[p.Row, p.Column] = CurrentTetromino.Id;
            }

            // This part takes care of the scoring
            int prevScore = Score;

            Score += GameGrid.clearFullRows(); // Check for any rows that need to be cleared

            if (Score - prevScore == 4){
                Score += 4;
            }
            
            if (isGameOver()){
                GameOver = true;
                if (Score > HighestScore){ //Checks for highscore when the game is over

                    HighestScore = Score;
                    SaveHighestScore();
                }
            } else {
                CurrentTetromino = TetrominoQueue.getAndUpdate();
            }
        }

        // Same logic here as in moveRight and moveLeft
        public void MoveTetrominoDown(){
            CurrentTetromino.Move(1, 0);

            if (!tetrominoFits()){
                CurrentTetromino.Move(-1, 0);
                placeTetromino();
            }
        }

        // The following two methods store and retrieve the highscore which is localy saved in a txt file
        private void LoadHighestScore() {
            string filePath = "HighestScore.txt";

            if (File.Exists(filePath)) {
                string scoreText = File.ReadAllText(filePath);
                if (int.TryParse(scoreText, out int savedScore)) {
                    HighestScore = savedScore;
                }
            } else {
                HighestScore = 0; // Default highest score if the file doesn't exist
            }
        }

        private void SaveHighestScore() {
            string filePath = "HighestScore.txt";

            File.WriteAllText(filePath, HighestScore.ToString());
        }

    }

}
