using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sudoku
{
    class Solver
    {
        SudokuBoard initialBoard;
        SudokuBoard currentBoard;
        List<Cell> blankCells;
        Stopwatch sw;
        int solutions;

        public Solver(SudokuBoard initial)
        {
            initialBoard = initial;
            solutions = 0;
            sw = new Stopwatch();
            sw.Start();
        }

        public void Restart()
        {
            currentBoard = SudokuBoard.DeepCopy(initialBoard);
            blankCells = currentBoard.GetPossibleGuesses();
            Start();
        }

        public void Start()
        {
            initialBoard.Refactor();
            currentBoard = SudokuBoard.DeepCopy(initialBoard);
            blankCells = currentBoard.GetPossibleGuesses();

            if (!currentBoard.Solved())
                GetPossibleGuesses();

            Console.WriteLine("\n\tSolving....");
            while (!currentBoard.Solved())
            {
                int numBlankCells = currentBoard.GetNumberOfBlank();

                TrySolving();

                if (numBlankCells == currentBoard.GetNumberOfBlank())
                    RecursiveGuess(currentBoard, 0);

                if (currentBoard.Finished() && !currentBoard.Solved())
                    Restart();
            }

            if (currentBoard.Finished() && currentBoard.Solved())
            {
                sw.Stop();
                Console.Clear();
                Console.WriteLine("\n\tElapsed: {0}", sw.Elapsed);
                Console.WriteLine("\t{0} SOLUTION'S FOUND", solutions);
                currentBoard.PrintEverything();
            }
        }

        public bool TrySolving()
        {
            for (int i = 0; i < 9; i++)
            {
                currentBoard.GetSection(i, SectionType.ROW).SingleOut();
                currentBoard.GetSection(i, SectionType.COLUMN).SingleOut();
                currentBoard.GetSection(i, SectionType.REGION).SingleOut();
            }

            if (currentBoard.Solved())
                return true;
            else
                return false;
        }

        public void RecursiveGuess(SudokuBoard previous, int previousGuess)
        {
            bool blocked = false;
            bool fullyBlocked = false;
            int currentGuess = 0;
            SudokuBoard tempBoard = null;

            double seconds = sw.Elapsed.TotalSeconds;

            if (seconds % 5 < 0.001)
            {
                Console.WriteLine("\n\tElapsed: {0}", sw.Elapsed);
                Console.WriteLine("\t{0} solutions", solutions);
            }

            if (previous.CurrentlyVerified())
            {
                if (!previous.Finished())
                {
                    tempBoard = SudokuBoard.DeepCopy(previous);

                    blankCells = tempBoard.GetPossibleGuesses();
                    if (previousGuess != 0)
                    {
                        currentGuess = blankCells.First().possibilities.Find(x => x > previousGuess);
                        if (currentGuess == 0)
                            fullyBlocked = true;
                    }
                    else if (currentGuess < blankCells.First().possibilities.First())
                    {
                        currentGuess = blankCells.First().possibilities.First();
                    }

                    if (currentGuess != 0)
                        blankCells.First().ForceCellValue(currentGuess);
                    else
                        blocked = true;

                    if (tempBoard.Finished() && !tempBoard.Solved())
                        blocked = true;

                    if (!blocked)
                        RecursiveGuess(tempBoard, 0);
                }
                else if (previous.Solved())
                {
                    fullyBlocked = true;

                    if (!previous.Equals(tempBoard))
                        currentBoard = SudokuBoard.DeepCopy(previous);

                    solutions++;
                }
            }
            else
                fullyBlocked = true;

            if (!fullyBlocked)
                RecursiveGuess(previous, currentGuess);
        }

        public void GetPossibleGuesses()
        {
            blankCells = currentBoard.GetPossibleGuesses();
        }
    }
}
