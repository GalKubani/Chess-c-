using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ChessLibrary
{
    public interface IAction
    {
        bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats);
        void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn);
    }
    public class ChessPiece
    {
        int id;
        public int getId(){return id;}
        bool white;
        public ChessPiece(bool white, int id){this.white = white; this.id = id;}
        public bool getWhite(){return white;}
    }
    public class Pawn : ChessPiece, IAction
    {
        int movement;
        bool firstTurn = true, justMadeDoubleStep = false;
        public override string ToString(){  return string.Format((getWhite() ? "W" : "B") + "P ");}
        public Pawn(bool white, int id) : base(white, id) { SetMovement(); }
        public bool GetJustMadeDoubleStep(){return justMadeDoubleStep;}
        public void SetJustMadeDoubleStep(bool justMadeDoubleStep){this.justMadeDoubleStep = justMadeDoubleStep;}
        public void SetMovement()
        {
            if (firstTurn == true)
                movement = 2;
            else
                movement = 1;
        }
        public int GetMovement(){return movement; }
        public void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            board[newRow, newColumn] = board[oldRow, oldColumn];
            board[oldRow, oldColumn] = null;
        }
        void crowning(ChessPiece[,] board, int newRow, int newColumn, int turnCount)
        {
            Console.WriteLine("Your pawn is crowned, which piece would you like? enter r/n/b/q to select ");
            string crown =  Console.ReadLine();
            bool end = false;
            int saveId = board[newRow, newColumn].getId();
            while (!end)
            {
                if (crown.Length == 1)
                {
                    switch (crown[0])
                    {
                        case 'r': board[newRow, newColumn] = null; board[newRow, newColumn] = new Rook(turnCount % 2 == 0 ? true : false, saveId); end = true; break;
                        case 'n': board[newRow, newColumn] = null; board[newRow, newColumn] = new Knight(turnCount % 2 == 0 ? true : false, saveId); end = true; break;
                        case 'b': board[newRow, newColumn] = null; board[newRow, newColumn] = new Bishop(turnCount % 2 == 0 ? true : false, saveId); end = true; break;
                        case 'q': board[newRow, newColumn] = null; board[newRow, newColumn] = new Queen(turnCount % 2 == 0 ? true : false, saveId); end = true; break;
                        default:
                            Console.WriteLine("Invalid entry,  which piece would you like? enter r/n/b/q to select");
                            crown = Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid entry,  which piece would you like? enter r/n/b/q to select");
                    crown = Console.ReadLine();
                }
            }
        }
        bool confirmIfAttackLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, bool checkingForThreats)
        {
            if (board[newRow, newColumn].getWhite() != board[oldRow, oldColumn].getWhite())
                if (checkingForThreats == false)
                {
                    MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                    return true;
                }
                else
                    return true;
            return false;
        }
        bool isAttackLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, bool checkingForThreats, int oldRowVar)
        {
            if (board[newRow, newColumn] != null && oldRowVar == newRow && oldColumn - 1 == newColumn && oldColumn > 0)
                if (confirmIfAttackLegal(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats))
                    return true;
            if (board[newRow, newColumn] != null && oldRowVar == newRow && oldColumn + 1 == newColumn && oldColumn < 7)
                if (confirmIfAttackLegal(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats))
                    return true;
            return false;
        }
        bool isHitInMotionLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn,  bool checkingForThreats, int oldRowVar)
        {
            int indexC;
            if (oldColumn == 7)
                indexC = oldColumn - 1;
            else
                indexC = oldColumn + 1;
            bool moveLegal = confirmHitInMotion(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, oldRowVar,indexC);
            if (moveLegal)
                return true;
            if (oldColumn == 0)
                indexC = oldColumn;
            else
                indexC = oldColumn - 1;
            moveLegal = confirmHitInMotion(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, oldRowVar, indexC);
            return moveLegal=true ? true : false;
        }
        bool confirmHitInMotion(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, bool checkingForThreats, int oldRowVar,int indexC)
        {
            if (board[oldRow, indexC] != null && board[newRow, newColumn] == null) //in case he attacks pawn on right
                if (board[oldRow, indexC] is Pawn)// makes sure that the pawn did make a double step, while making sure that the user entered the same side as the enemy pawn
                    if (((Pawn)board[oldRow, indexC]).GetJustMadeDoubleStep() && board[oldRowVar, indexC] == null && oldRowVar == newRow && indexC == newColumn)
                    {
                        if (checkingForThreats == false)
                        {
                            board[oldRow, indexC] = null;
                            MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                            return true;
                        }
                        return true;
                    }
            return false;
        }
        public bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {
            if (getWhite())
            {
                if (oldRow == 4)     // white "hit in motion" excluding the edges only happens on row 4
                    isHitInMotionLegal(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats,oldRow+1);
                else if (newRow - oldRow > movement || newRow - oldRow <= 0) // go down
                    return false;
                if (movement == 2 && board[oldRow + 1, oldColumn] != null && oldRow + 2 == newRow)
                    return false;
            }  // checks pawn movement possible according to color, can only move in 1 direction
            else // black player
            {
                if (oldRow == 3) //black "hit in motion" excluding the edges only happens on row 3
                    isHitInMotionLegal(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats,oldRow - 1);
                else if (oldRow - newRow > movement || newRow - oldRow >= 0)// go up
                    return false;
                if (movement == 2 && board[oldRow - 1, oldColumn] != null && oldRow - 2 == newRow) // in case the user wants to make double step and is blocked
                    return false;
            }     // both check return false in case the movement isnt in the range
            if (board[newRow, newColumn] == null && oldColumn == newColumn)
            {  // in case the movement is possible, the pawn is checked for its first turn and sets its movement to 1 incase it was the first turn
                if (oldRow + 2 == newRow || oldRow - 2 == newRow)
                    justMadeDoubleStep = true;
                else
                    justMadeDoubleStep = false;
                firstTurn = false;
                SetMovement();
            }
            if (newColumn == oldColumn && board[newRow, newColumn] == null)
                MakeMove(board, oldRow, oldColumn, newRow, newColumn);
            else if (turnCount == 0 || turnCount % 2 == 0 && board[oldRow, oldColumn] != null) // white turn attack
            {
                bool didPawnAttack= isAttackLegal(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, oldRow + 1);
                if (!didPawnAttack)
                    return false;
            }
            else if (turnCount % 2 != 0 && board[oldRow, oldColumn] != null) // black player turn attack
            {
                bool didPawnAttack = isAttackLegal(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, oldRow - 1);
                if (!didPawnAttack)
                    return false;
            }
            if (newRow == 7 && !checkingForThreats || newRow == 0 && !checkingForThreats)
                crowning(board, newRow, newColumn, turnCount);  // in case of crowning
            return true;
        }
    }
    public class Rook : ChessPiece, IAction
    {
        bool madeFirstStep = false;
        public override string ToString(){ return string.Format((getWhite() ? "W" : "B") + "R ");}
        public Rook(bool white, int id) : base(white, id) { }
        public Rook(bool white, bool madeFirstStep, int id) : base(white, id) { SetMadeFirstStep(madeFirstStep); }
        public bool GetMadeFirstStep(){ return madeFirstStep;}
        public void SetMadeFirstStep(bool madeFirstStep){this.madeFirstStep = madeFirstStep;}
        public void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            board[newRow, newColumn] = board[oldRow, oldColumn];
            board[oldRow, oldColumn] = null;
        }
        bool checkMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, bool checkingForThreats,int endOfLoop,int startOfLoop,bool vertical)
        {
            for (int i = startOfLoop; i < endOfLoop; i++) //from after the old column, until and including the new one 
            {     // if the move was possible at the end of this loop it will be made, if not it returns false
                if (i < endOfLoop - 1) // runs until the slot requested to address, if its not null it means the insert is false
                {
                    if (vertical)
                    {
                        if (board[i + 1, oldColumn] != null)
                            return false;
                    }
                    else
                    {
                        if (board[oldRow, i + 1] != null)
                            return false;
                    }
                }
                else if (i + 1 == endOfLoop && board[newRow, newColumn] != null)
                {
                    if (board[newRow, newColumn].getWhite() == board[oldRow, oldColumn].getWhite())
                        return false;
                }
            }
            return true;
        }
        public bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {  // these 4 if statements check if the move is logical for a rook, meaning only in a line or a row
            if (oldRow == newRow && oldColumn < newColumn) // move right
            {
                bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, newColumn, oldColumn, false);
                if (isMoveValid)
                    return true;
            }  // move right
            if (oldRow == newRow && oldColumn > newColumn) // move left
            {
                bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, oldColumn, newColumn, false);
                if (isMoveValid)
                    return true;
            }   // move left
            if (oldRow > newRow && oldColumn == newColumn)  // move up
            {
                bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, oldRow, newRow, true);
                if (isMoveValid)
                    return true;
            }    // move up
            if (oldRow < newRow && oldColumn == newColumn)// move down 
            {
                bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, checkingForThreats, newRow, oldRow, true);
                if (isMoveValid)
                    return true;
            }   // move down
            return false;
        }
    }
    public class Knight : ChessPiece, IAction
    {
        public Knight(bool white, int id) : base(white, id) { }
        public override string ToString() { return string.Format((getWhite() ? "W" : "B") + "N ");}
        public void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            board[newRow, newColumn] = board[oldRow, oldColumn];
            board[oldRow, oldColumn] = null;
        }
        public bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {
            if (oldColumn == newColumn - 2 || oldColumn == newColumn + 2)
            {//2 steps up or down
                if (newRow == oldRow - 1) { }// in case goes left
                else if (newRow == oldRow + 1) { }// in case goes right
                else
                    return false;
            }//2 steps up or down
            else if (oldRow == newRow - 2 || oldRow == newRow + 2)
            {//2 steps left or right
                if (newColumn == oldColumn - 1) { }// in case goes up
                else if (newColumn == oldColumn + 1) { }// in case goes down
                else
                    return false;
            }//2 steps left or right
            else  // if neither happent the move isnt possible
                return false;
            if (board[newRow, newColumn] != null) // if the move is possible for a knight it checks if the slot isnt null, 
            {  /// if its not null, the code makes sure the user isnt eating his own pieces
                if (board[newRow, newColumn].getWhite() != board[oldRow, oldColumn].getWhite())
                {
                    if (checkingForThreats == false)
                    {
                        MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                    }
                    return true;
                }
            }
            else  // if it is null, then it means the move is possible and makes it here
            {
                if (checkingForThreats == false)
                {
                    MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                }
                return true;
            }
            return false;
        }
    }
    public class Bishop : ChessPiece, IAction
    {
        public override string ToString(){ return string.Format((getWhite() ? "W" : "B") + "B ");}
        public Bishop(bool white, int id) : base(white, id) { }
        public void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            board[newRow, newColumn] = board[oldRow, oldColumn];
            board[oldRow, oldColumn] = null;
        }
        bool checkMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn,int i, int column,int endOfLoop,int row)
        {
            if (i != endOfLoop)
                if (board[row, column] != null) // fisrt checking if the line of movement is empty, if it isnt, returns false as cannot move due to blocked
                    return false;
            else if (board[newRow, newColumn] != null)// at the last run, checks if the requested slot isnt null // after that it compares the teams of the pieces, if they dont match the move is valid and made
                if (!(board[newRow, newColumn].getWhite() ^ board[oldRow, oldColumn].getWhite()))
                    return false;
            return true;
        }
        public bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {
            if (oldRow - oldColumn == newRow - newColumn || -oldRow - oldColumn == newRow - newColumn || -oldRow - oldColumn == -newRow - newColumn || oldRow - oldColumn == -newRow - newColumn)
            {
                if (oldRow < newRow && oldColumn < newColumn) // down right ++
                {
                    for (int i = oldRow, j = oldColumn; i < newRow && j != 7 && i != 7; i++, j++)
                    {
                        bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, i, j + 1, newRow - 1, i + 1);
                        if (!isMoveValid)
                            return false;
                    }
                    if (checkingForThreats == false)
                    {
                        MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                    }
                    return true;
                }  // down right
                if (oldRow > newRow && oldColumn > newColumn) // up left --
                {
                    for (int i = oldRow, j = oldColumn; i > newRow && j != 0 && i != 0; i--, j--)
                    {
                        bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, i, j - 1, newRow + 1, i-1);
                        if (!isMoveValid)
                            return false;
                    }
                    if (checkingForThreats == false)
                    {
                        MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                    }
                    return true;
                }  // up left
                if (oldRow > newRow && oldColumn < newColumn) // up right -+
                {
                    for (int i = oldRow, j = oldColumn; i > newRow && j != 7 && i != 0; j++, i--)
                    {
                        bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, i, j + 1, newRow + 1, i-1);
                        if (!isMoveValid)
                            return false;
                    }
                    if (checkingForThreats == false)
                    {
                        MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                    }
                    return true;
                }  //up right
                if (oldRow < newRow && oldColumn > newColumn) // down left +-
                {
                    for (int i = oldRow, j = oldColumn; i < newRow && j != 0 && i != 7; i++, j--)
                    {
                        bool isMoveValid = checkMove(board, oldRow, oldColumn, newRow, newColumn, i, j - 1, newRow - 1, i-1);
                        if (!isMoveValid)
                            return false;
                    }
                    if (checkingForThreats == false)
                    {
                        MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                    }
                    return true;
                } // down left
            }
            return false;
        }
    }
    public class Queen : ChessPiece, IAction
    {
        Bishop queenAsBishop;
        Rook queenAsRook;
        public override string ToString(){ return string.Format((getWhite() ? "W" : "B") + "Q ");}
        public bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {
            return queenAsBishop.IsMoveLegal(board, oldRow, oldColumn, newRow, newColumn, turnCount, checkingForThreats) || queenAsRook.IsMoveLegal(board, oldRow, oldColumn, newRow, newColumn, turnCount, checkingForThreats);
        }
        public Queen(bool white, int id) : base(white, id)
        {
            queenAsBishop = new Bishop(white, id);
            queenAsRook = new Rook(white, id);
        }
        public void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            board[newRow, newColumn] = board[oldRow, oldColumn];
            board[oldRow, oldColumn] = null;
        }
    }
    public class King : ChessPiece, IAction
    {
        bool madeFirstStep = false;
        public override string ToString(){ return string.Format((getWhite() ? "W" : "B") + "K ");}
        public King(bool white, int id) : base(white, id) { }
        public King(bool white, bool madeFirstStep, int id) : base(white, id) { madeFirstStep = true; }
        public bool GetMadeFirstStep(){return madeFirstStep; }
        public void MakeMove(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn)
        {
            board[newRow, newColumn] = board[oldRow, oldColumn];
            board[oldRow, oldColumn] = null;
        }
        bool isHazrahaLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {
            if (oldColumn + 2 == newColumn)//moving right
                if (board[oldRow, 5] == null && board[oldRow, 6] == null)
                    if (board[oldRow, 7] is Rook)
                        if (!((Rook)board[oldRow, 7]).GetMadeFirstStep() && !madeFirstStep)
                        {
                            if (checkingForThreats == false)// will also need to check the rook that might be moved, need  to check threat for king first before each step          
                            {                      // if theres no threat for the king, make sure to change all rook's options for hazraha to false
                                board[oldRow, 5] = new Rook(turnCount % 2 == 0 ? true : false, true, board[oldRow, 7].getId());
                                board[oldRow, 7] = null;
                                MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                                madeFirstStep = true;
                            }
                            return true;
                        }
            if (oldColumn - 2 == newColumn)// moving left
                if (board[oldRow, 2] == null && board[oldRow, 3] == null)
                    if (board[oldRow, 0] is Rook)
                        if (!((Rook)board[oldRow, 0]).GetMadeFirstStep() && !madeFirstStep)
                        {
                            if (checkingForThreats == false)
                            {
                                board[oldRow, 3] = new Rook(turnCount % 2 == 0 ? true : false, true, board[oldRow, 0].getId());
                                board[oldRow, 0] = null;
                                MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                                madeFirstStep = true;
                            }
                            return true;
                        }
            return false;
        }
        public bool IsMoveLegal(ChessPiece[,] board, int oldRow, int oldColumn, int newRow, int newColumn, int turnCount, bool checkingForThreats)
        {
            if (madeFirstStep == false)  // only if its the first step for the king
                if (oldRow == 0 || oldRow == 7 && oldRow == newRow) // black hazraha
                {
                    bool isHazrahaPossible = isHazrahaLegal(board, oldRow, oldColumn, newRow, newColumn, turnCount, checkingForThreats);
                    if (isHazrahaPossible)
                        return true;
                }
            if (oldRow + 1 == newRow && oldColumn + 1 == newColumn || oldRow - 1 == newRow && oldColumn - 1 == newColumn || oldRow - 1 == newRow && oldColumn + 1 == newColumn || oldRow + 1 == newRow && oldColumn - 1 == newColumn
                || oldRow == newRow && oldColumn == newColumn + 1 || oldRow == newRow && oldColumn == newColumn - 1 || oldRow == newRow - 1 && oldColumn == newColumn || oldRow == newRow + 1 && oldColumn == newColumn)
            {
                if (board[newRow, newColumn] != null)
                {
                    if (board[newRow, newColumn].getWhite() != board[oldRow, oldColumn].getWhite())
                    {
                        if (checkingForThreats == false)
                        {
                            MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                            madeFirstStep = true;
                        }
                        return true;
                    }
                }
                else
                {
                    if (checkingForThreats == false)
                    {
                        MakeMove(board, oldRow, oldColumn, newRow, newColumn);
                        madeFirstStep = true;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}