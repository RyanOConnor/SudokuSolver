using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    [Serializable]
    public class SudokuBoard
    {
        private Cell[,] board;
        private List<Section> rows;
        private List<Section> columns;
        private List<Section> regions;

        public static SudokuBoard DeepCopy(SudokuBoard copy)
        {
            SudokuBoard other = (SudokuBoard)copy.MemberwiseClone();
            other.board = new Cell[9, 9];
            other.rows = new List<Section>();
            other.columns = new List<Section>();
            other.regions = new List<Section>();

            for (int i = 0; i < 9; i++)
            {
                other.rows.Add(new Section(SectionType.ROW));                     // initialize each Row
                other.columns.Add(new Section(SectionType.COLUMN));                  // initalize each Column
                other.regions.Add(new Section(SectionType.REGION));                  // initialize each Region
            }

            // Initalize each cell with it's preset value and whether or not it's a blank (modifiable) square
            for (int i = 0; i < other.board.GetLength(0); i++)
            {
                for (int j = 0; j < other.board.GetLength(1); j++)
                {
                    if ((copy.board[i, j].value > 0) && (copy.board[i, j].value <= 9))              // if coordinate is a preset cell, set it to value and isBlank false
                        other.board[i, j] = new Cell(copy.board[i, j].value, false, j, i);
                    else                                                        // else set it to zero and isBlank true
                        other.board[i, j] = new Cell(0, true, j, i);
                }
            }

            //  Assign each row and column it's group of cells
            List<Cell> rowCells = new List<Cell>();
            List<Cell> colCells = new List<Cell>();
            for (int i = 0; i < other.board.GetLength(0); i++)
            {
                for (int j = 0; j < other.board.GetLength(1); j++)
                {
                    rowCells.Add(other.board[i, j]);         // load up all cells in row
                    colCells.Add(other.board[j, i]);         // load up all cells in column
                }
                other.rows[i].AssignCells(rowCells);          // set each rows cells
                other.columns[i].AssignCells(colCells);       // set each columns cells
                rowCells = new List<Cell>();           // generate new List after each row as to not delete from memory
                colCells = new List<Cell>();           // generate new List after each column as to not delete from memory
            }

            // Assign each region it's group of cells
            List<Cell> regCells = new List<Cell>();
            int reg = 0;
            for (int i = 0; i < other.board.GetLength(0); i += 3)
            {
                for (int j = 0; j < other.board.GetLength(1); j += 3)
                {
                    for (int k = i; k < i + 3; k++)
                        for (int l = j; l < j + 3; l++)
                            regCells.Add(other.board[k, l]);  // load up all cells in region

                    other.regions[reg].AssignCells(regCells);  // set each regions cells
                    ++reg;                              // increment counter for List of regions
                    regCells = new List<Cell>();        // generate new List after each region as to not delete from memory
                }
            }

            //  Assign each cell their sections
            int regNum = 0;
            for (int i = 0; i < other.board.GetLength(0); i++)
            {
                for (int j = 0; j < other.board.GetLength(1); j++)
                {
                    if ((j % 3 == 0) && (j != 0))   // step over one region horizontally every 3 cells
                        regNum++;

                    other.board[i, j].SetSections(other.rows[i], other.columns[j], other.regions[regNum]);
                }
                if (i < 2)                          // bring region back to 0
                    regNum = 0;
                else if (i < 5)                     // bring region back to 3
                    regNum = 3;
                else if (i < 8)                     // bring region back to 6
                    regNum = 6;
            }
            other.Refactor();
            return other;
        }

        public SudokuBoard(int[,] presets)
        {
            if ((presets.GetLength(0) > 9) || (presets.GetLength(1) > 9))
                throw new Exception("Error - Sudoku board size too large");
            else
            {
                board = new Cell[9, 9];
                rows = new List<Section>();
                columns = new List<Section>();
                regions = new List<Section>();
                // Initialize each section
                for (int i = 0; i < 9; i++)
                {
                    rows.Add(new Section(SectionType.ROW));                     // initialize each Row
                    columns.Add(new Section(SectionType.COLUMN));                  // initalize each Column
                    regions.Add(new Section(SectionType.REGION));                  // initialize each Region
                }

                // Initalize each cell with it's preset value and whether or not it's a blank (modifiable) square
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if ((presets[i, j] > 0) && (presets[i, j] <= 9))              // if coordinate is a preset cell, set it to value and isBlank false
                            board[i, j] = new Cell(presets[i, j], false, j, i);
                        else                                                        // else set it to zero and isBlank true
                            board[i, j] = new Cell(0, true, j, i);
                    }
                }

                //  Assign each row and column it's group of cells
                List<Cell> rowCells = new List<Cell>();
                List<Cell> colCells = new List<Cell>();
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        rowCells.Add(board[i, j]);         // load up all cells in row
                        colCells.Add(board[j, i]);         // load up all cells in column
                    }
                    rows[i].AssignCells(rowCells);          // set each rows cells
                    columns[i].AssignCells(colCells);       // set each columns cells
                    rowCells = new List<Cell>();           // generate new List after each row as to not delete from memory
                    colCells = new List<Cell>();           // generate new List after each column as to not delete from memory
                }

                // Assign each region it's group of cells
                List<Cell> regCells = new List<Cell>();
                int reg = 0;
                for (int i = 0; i < board.GetLength(0); i += 3)
                {
                    for (int j = 0; j < board.GetLength(1); j += 3)
                    {
                        for (int k = i; k < i + 3; k++)
                            for (int l = j; l < j + 3; l++)
                                regCells.Add(board[k, l]);  // load up all cells in region

                        regions[reg].AssignCells(regCells);  // set each regions cells
                        ++reg;                              // increment counter for List of regions
                        regCells = new List<Cell>();        // generate new List after each region as to not delete from memory
                    }
                }

                //  Assign each cell their sections
                int regNum = 0;
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if ((j % 3 == 0) && (j != 0))   // step over one region horizontally every 3 cells
                            regNum++;

                        board[i, j].SetSections(rows[i], columns[j], regions[regNum]);
                    }
                    if (i < 2)                          // bring region back to 0
                        regNum = 0;
                    else if (i < 5)                     // bring region back to 3
                        regNum = 3;
                    else if (i < 8)                     // bring region back to 6
                        regNum = 6;
                }
            }
        }

        public List<Cell> GetPossibleGuesses()
        {
            var guesses = board.OfType<Cell>()
                .Where(x => x.isBlank)
                .ToList();
            return guesses;
        }

        public Section GetSection(int position, SectionType type)
        {
            Section section = null;
            if (type == SectionType.ROW)
                section = rows[position];
            else if (type == SectionType.COLUMN)
                section = columns[position];
            else if (type == SectionType.REGION)
                section = regions[position];
            return section;
        }

        public int GetNumberOfBlank()
        {
            return board.OfType<Cell>().Count(x => x.isBlank);
        }

        public bool Finished()
        {
            if (board.OfType<Cell>().All(x => (!x.isBlank)))
                return true;
            else
                return false;
        }

        public void Refactor()
        {
            for (int i = 0; i < 9; i++)             // Initialize each cell's potential values
            {
                rows[i].RefactorPossibilities();
                columns[i].RefactorPossibilities();
                regions[i].RefactorPossibilities();
            }
        }

        public void PrintEverything()
        {
            Console.WriteLine();
            int row = 0;

            for (int j = 0; j < 9; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if ((k % 3 == 0) && (k != 0))
                            Console.Write(" █ ");
                        else if (k != 0)
                            Console.Write(" | ");
                        for (int l = row; l < row + 3; l++)
                        {
                            if (board[j, k].isBlank)
                                if (board[j, k].possibilities.Contains(l + 1))
                                    Console.Write(board[j, k].possibilities.Find(x => x == l + 1));
                                else
                                    Console.Write(" ");
                            else if (!board[j, k].isBlank)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                if (l == 4)
                                    Console.Write(board[j, k].value);
                                else
                                    Console.Write(" ");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                                Console.Write(" ");
                        }
                    }
                    if (row == 0)
                        row = 3;
                    else if (row == 3)
                        row = 6;
                    else if (row == 6)
                        row = 0;
                    Console.WriteLine();
                }
                if (((j + 1) % 3 == 0) && j != 8)
                    Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
                else if (j != 8)
                    Console.WriteLine("----+-----+-----█-----+-----+-----█-----+-----+----");

            }
        }

        public bool Solved()
        {
            int verifyRegion = 0;
            int verifyColumn = 0;
            int verifyRow = 0;
            bool legitimate = true;
            int position = -1;

            for (int i = 0; i < 9; i++)
            {
                verifyRegion = regions[i].items.Sum(x => x.value);
                verifyColumn = columns[i].items.Sum(x => x.value);
                verifyRow = rows[i].items.Sum(x => x.value);
                if ((verifyRegion != 45) || (verifyColumn != 45) || (verifyRow != 45))
                {
                    legitimate = false;
                    position = i;
                    break;
                }
            }

            if (legitimate)
                return true;
            else
                return false;
        }

        public bool CurrentlyVerified()
        {
            for (int i = 0; i < 9; i++)
            {
                if (!rows[i].SectionVerified())
                    return false;
                else if (!columns[i].SectionVerified())
                    return false;
                else if (!regions[i].SectionVerified())
                    return false;
            }
            return true;
        }
    }
}
