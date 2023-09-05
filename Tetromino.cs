namespace Tetris{
    public abstract class Tetromino{
        protected abstract Position[][] Tiles {get;} // This array will contain the tile locations in all 4 rotational states that every tetromino has
        protected abstract Position StartPosition {get;} // Dictates where the tetromino spawn on the grid
        public abstract int Id {get;} // This is used to identify the tetromino

        private int rotationState;
        private Position offset;

        public Tetromino(){
            offset = new Position(StartPosition.Row, StartPosition.Column);
        }

        // The method uses yield return to return the positions on a grid which are occupied by the tetromino and it also takes into account the rotationstate
        public IEnumerable<Position> TilePositions(){
            foreach(Position p in Tiles[rotationState]){
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        // The following two alter the rotation state
        public void rotateClockwise(){
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        public void rotateCounterClockwise(){
            if (rotationState == 0){

                rotationState = Tiles.Length - 1;
            }else{
                rotationState--;
            }
        }

        // Method move later allows us to move a tetromino in any direction
        // At this time we see that is changes the offset of a tetormino on a grid
        public void Move(int rows, int columns){
            offset.Row += rows;
            offset.Column += columns;

        }

        // Resets the state of tetromino, it is used when the new tetromino starts coming down
        public void reset(){
            rotationState = 0;
            offset.Row = StartPosition.Row;
            offset.Column = StartPosition.Column;
        }
    }


    public class TetrominoQueue{

        private readonly Tetromino[] tetrominoes = new Tetromino[]{ // An array of tetrominoes

            new ITetromino(),
            new TTetromino(),
            new ZTetromino(),
            new STetromino(),
            new OTetromino(),
            new JTetromino(),
            new LTetromino()

        };

        private readonly Random random = new Random();

        public Tetromino NextTetromino { get; private set;}

        public TetrominoQueue(){
            NextTetromino = randomTetromino();
        }

        // Method randomly picks a tetromino out of the Tetromino array we have created above
        private Tetromino randomTetromino(){
            return tetrominoes[random.Next(tetrominoes.Length)];
        }

        // The method retrieves the next tetromino randomly using the method randomTetromino
        public Tetromino getAndUpdate(){
            Tetromino tetromino = NextTetromino;

            do{
                NextTetromino = randomTetromino();
            } while (tetromino.Id == NextTetromino.Id);

            return tetromino;

        }
        
    }

    // Create classes for each tetromino
    public class ITetromino : Tetromino
    {
        
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(1,0), new(1,1), new(1,2), new(1,3) },
            new Position[] { new(0,2), new(1,2), new(2,2), new(3,2) },
            new Position[] { new(2,0), new(2,1), new(2,2), new(2,3) },
            new Position[] { new(0,1), new(1,1), new(2,1), new(3,1) }
        };
        public override int Id => 1; // This is how we indentify which tetromino it is
        protected override Position StartPosition => new Position(-1, 3); // Start position of the tetromino
        protected override Position[][] Tiles => tiles;
    }

    // It is same for every tetromino as it is for ITetromino
    public class JTetromino : Tetromino
    {
        public override int Id => 2;

        protected override Position StartPosition => new(0, 3);

        protected override Position[][] Tiles => new Position[][] 
        {
            new Position[] {new(0,0), new(1,0), new(1,1), new(1,2)},
            new Position[] {new(0,1), new(0,2), new(1,1), new(2,1)},
            new Position[] {new(1,0), new(1,1), new(1,2), new(2,2)},
            new Position[] {new(0,1), new(1,1), new(2,1), new(2,0)}
        };
    }


    public class LTetromino : Tetromino
    {
        public override int Id => 3;

        protected override Position StartPosition => new(0, 3);

        protected override Position[][] Tiles => new Position[][] 
        {
            new Position[] {new(0,2), new(1,0), new(1,1), new(1,2)},
            new Position[] {new(0,1), new(1,1), new(2,1), new(2,2)},
            new Position[] {new(1,0), new(1,1), new(1,2), new(2,0)},
            new Position[] {new(0,0), new(0,1), new(1,1), new(2,1)}
        };
    }

    public class OTetromino : Tetromino
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,0), new(0,1), new(1,0), new(1,1) }
        };

        public override int Id => 4;
        protected override Position StartPosition => new Position(0, 4);
        protected override Position[][] Tiles => tiles;
    }

    public class STetromino : Tetromino
    {
        public override int Id => 5;

        protected override Position StartPosition => new(0, 3);

        protected override Position[][] Tiles => new Position[][] 
        {
            new Position[] { new(0,1), new(0,2), new(1,0), new(1,1) },
            new Position[] { new(0,1), new(1,1), new(1,2), new(2,2) },
            new Position[] { new(1,1), new(1,2), new(2,0), new(2,1) },
            new Position[] { new(0,0), new(1,0), new(1,1), new(2,1) }
        };
    }

    public class TTetromino : Tetromino
    {
        public override int Id => 6;

        protected override Position StartPosition => new(0, 3);

        protected override Position[][] Tiles => new Position[][] 
        {
            new Position[] {new(0,1), new(1,0), new(1,1), new(1,2)},
            new Position[] {new(0,1), new(1,1), new(1,2), new(2,1)},
            new Position[] {new(1,0), new(1,1), new(1,2), new(2,1)},
            new Position[] {new(0,1), new(1,0), new(1,1), new(2,1)}
        };
    }

    public class ZTetromino : Tetromino
    {
        public override int Id => 7;

        protected override Position StartPosition => new(0, 3);

        protected override Position[][] Tiles => new Position[][] 
        {
            new Position[] {new(0,0), new(0,1), new(1,1), new(1,2)},
            new Position[] {new(0,2), new(1,1), new(1,2), new(2,1)},
            new Position[] {new(1,0), new(1,1), new(2,1), new(2,2)},
            new Position[] {new(0,1), new(1,0), new(1,1), new(2,0)}
        };
    }
}

 
