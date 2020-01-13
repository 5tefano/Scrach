using System;

public class Program
{
    public static void Main()
    {
        var connectFourConsoleGame = new ConnectThreeConsoleGame();
        connectFourConsoleGame.Start();
    }

    public class ConnectThreeConsoleGame
    {
        int columns = 7;
        int rows = 6;
        int connectionsToWin = 4;
        bool _gameInProgress { get; set; }
        GameEngine _gameEngine { get; set; }

        public ConnectThreeConsoleGame()
        {
            var player1Color = Color.Red;           
            var creationRequest = new GameEngineCreationRequest(columns, rows, connectionsToWin, player1Color);
            _gameEngine = new GameEngine(creationRequest);
        }

        public void Start()
        {
            _gameInProgress = true;
            Console.Clear();
            
            while(_gameEngine.Status == GameStatus.GameInProgress)
            {
                printScreen();
                promptPlayerToSelectAColumn();            
            }

            printScreen();
        }

        private void printScreen()
        {
            Console.Clear();            
            printGameBoard();
            printPlayersInfo();
            Console.WriteLine();
        }

        private static string getColorAsString(Color color)
        {
            if(color == Color.Red)
            {
                return "Red";
            }
            else 
            {
                return "Yellow";
            }
        }

        #region Prompt Methods

        private void promptPlayerToSelectAColumn()
        {
            Console.Write("\nPlease select a column to drop your game piece. > ");
            var input = Console.ReadLine();
            
            if(string.IsNullOrEmpty(input))
            {
                return;
            }

            var inputAsInt = 0;
            
            try {
                inputAsInt = Convert.ToInt32(input);
                _gameEngine.AddGamePiece(inputAsInt);
            } 
            catch {                
                return;
            }
        }

        #endregion

        #region Print Methods
        
        private void printPlayersInfo()
        {
            Console.Write("\nPLAYER 1: " + getColorAsString(_gameEngine.Player1.Color));
            if(_gameEngine.IsPlayer1sTurn)
            {
                Console.Write("  \t<< YOUR TURN");  
            }
            
            Console.Write("\nPLAYER 2: " + getColorAsString(_gameEngine.Player2.Color));
            if(!_gameEngine.IsPlayer1sTurn)
            {
                Console.Write("  \t<< YOUR TURN");  
            }
        }

        private void printGameBoard()
        {
            var rowCount = _gameEngine.GameBoard.NumberOfRows;
            var columnCount = _gameEngine.GameBoard.NumberOfColumns;

            for(var i = 1; i <= _gameEngine.GameBoard.NumberOfColumns; i++)
            {
                Console.Write("    " + i + "   \t");    
            }
            Console.WriteLine();
            //Console.Write("    1   \t    2    \t    3    \t    4    \t    5   \t    6   \t    7   \n");
            //Console.Write(" --------\t --------\t --------\t --------\t --------\t --------\t --------\n");
            for(var i = rowCount - 1; i >= 0; i--)
            {
                for(var j = 0; j < columnCount; j++)
                {
                    var gameBoardSlot = _gameEngine.GameBoard.Board[j, i];
                    printgameBoardSlot(gameBoardSlot);
                }
                Console.Write("\n");
            }
        }

        private static void printgameBoardSlot(GameBoardSlot gameBoardSlot)
        {
            if(gameBoardSlot.IsOccupied)
            {
                if(gameBoardSlot.OccupiedBy.Color == Color.Red)
                {
                    Console.Write("{  RED   }\t");
                }
                else
                {
                    Console.Write("{ YELLOW }\t");
                }
            }
            else
            {
                Console.Write("{        }\t");
            }
        }

        #endregion
    }

