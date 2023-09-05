namespace Tetris {
    
    public class GameGrid {
        private readonly int[,] grid;
        public int Rows {get;}
        public int Columns {get;}

        public int this[int r , int c]{
            get => grid[r,c];
            set => grid[r,c] = value;
        }

        public GameGrid(int rows, int columns){ // Grid is initialized by the number of rows and columns
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }
        
        // Checks if the block is empty or not
        public bool isEmpty(int r, int c){
            return r>= 0 && r < Rows && c >= 0 && c < Columns && grid[r,c] == 0;
        }

        // Checks if row is full
        public bool isRowFull(int r){
            for (int c = 0; c < Columns; c++){ // if every member of the row we are checking is not a 0 then it is full
                if (grid[r,c] == 0){
                    return false;
                }
            }

            return true;
        }

        // Checks if row is empty
        public bool isRowEmpty(int r){ 
            for (int c = 0; c < Columns; c++){ // If every member of the row is 0 then it is empty
                if (grid[r,c] != 0){
                    return false;
                }
            }

            return true;
        }

        // The method moves the row down one row
        private void moveRowDown(int r, int nRows){
            for (int c = 0; c < Columns; c++){ // We put the id's of the row we are working with below it and placing 0's on that row so that we know it is empty
                grid[r + nRows, c] = grid[r,c];
                grid[r,c] = 0;
            }
        }
        
        // The method clears full rows
        public int clearFullRows(){
            int cleared = 0; // Keep track of how many rows we have cleared because then we have to move down the row on top of it accordingly

            for (int r = Rows - 1; r >= 0; r-- ){
                if (isRowFull(r)) // If row is full then it is cleared
                {

                    for (int c = 0; c < Columns; c++) // Clear one row
                    {
                        grid[r,c] = 0;
                    }

                    cleared++;
                } else if (cleared > 0){ // If it is not full and at least one row has been cleared below the row we are checking now then we move it down by int cleared
                    moveRowDown(r,cleared);
                }
                
            }

            return cleared; // returns number of cleared rows as it is needed for scoring
        }
    }
}