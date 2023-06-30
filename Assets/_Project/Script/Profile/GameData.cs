using System.Collections.Generic;

namespace Profile
{
    public class GameData
    {
        public bool HasSaved;
        public bool IsBreakBestScore;
        public int Score;
		public int ScoreHalloween;
        public Board Grid;
        public List<Block> Blocks;
		public float Duration;
        public int TimesGameOver;
        public int ContinueScore;
        public int ContinueDiamonds;

		public GameData()
		{
			ScoreHalloween = 0;
		}

        public class Board
        {
            public List<Row> Cells;

            public class Row
            {
                public List<Cell> Cells;

                public class Cell
                {
                    public int Type;
                }
            }
        }

        public class Block
        {
            public int Type;
            public bool IsEmpty;
        }
    }
}