    public class GameEngine
    {
        public GameStatus Status { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public GameBoard GameBoard { get; private set; }
        public bool IsPlayer1sTurn { get; private set; }
        private int _numberOfConnectionsToWin { get; set; }

        public GameEngine(GameEngineCreationRequest creationRequest)
        {
            GameBoard = new GameBoard(creationRequest.NumberOfColumns, creationRequest.NumberOfColumns);
            Status = GameStatus.GameInProgress;
            _numberOfConnectionsToWin = creationRequest.NumberOfGamePiecesToConnect;
            IsPlayer1sTurn = creationRequest.Player1GoesFirst;
            Player1 = new Player(creationRequest.Player1Color);            
            if(creationRequest.Player1Color == Color.Red)
            {
                Player2 = new Player(Color.Yellow);
            }
            else
            {
                Player2 = new Player(Color.Red);
            }
        }
        
        public AddGamePieceResponse AddGamePiece(int column)
        {
            if(Status == GameStatus.GameInProgress)
            {
                var response = GameBoard.AddGamePiece(column, getCurrentPlayersGamePiece());
                if(response.IsValidEntry)
                {
                    validateWinningMove();
                    if(Status == GameStatus.GameInProgress)
                    {
                        togglePlayerTurn();
                    }
                }
                return response;
            }
            else
            {
                return new AddGamePieceResponse(AddGamePieceErrorType.GameOver);
            }
        }       

        private GamePiece getCurrentPlayersGamePiece()
        {
            if(IsPlayer1sTurn)
            {
                return new GamePiece(Player1.Color);
            }
            else
            {
                return new GamePiece(Player2.Color);
            }
        }
        
        private void validateWinningMove()
        {
            if(detectConnectionsInColumns() || detectConnectionsInRows())
            {
                if(IsPlayer1sTurn)
                {
                    Status = GameStatus.Player1HasWon;
                }
                else
                {
                    Status = GameStatus.Player2HasWon;
                }
            }
        }

        private bool detectConnectionsInColumns()
        {
            for(var i = 0; i < GameBoard.NumberOfColumns; i++)
            {
                var counter = 0;
                GamePiece lastGamePiece = null;
                for(var j = 0; j < GameBoard.NumberOfRows; j++)
                {
                    var currentGameSlot = GameBoard.Board[i,j];
                    if(!currentGameSlot.IsOccupied)
                    {
                        break;
                    }

                    var currentGamePiece = currentGameSlot.OccupiedBy;                    
                    if(lastGamePiece != null && currentGamePiece.Color == lastGamePiece.Color)
                    {
                        counter++;
                        if(counter == _numberOfConnectionsToWin - 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        counter = 0;
                    }
                    lastGamePiece = currentGamePiece;
                }
            }
            return false;
        }

        private bool detectConnectionsInRows()
        {
            for(var r = 0; r < GameBoard.NumberOfRows; r++)
            {
                var counter = 0;
                GamePiece lastGamePiece = null;
                for(var c = 0; c < GameBoard.NumberOfColumns; c++)
                {
                    var currentGameSlot = GameBoard.Board[c,r];
                    if(!currentGameSlot.IsOccupied)
                    {
                        break;
                    }

                    var currentGamePiece = currentGameSlot.OccupiedBy;                    
                    if(lastGamePiece != null && currentGamePiece.Color == lastGamePiece.Color)
                    {
                        counter++;
                        if(counter == _numberOfConnectionsToWin - 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        counter = 0;
                    }
                    lastGamePiece = currentGamePiece;
                }
            }
            return false;
        }

        private void togglePlayerTurn()
        {
            IsPlayer1sTurn = !IsPlayer1sTurn;
        }
    }

    public class GameEngineCreationRequest
    {
        public int NumberOfColumns { get; private set; }
        public int NumberOfRows { get; private set; }
        public int NumberOfGamePiecesToConnect { get; private set; }
        public Color Player1Color { get; private set; }
        public bool Player1GoesFirst { get; private set; }

        public GameEngineCreationRequest(int numberOfColumns, int numberOfRows, int numberOfGamePiecesToConnect, Color player1Color, bool player1GoesFirst = true)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRows = numberOfRows;
            NumberOfGamePiecesToConnect = numberOfGamePiecesToConnect;
            Player1Color = player1Color;
            Player1GoesFirst = player1GoesFirst;
        }
    }

    public class GameBoard
    {
        public GameBoardSlot[,] Board { get; private set; }
        public int NumberOfColumns { get; private set; }
        public int NumberOfRows { get; private set; }

        public GameBoard(int numberOfColumns, int numberOfRows)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRows = numberOfRows;
            validateCreationParams();
            Board = new GameBoardSlot[NumberOfColumns, NumberOfRows];
            bootstrapBoard();
        }
        
        public AddGamePieceResponse AddGamePiece(int column, GamePiece gamePiece)
        {
            if(column >= 1 && column <= NumberOfColumns)
            {
                for(var i = 0; i < NumberOfRows; i++)
                {
                    var gamePieceSlot = Board[column - 1, i];
                    if(!gamePieceSlot.IsOccupied)
                    {
                        gamePieceSlot.AddGamePiece(gamePiece);
                        return new AddGamePieceResponse();                        
                    }
                }
                return new AddGamePieceResponse(AddGamePieceErrorType.ColumnFull);
            }

            return new AddGamePieceResponse(AddGamePieceErrorType.InvalidColumn);
        }

        public void ResetBoard()
        {
            bootstrapBoard();
        }

        private void bootstrapBoard()
        {
            for(var i = 0; i < NumberOfColumns; i++)
            {
                for(var j = 0; j < NumberOfRows; j++)
                {
                    Board[i,j] = new GameBoardSlot();
                }
            }
        }

        private void validateCreationParams()
        {
            if(NumberOfColumns < 2 || NumberOfColumns > 20)
            {
                throw new InvalidOperationException("NumberOfColumns must be between 2 and 20");
            }

            if(NumberOfRows < 2 || NumberOfRows > 20)
            {
                throw new InvalidOperationException("NumberOfRows must be between 2 and 20");
            }
        }
    }
    
    public class AddGamePieceResponse
    {
        public bool IsValidEntry { get; private set; }
        public AddGamePieceErrorType ErrorType { get; private set; }
        
        public AddGamePieceResponse()
        {
            IsValidEntry = true;
        }

        public AddGamePieceResponse(AddGamePieceErrorType errorType)
        {
            IsValidEntry = false;
            ErrorType = errorType;
        }
    }

    public enum AddGamePieceErrorType
    {            
        InvalidColumn,
        ColumnFull,
        GameOver
    }

    public class Player 
    {
        public Color Color { get; private set; }
        public Player(Color color)
        {
            Color = color;
        }
    }    

    public class GameBoardSlot
    {
        public bool IsOccupied { get; private set; }
        
        public GamePiece OccupiedBy { get; private set; }

        public GameBoardSlot()
        {
            IsOccupied = false;
        }

        public void AddGamePiece(GamePiece gamePiece)
        {
            if(OccupiedBy == null)
            {
                OccupiedBy = gamePiece;
                IsOccupied = true;
            }
        }
    }    

    public enum Color
    {
        Red,
        Yellow
    }

    public class GamePiece
    {
        public Color Color { get; private set; }       

        public GamePiece(Color color)
        {
            Color = color;
        }
    }

    public enum GameStatus
    {
        GameInProgress,
        Player1HasWon,
        Player2HasWon,
        TieGame
    }
}