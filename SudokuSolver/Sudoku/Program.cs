using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] board = getBoardInput1();

            SudokuBoard sudokuBoard = new SudokuBoard(board);
            sudokuBoard.Refactor();
            sudokuBoard.PrintEverything();
            Solver solver = new Solver(sudokuBoard);
            solver.Start();
        }

        public static int[,] getBoardInput1()
        {
            int[,] board = new int[9, 9];

            board[0, 2] = 2;
            board[0, 4] = 3;
            board[0, 5] = 6;
            //board[0, 8] = 5;
            board[1, 1] = 5;
            board[1, 3] = 9;
            //board[1, 7] = 2;
            board[2, 1] = 6;
            board[2, 4] = 2;
            board[2, 7] = 4;
            board[3, 7] = 1;
            //board[3, 8] = 2;
            board[4, 0] = 1;
            board[4, 3] = 5;
            board[4, 5] = 7;
            //board[4, 8] = 6;
            board[5, 0] = 3;
            board[5, 1] = 7;
            board[6, 1] = 3;
            board[6, 4] = 6;
            board[6, 7] = 8;
            board[7, 1] = 1;
            //board[7, 5] = 2;
            board[7, 7] = 6;
            //board[8, 0] = 6;
            board[8, 3] = 4;
            board[8, 4] = 1;
            board[8, 6] = 2;

            return board;
        }

        public static int[,] getBoardInput2()
        {
            int[,] board = new int[9, 9];

            board[0, 0] = 8;
            board[1, 2] = 3;
            board[1, 3] = 6;
            board[2, 1] = 7;
            board[2, 4] = 9;
            board[2, 6] = 2;
            board[3, 1] = 5;
            board[3, 5] = 7;
            board[4, 4] = 4;
            board[4, 5] = 5;
            board[4, 6] = 7;
            board[5, 3] = 1;
            board[5, 7] = 3;
            board[6, 2] = 1;
            board[6, 7] = 6;
            board[6, 8] = 8;
            board[7, 2] = 8;
            board[7, 3] = 5;
            board[7, 7] = 1;
            board[8, 1] = 9;
            board[8, 6] = 4;

            return board;
        }
    }
}
