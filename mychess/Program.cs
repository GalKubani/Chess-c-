using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary;
namespace mychess
{
    class ChessGameLauncher
    {
        static void Main(string[] args)
        {
            new ChessGame().Play();
        }
    }
    class ChessGame
    {
        public void Play()
        {
            int turnCount = 0, boardsInserted = 1, turnsNothingAttacked = 0, totalpieces = 32, currentpieces = totalpieces;
            ChessPiece[,] board = new ChessPiece[8, 8];
            ChessPiece[][][] allBoardsUntilChange;
            allBoardsUntilChange = new ChessPiece[50][][];
            for (int z = 0; z < 50; z++)
                allBoardsUntilChange[z] = new ChessPiece[8][];
            for (int z = 0; z < 50; z++)
                for (int i = 0; i < 8; i++)
                    allBoardsUntilChange[z][i] = new ChessPiece[8];
            resetBoard(board);
            copyBoard(board, allBoardsUntilChange[0]);
            bool checkMate = false, threatOnKing = false, Draw = false, pawnJustMoved = false;
            Console.WriteLine("Welcome to chess, I hope you enjoy the game ");
            while (!checkMate && turnsNothingAttacked < 50 && totalpieces >= 2 && !Draw)
            {
                if (turnCount == 0 || turnCount % 2 == 0)
                    Console.WriteLine("Its the WHITE players's turn, please insert the desired piece to move, and the location to move it to ");
                else
                    Console.WriteLine("Its the BLACK players's turn, please insert the desired piece to move, and the location to move it to ");
                string input = Console.ReadLine();
                didPawnJustMadeDoubleStep(board, turnCount);
                playTurn(input, board, turnCount, ref pawnJustMoved);
                turnCount++;
                copyBoard(board, allBoardsUntilChange[boardsInserted]);
                boardsInserted++;
                if (turnsNothingAttacked == 0)
                    for (int z = 0; z < boardsInserted - 1; z++) // if board has changed indefinetly
                    {
                        deleteAllBoards(allBoardsUntilChange[z]);
                        boardsInserted = 1;
                        copyBoard(board, allBoardsUntilChange[0]);
                    }
                if (doesBoardRepeats3Times(allBoardsUntilChange, boardsInserted) || isPatDraw(board, turnCount))
                {
                    Console.WriteLine("The game ended because "+ (isPatDraw(board, turnCount)?"of a PAT draw":"the board repeated itself 3 times"));
                    printBoard(board);
                    Draw = true;
                    continue;
                }
                threatOnKing = isKingThreatened(board, turnCount, false);
                if (threatOnKing)
                    checkMate = isGameOver(board, turnCount);
                if (checkMate)
                {
                    printBoard(board);
                    Console.WriteLine("CheckMate!!! congratulations for the " + (turnCount % 2 != 0 ? "white" : "black") + " player");
                    continue;
                }
                if (threatOnKing)
                    Console.WriteLine("Check! guard your king be careful " + (turnCount % 2 != 0 ? "black" : "white") + " player");
                currentpieces = printBoard(board);
                if (totalpieces == currentpieces)
                    turnsNothingAttacked++;
                else
                    turnsNothingAttacked = 0;
                if (pawnJustMoved)
                    turnsNothingAttacked = 0;
                totalpieces = currentpieces;
                if (turnsNothingAttacked == 50|| currentpieces <= 2)
                    Console.WriteLine("The game ended in a draw, "+( turnsNothingAttacked == 50 ? "50 turns without an attack" : "not enough pieces for any possible win"));
            }
        }
        void deleteAllBoards(ChessPiece[][] allBoardsUntilChange)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    allBoardsUntilChange[i][j] = null;
                }
        }
        void copyBoard(ChessPiece[,] board, ChessPiece[][] allBoardsUntilChange)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] is Rook)
                    {
                        allBoardsUntilChange[i][j] = new Rook(board[i, j].getWhite(), ((Rook)board[i, j]).GetMadeFirstStep(), board[i, j].getId());
                        continue;
                    }
                    if (board[i, j] is King)
                    {
                        allBoardsUntilChange[i][j] = new King(board[i, j].getWhite(), ((King)board[i, j]).GetMadeFirstStep(), board[i, j].getId());
                        continue;
                    }
                    allBoardsUntilChange[i][j] = board[i, j];
                }
        }
        void resetBoard(ChessPiece[,] board)
        {
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = new Pawn(true, i + 10);
                board[6, i] = new Pawn(false, i + 60);
                board[2, i] = null;
                board[3, i] = null;
                board[4, i] = null;
                board[5, i] = null;
            }
            board[0, 0] = new Rook(true, 0);
            board[0, 7] = new Rook(true, 7);
            board[7, 0] = new Rook(false, 70);
            board[7, 7] = new Rook(false, 77);
            board[0, 1] = new Knight(true, 1);
            board[0, 6] = new Knight(true, 6);
            board[7, 1] = new Knight(false, 71);
            board[7, 6] = new Knight(false, 76);
            board[0, 2] = new Bishop(true, 2);
            board[0, 5] = new Bishop(true, 5);
            board[7, 2] = new Bishop(false, 72);
            board[7, 5] = new Bishop(false, 75);
            board[0, 3] = new Queen(true, 3);
            board[0, 4] = new King(true, 4);
            board[7, 3] = new Queen(false, 73);
            board[7, 4] = new King(false, 74);
            printBoard(board);
        }
        void didPawnJustMadeDoubleStep(ChessPiece[,] board, int turnCount)
        {
            bool result = false;         
            for (int i = 1; i < 6 && result == false; i++)
                for (int j = 0; j < 7 && result == false; j++)
                    if (board[i, j] != null)
                        if (board[i, j] is Pawn)
                        {
                            if (!((Pawn)board[i, j]).getWhite() && turnCount % 2 != 0 || ((Pawn)board[i, j]).getWhite() && turnCount % 2 == 0)
                            {
                                if (((Pawn)board[i, j]).GetJustMadeDoubleStep())
                                {
                                    ((Pawn)board[i, j]).SetJustMadeDoubleStep(false);
                                    result = true;
                                }
                            }
                        }
        }
        bool checkThreatForPieceInserted(ChessPiece[,] board, int row, int column, int turnCount, bool checkingNull)
        {
            bool aThreat = false;
            isThereAThreat(board, row, column, turnCount, checkingNull, true, ref aThreat);
            return aThreat;
        }
        bool doesBoardRepeats3Times(ChessPiece[][][] allBoardsUntilChange, int boardsInserted)
        {
            int counter = 0;
            bool boardIsntTheSame = false; ;
            for (int z = boardsInserted - 3; z >= 0 ; z -= 2)
            {
                boardIsntTheSame = false;
                for (int i = 0; i < 8 && !boardIsntTheSame; i++)
                {
                    for (int j = 0; j < 8 && !boardIsntTheSame; j++)
                    {
                        if (allBoardsUntilChange[boardsInserted - 1][i][j] != null && null != allBoardsUntilChange[z][i][j])// makes sure they are both pieces
                        {
                            if (allBoardsUntilChange[boardsInserted - 1][i][j].getId() == allBoardsUntilChange[z][i][j].getId())// if they are the same piece
                            {
                                if (allBoardsUntilChange[boardsInserted - 1][i][j].getWhite() == allBoardsUntilChange[z][i][j].getWhite())// for the same army
                                {
                                    if (allBoardsUntilChange[boardsInserted - 1][i][j] is Rook && allBoardsUntilChange[z][i][j] is Rook)// in case for rooks
                                        if (((Rook)allBoardsUntilChange[boardsInserted - 1][i][j]).GetMadeFirstStep() != ((Rook)allBoardsUntilChange[z][i][j]).GetMadeFirstStep())
                                            boardIsntTheSame = true;
                                    if (allBoardsUntilChange[boardsInserted - 1][i][j] is King && allBoardsUntilChange[z][i][j] is King)
                                        if (((King)allBoardsUntilChange[boardsInserted - 1][i][j]).GetMadeFirstStep() != ((King)allBoardsUntilChange[z][i][j]).GetMadeFirstStep())
                                            boardIsntTheSame = true;
                                }
                                else
                                    boardIsntTheSame = true;
                            }
                            else
                                boardIsntTheSame = true;
                        }
                        else if (allBoardsUntilChange[boardsInserted - 1][i][j] == null && null == allBoardsUntilChange[z][i][j]) { }
                        else
                            boardIsntTheSame = true;
                    }
                    if (i == 7 && !boardIsntTheSame)
                        counter++;
                }
            }
            return counter >= 3 ? true : false;
        }
        bool isKingThreatened(ChessPiece[,] board, int turnCount, bool checkingMyKing)
        {
            int[] kingRowAndColumn = new int[2];
            kingRowAndColumn = findTheKing(board, turnCount, checkingMyKing);// locates the king that we are trying to threat
            bool aThreat = checkThreatForPieceInserted(board, kingRowAndColumn[0], kingRowAndColumn[1], turnCount, false);
            return aThreat;
        }
        bool checkPlayerForPat(ChessPiece[,] board, int turnCount, ref int row, ref int column)
        {
            for (int indexR = 0; indexR < 8; indexR++)
                for (int indexC = 0; indexC < 8; indexC++)
                    if (!((IAction)board[row, column]).IsMoveLegal(board, row, column, indexR, indexC, turnCount, true))
                        return false;
            return true;
        }
        bool isPatDraw(ChessPiece[,] board, int turnCount)
        {
            bool checkingWhite = turnCount % 2 == 0 ? true : false;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (board[i, j] != null)
                        if (board[i, j].getWhite() && checkingWhite || !board[i, j].getWhite() && !checkingWhite) 
                            if (!checkPlayerForPat(board, turnCount, ref i, ref j))
                                return false;
            return true;
        }
        bool isGameOver(ChessPiece[,] board, int turnCount)
        {
            if (canKingCanSaveHimselfFromChess(board, turnCount)) // first if the king can save himself by moving
                return false;
            int[] threatRowandCollum = findTheThreat(board, turnCount, false);// second if there is a direct threat on the piece attacking the king
            int threatRow = threatRowandCollum[0], threatCollum = threatRowandCollum[1], kingRow = threatRowandCollum[2], kingCollum = threatRowandCollum[3];
            bool check = checkThreatForPieceInserted(board, threatRow, threatCollum, turnCount, false);
            if (check)
                return false;
            return canBlockThreat(board, threatRow, threatCollum, kingRow, kingCollum, turnCount) ? false : true;
        }
        bool canBlockThreat(ChessPiece[,] board, int threatR, int threatC, int kingR, int kingC, int turnCount)
        {  // here we check the nulls between the king and his threat, if any of them can be threatened by a piece of the king, its not checkmate
            if (threatR - 1 == kingR && threatC - 1 == kingC|| threatR + 1 == kingR && threatC + 1 == kingC|| threatR + 1 == kingR && threatC - 1 == kingC||
                threatR - 1 == kingR && threatC + 1 == kingC|| threatR - 1 == kingR && threatC == kingC|| threatR + 1 == kingR && threatC == kingC||
                threatR == kingR && threatC - 1 == kingC|| threatR == kingR && threatC + 1 == kingC)
                return false;
            bool canBlock = false;
            if (threatR == kingR && threatC < kingC) // incase threat is left of the king
                for (int j = threatC + 1; j < kingC && !canBlock; j++)
                    if (checkThreatForPieceInserted(board, kingR, j, turnCount, true))
                        canBlock = true;
            if (threatR == kingR && threatC > kingC) // incase threat is right of the king
                for (int j = kingC + 1; j < threatC && !canBlock; j++)
                    if (checkThreatForPieceInserted(board, kingR, j, turnCount, true))
                        canBlock = true;
            if (threatR > kingR && threatC == kingC) // incase threat is below the king
                for (int i = kingR + 1; i < threatR && !canBlock; i++)
                    if (checkThreatForPieceInserted(board, i, kingC, turnCount, true))
                        canBlock = true;
            if (threatR < kingR && threatC == kingC) // incase threat is above the king
                for (int i = threatR + 1; i < kingR && !canBlock; i++)
                    if (checkThreatForPieceInserted(board, i, kingC, turnCount, true))
                        canBlock = true;
            if (threatR > kingR && threatC > kingC) // incase threat is down right diagnal
                for (int i = kingR + 1, j = kingC + 1; i < threatR && !canBlock; i++, j++)
                    if (checkThreatForPieceInserted(board, i, j, turnCount, true))
                        canBlock = true;          
            // pawn can only block threat that are diagnal 
            if (threatR < kingR && threatC < kingC) // incase threat is up left diagnal
                for (int i = threatR + 1, j = threatC + 1; i < kingR && !canBlock; i++, j++)
                    if (checkThreatForPieceInserted(board, i, j, turnCount, true))
                        canBlock = true;
            if (threatR < kingR && threatC > kingC) // incase threat is up right diagnal
                for (int i = threatR + 1, j = threatC - 1; i < kingR && !canBlock; i++, j--)
                    if (checkThreatForPieceInserted(board, i, j, turnCount, true))
                        canBlock = true;
            if (threatR > kingR && threatC < kingC) // incase threat is down left diagnal
                for (int i = threatR - 1, j = threatC + 1; i > kingR && !canBlock; i--, j++)
                    if (checkThreatForPieceInserted(board, i, j, turnCount, true))
                        canBlock = true;
            return canBlock ? true : false;
        }
        bool canKingCanSaveHimselfFromChess(ChessPiece[,] board, int turnCount)
        {  // this will check if the king can save himself, counting the possible escapes, more than 1 means its not checkmate
            int i, j = 0, possibleEscapes = 0;
            int[] kingRowAndColumn = new int[2];
            kingRowAndColumn = findTheKing(board, turnCount, false);
            i = kingRowAndColumn[0];
            j = kingRowAndColumn[1];
            ChessPiece placeholderForMovement = new King(board[i, j].getWhite(), board[i, j].getId());
            board[i, j] = null;
            int newR = i, newC = j;
            if (newR < 1)
                newR = 1;
            if (newC < 1)
                newC = 1;
            for (int indexR = newR - 1; indexR < newR + 1 && possibleEscapes == 0; indexR++)
                for (int indexC = newC - 1; indexC < newC + 1 && possibleEscapes == 0; indexC++)
                    if (board[indexR, indexC] == null)
                    {
                        board[indexR, indexC] = placeholderForMovement;
                        if (!isKingThreatened(board, turnCount, false))
                        {
                            possibleEscapes++;
                        }
                        board[indexR, indexC] = null;
                    }
            board[i, j] = placeholderForMovement;
            return possibleEscapes == 0 ? false:true ;
        }
        bool doesMoveLeaveKingUnderThreat(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount)
        {
            ChessPiece placeholderForMovement = null;
            if (board[newRow, newColumn] is Queen)
                placeholderForMovement = new Queen(board[newRow, newColumn].getWhite(), board[newRow, newColumn].getId());
            else if (board[newRow, newColumn] is Bishop)
                placeholderForMovement = new Bishop(board[newRow, newColumn].getWhite(), board[newRow, newColumn].getId());
            else if (board[newRow, newColumn] is Rook)
                placeholderForMovement = new Rook(board[newRow, newColumn].getWhite(), board[newRow, newColumn].getId());
            else if (board[newRow, newColumn] is Knight)
                placeholderForMovement = new Knight(board[newRow, newColumn].getWhite(), board[newRow, newColumn].getId());
            else if (board[newRow, newColumn] is Pawn)
                placeholderForMovement = new Pawn(board[newRow, newColumn].getWhite(), board[newRow, newColumn].getId());
            if (board[newRow, newColumn] is null && board[oldRow, oldColumn] is King) // in case the user moves his king
            { // in case the slot is null, temporary place the piece there just to check if king is under threat
                board[newRow, newColumn] = board[oldRow, oldColumn];// this will not guarentee that the move will be made, in case it isnt legal for the piece that will be checked later
                placeholderForMovement = new King(board[oldRow, oldColumn].getWhite(), board[oldRow, oldColumn].getId());
                board[oldRow, oldColumn] = null;
                if (oldColumn + 2 == newColumn || oldColumn - 2 == newColumn)
                {    // case of hazraha
                    if (!isKingThreatened(board, turnCount, false))
                    {
                        if (board[newRow, newColumn - 1] == null)
                        {
                            board[newRow, newColumn - 1] = placeholderForMovement;
                            board[newRow, newColumn] = null;
                            bool isKingUnderThreat = isKingThreatened(board, turnCount, false);
                            board[newRow, newColumn - 1] = null;
                            board[oldRow, oldColumn] = placeholderForMovement;
                            if (isKingUnderThreat) // here we need to check if the player making the move leaves his own king under threat, if he does the move is blocked
                                return true;
                            else
                                return false;
                        }
                    }
                    else
                    {
                        board[oldRow, oldColumn] = new King(turnCount % 2 == 0 ? true : false, board[newRow, newColumn].getId());
                        board[newRow, newColumn] = null;
                        return true;
                    }
                }
                bool isPieceUnderThreat = checkThreatForPieceInserted(board, newRow, newColumn, turnCount, false);
                board[oldRow, oldColumn] = new King(turnCount % 2 == 0 ? true : false, true, board[newRow, newColumn].getId());
                board[newRow, newColumn] = null;
                return isPieceUnderThreat ? true : false; // here we need to check if the player making the move leaves his own king under threat, if he does the move is blocked
            }
            else if (board[newRow, newColumn] != null)
            {
                if (board[newRow, newColumn].getWhite() != board[oldRow, oldColumn].getWhite()) // only continues if they arent the same color
                    if (true == ((IAction)board[oldRow, oldColumn]).IsMoveLegal(board, oldRow, oldColumn, newRow, newColumn, turnCount, true)) //checking if piece can make move
                    {// move wont happen but is possible
                        ((IAction)board[oldRow, oldColumn]).MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                        bool isKingUnderThreat = isKingThreatened(board, turnCount, false);
                        board[oldRow, oldColumn] = board[newRow, newColumn];
                        board[newRow, newColumn] = placeholderForMovement;
                        return isKingUnderThreat ? true : false;
                    }
            }
            else if (board[newRow, newColumn] == null)
            {
                ((IAction)board[oldRow, oldColumn]).MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                bool isKingUnderThreat = isKingThreatened(board, turnCount, false);
                board[oldRow, oldColumn] = board[newRow, newColumn];
                board[newRow, newColumn] = null;
                return isKingUnderThreat ? true : false;// here we need to check if the player making the move leaves his own king under threat, if he does the move is blocked
            }
            else
                return false;
            return true;
        }
        bool isMoveValid(ref string input, ChessPiece[,] board, int turnCount, ref bool pawnJustMoved, ref bool inputcheck, int[] newAndOldRowAndCollums)
        {
            if (!doesMoveLeaveKingUnderThreat(board, newAndOldRowAndCollums[0], newAndOldRowAndCollums[1], newAndOldRowAndCollums[2], newAndOldRowAndCollums[3], turnCount)) // need to check if move will threat the king
            {// only if the func above return false, it means the move is possible without leaving king to direct threat
                if (!((IAction)board[newAndOldRowAndCollums[0], newAndOldRowAndCollums[1]]).IsMoveLegal(board, newAndOldRowAndCollums[0], newAndOldRowAndCollums[1], newAndOldRowAndCollums[2], newAndOldRowAndCollums[3], turnCount, false))
                {  // here it checks if the piece can make the move the user is requesting, according to which piece it is
                    Console.WriteLine("Invalid move");
                    input = getInput(input, out inputcheck);
                    return false;
                }
                else
                    if (board[newAndOldRowAndCollums[2], newAndOldRowAndCollums[3]] is Pawn)
                    pawnJustMoved = true;
            }
            else // in case the move leaves his own king under threat
            {
                Console.WriteLine("Invalid move");
                input = getInput(input, out inputcheck);
                return false;
            }
            return true;
        }
        int printBoard(ChessPiece[,] board)
        {
            int currentpieces = 0, totalKnightsandBishops = 0;
            Console.WriteLine("   A  B  C  D  E  F  G  H");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(i + 1 + "  ");
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] is null)
                        Console.Write("EE ");
                    else
                    {
                        currentpieces++;
                        Console.Write(board[i, j]);
                        if(board[i,j] is Knight|| board[i,j] is Bishop)
                            totalKnightsandBishops++;
                    }
                }
                Console.WriteLine();
            }
            return totalKnightsandBishops <= 1 && currentpieces <= 3? currentpieces - 1:currentpieces;
        }
        int[] findTheKing(ChessPiece[,]board, int turnCount, bool checkingMyKing)
        {
            int[] kingRowAndColumn=new int[2];
            kingRowAndColumn[0] = 0;
            kingRowAndColumn[1] = 0;
            int i, j = 0;
            if (checkingMyKing)
                turnCount++;
            bool foundking = false;
            for (i = 0; i < 8 && !foundking; i++)// find the king
            {
                for (j = 0; j < 8 && !foundking; j++)
                    if (board[i, j] is King)
                    {
                        if (board[i, j].getWhite() == true && turnCount == 0 || turnCount % 2 == 0)// found black king in white's turn -- under threat
                        {
                            foundking = true;
                            continue;
                        }
                        if (board[i, j].getWhite() == false && turnCount % 2 != 0)// found white king in black's turn -- under threat
                        {
                            foundking = true;
                            continue;
                        }
                    }
            }
            kingRowAndColumn[0] = i - 1;
            kingRowAndColumn[1] = j - 1;
            return kingRowAndColumn;
        }
        int[] findTheThreat(ChessPiece[,] board, int turnCount, bool checkingMyKing)
        {
            int[] kingRowAndColumn = new int[2];
            kingRowAndColumn=findTheKing(board, turnCount, checkingMyKing);
            int i = kingRowAndColumn[0], j = kingRowAndColumn[1];
            int[] threatRowandCollum = new int[4];
            threatRowandCollum[2] = i;
            threatRowandCollum[3] = j;
            bool aThreat = false;
            return threatRowandCollum = isThereAThreat(board, i, j, turnCount, false, checkingMyKing, ref aThreat);
        }// if there is no threat, the array would be null,otherwise the array returns the location of the threat
        int[] isThereAThreat(ChessPiece[,] board, int row, int column, int turnCount, bool checkingNull, bool checkingMyKing, ref bool aThreat)
        {
            int[] threatRowandCollum = new int[4];
            threatRowandCollum[2] = row;
            threatRowandCollum[3] = column; 
            for (int x = 0; x <  8&& !aThreat; x++)
                for (int y = 0; y < 8 && !aThreat; y++)
                    if (board[x, y] != null)
                        if (((IAction)board[x, y]).IsMoveLegal(board, x, y, row, column, turnCount, true))
                        {
                            if (board[x, y] is King)
                            {
                                ChessPiece placeholderForMovement = board[row, column];
                                board[row, column] = board[x, y];
                                board[x, y] = null;
                                if (!isKingThreatened(board, turnCount, false))
                                {
                                    board[x, y] = board[row, column];
                                    board[row, column] = placeholderForMovement;
                                    threatRowandCollum[0] = x;
                                    threatRowandCollum[1] = y;
                                    aThreat = true;
                                    return threatRowandCollum;
                                }
                                else
                                {
                                    board[x, y] = board[row, column];
                                    board[row, column] = placeholderForMovement;
                                }
                            }
                            else
                            {
                                threatRowandCollum[0] = x;
                                threatRowandCollum[1] = y;
                                aThreat = true;
                                return threatRowandCollum;
                            }
                        }
            return threatRowandCollum;
        }
        int[] isInputValid(string input)
        {
            int[] numToUpdate = new int[4];
            bool inputcheck = false;
            input = input.ToLower();
            while (!inputcheck)
            {
                if (input.Length != 4)
                    input = getInput(input, out inputcheck);
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        switch (input[i]) // checks if the letters are valid in the input
                        {
                            case '1':case '2':case '3':case '4':case '5':case '6':case '7':case '8':
                                if (i == 0 || i % 2 == 0)
                                    break;
                                else
                                {
                                    input = getInput(input, out inputcheck);
                                    i = -1;
                                    break;
                                }
                            case 'a':case 'b':case 'c':case 'd':case 'e':case 'f':case 'g':case 'h':
                                if (i % 2 != 0)
                                    break;
                                else
                                {
                                    input = getInput(input, out inputcheck);
                                    i = -1;
                                    break;
                                }
                            default:
                                input = getInput(input, out inputcheck);
                                i = -1;
                                break;
                        }
                    }
                    for (int j = 0; j < 4; j++)
                    {
                        switch (input[j])
                        {
                            case '1': case 'a': numToUpdate[j] = 0; break;
                            case '2': case 'b': numToUpdate[j] = 1; break;
                            case '3': case 'c': numToUpdate[j] = 2; break;
                            case '4': case 'd': numToUpdate[j] = 3; break;
                            case '5': case 'e': numToUpdate[j] = 4; break;
                            case '6': case 'f': numToUpdate[j] = 5; break;
                            case '7': case 'g': numToUpdate[j] = 6; break;
                            case '8': case 'h': numToUpdate[j] = 7; break;
                        }
                    }
                    inputcheck = true;
                }
            }
            return numToUpdate;
        }// makeMove uses this only!
        string playTurn(string input, ChessPiece[,] board, int turnCount,ref bool pawnJustMoved)
        {
            bool inputcheck = false;
            int[] newAndOldRowAndCollums = new int[4];
            while (!inputcheck)
            {
                if (input.Length != 4)
                {
                    Console.WriteLine("Invalid entry");
                    input = getInput(input, out inputcheck);
                    continue;
                }
                else
                    newAndOldRowAndCollums = isInputValid(input);
                if (board[newAndOldRowAndCollums[0], newAndOldRowAndCollums[1]] is IAction)// checks if its a piece
                {
                    if (board[newAndOldRowAndCollums[0], newAndOldRowAndCollums[1]].getWhite())//if user selected a white piece
                    {
                        if (turnCount == 0 || turnCount % 2 == 0) // and its white's turn
                        {
                            if (!isMoveValid(ref input, board, turnCount, ref pawnJustMoved, ref inputcheck, newAndOldRowAndCollums))
                                continue;
                        }
                        else // in case the player selected his opponent's piece
                        {
                            Console.WriteLine("Thats not your piece");
                            input = getInput(input, out inputcheck);
                            continue;
                        }
                    }
                    else
                    {
                        if (turnCount % 2 != 0) // and its black's turn
                        {
                            if (!isMoveValid(ref input,board,turnCount,ref pawnJustMoved,ref inputcheck,newAndOldRowAndCollums))
                                continue;
                        }// in case user selected his opponents piece
                        else
                        {
                            Console.WriteLine("Thats not your piece");
                            input = getInput(input, out inputcheck);
                            continue;
                        }
                    } // if the piece is black
                }
                else
                {
                    Console.WriteLine("Invalid entry");
                    input = getInput(input, out inputcheck);
                    continue;
                }// checks if the user input of which to move is actually a piece
                inputcheck = true;
            }
            return input;
        }
        string getInput(string input,out bool inputcheck)
        {
            inputcheck = false;
            Console.WriteLine("Please enter a valid row # and collum letter to move from, and another pair deciding where to move");
            return input = Console.ReadLine();
        }
    }
}