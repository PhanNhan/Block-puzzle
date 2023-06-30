using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassicMode.Gameplay.Controllers
{
    public class BlockHelper
    {
        private static List<int> ListMarketBlock = new List<int>();

        private static int[][][] BlockData = new int[][][]
        {
			//Shape 0
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 1
			new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 2
			new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 3
			new int[][]
            {
                new int[] { 1, 1, 1, 1, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 4
			new int[][]
            {
                new int[] { 1, 1, 1, 1, 1 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 5
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 6
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 7
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 8
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 }
            },
			//Shape 9
			new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 10
			new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 11
			new int[][]
            {
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 12
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 13
			new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 14
			new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 1, 0, 0 },
                new int[] { 0, 0, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 15
			new int[][]
            {
                new int[] { 0, 0, 1, 0, 0 },
                new int[] { 0, 0, 1, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 16
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 17
			new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//Shape 18
			new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //19
            new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //20
            new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //21
            new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //22
            new int[][]
            {
                new int[] { 0, 0, 1, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//23
			new int[][]
            {
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//24
			new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//25
			new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
			//26
			new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },

            //27
            new int[][]
            {
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //28
            new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //29
            new int[][]
            {
                new int[] { 1, 1, 1, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //30
            new int[][]
            {
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //31
            new int[][]
            {
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 1, 1, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //32
            new int[][]
            {
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //33
            new int[][]
            {
                new int[] { 0, 1, 1, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            },
            //34
            new int[][]
            {
                new int[] { 1, 0, 0, 0, 0 },
                new int[] { 1, 1, 0, 0, 0 },
                new int[] { 0, 1, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0 }
            }
        };

        public static int GetBlockCount { get { return BlockData.Length; } }
        public static int[][] CloneBlockMatrix(int type)
        {
            type = type % BlockHelper.GetBlockCount;
            int rowCount = BlockData[type].Length;
            int columnCount = BlockData[type][0].Length;

            int[][] clonedMatrix = new int[rowCount][];
            for (int row = 0; row < rowCount; row++)
            {
                clonedMatrix[row] = new int[columnCount];
                for (int col = 0; col < columnCount; col++)
                {
                    clonedMatrix[row][col] = BlockData[type][row][col];
                }
            }
            return clonedMatrix;
        }

        public static List<int> GetRandomBlocks(List<int> igroneList)
        {
            var items = new List<int>();
            for (int i = 0, c = GetBlockCount; i < c; ++i)
            {
                items.Add(i);
            }

            if (igroneList != null && igroneList.Count > 0)
            {
                for (int i = 0, c = igroneList.Count; i < c; ++i)
                {
                    items.Remove(igroneList[i]);
                }
            }

            var results = new List<int>();
            for (int i = 0, c = 3; i < c; ++i)
            {
                var t = Util.RandomHelper.Choice(items);
                results.Add(t);
                items.Remove(t);
            }
            return results;
        }

        public static PrioritisedRandomGenerator<int> blockRandomGenerator = null;

        public static List<int> ThreeBlockRatios = new List<int>(new int[] {
            1, //1x1
			1, //1x2
			1, //1x3
			1, //1x4
			1, //1x5
			1, //2x1
			1, //3x1
			1, //4x1
			1, //5x1
			1, //L2.1
			1, //L2.2
			1, //L2.3
			1, //L2.4
			1, //L3.1
			1, //L3.2
			1, //L3.3
			1, //L3.4
			1, //2x2
			1, //3x3
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
        });

        public static List<int> OneBlockRatios = new List<int>(new int[] {
            1, //1x1
			1, //1x2
			1, //1x3
			1, //1x4
			1, //1x5
			1, //2x1
			1, //3x1
			1, //4x1
			1, //5x1
			1, //L2.1
			1, //L2.2
			1, //L2.3
			1, //L2.4
			1, //L3.1
			1, //L3.2
			1, //L3.3
			1, //L3.4
			1, //2x2
			1, //3x3,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1
        });

        public static void Init()
        {

            bool enableOneBlock = C.GameplayConfig.GameModeOneBlock; ;
            List<int> currentBlockRatios = enableOneBlock ? OneBlockRatios : ThreeBlockRatios;
            if (currentBlockRatios == null || currentBlockRatios.Count == 0)
                currentBlockRatios = enableOneBlock ? OneBlockRatios : ThreeBlockRatios;

            Dictionary<int, int> blockPriorities = new Dictionary<int, int>();
            for (int i = 0; i < currentBlockRatios.Count && i < BlockHelper.GetBlockCount; i++)
            {
                if (currentBlockRatios[i] > 0)
                    blockPriorities.Add(i, currentBlockRatios[i]);
            }
            blockRandomGenerator = new PrioritisedRandomGenerator<int>(blockPriorities);
        }

        public static int RandomBlock()
        {
            Init();
            if (blockRandomGenerator == null)
            {
                return 0;
            }
            return blockRandomGenerator.GetRandom();
        }
    }
}
