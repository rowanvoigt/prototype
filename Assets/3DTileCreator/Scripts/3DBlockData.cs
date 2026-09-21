using System;
using UnityEngine;

namespace TilePainter3D.Blocks
{
    [Serializable]
    public class BlockData
    {
        public string name;
        //////////////////////BASIC///////////////////////////
        [Header("0. Block Shape")]
        [SerializeField]
        [Tooltip("Basic shape with 0 connections")]
        public TileBlockConfig block0Conn;
        /////////////////////THIN/////////////////////////////
        [Header("1. Thin/Path blocks")]
        [SerializeField]
        [Tooltip("End of a path")]
        public TileBlockConfig block1Conn;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks of blocks")]
        public TileBlockConfig block2ConnStraight;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks of blocks making a L shape")]
        public TileBlockConfig block2ConnCornerThin;
        [SerializeField]
        [Tooltip("Connects 3 thin blocks of blocks making a T shape")]
        public TileBlockConfig block3ConnT;
        [SerializeField]
        [Tooltip("Connects 4 thin blocks making a crosspath")]
        public TileBlockConfig block4Conn;
        //////////////////////SPECIAL////////////////////////////
        [Header("2. Special cases")]
        [SerializeField]
        [Tooltip("Connects 4 corner blocks making a four way")]
        public TileBlockConfig block4ConnSpecialFourWay;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks and 1 surface corner making a T shape(Righ case)")]
        public TileBlockConfig block3ConnTSpecialR;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks and 1 surface corner making a T shape(Left case)")]
        public TileBlockConfig block3ConnTSpecialL;
        [SerializeField]
        [Tooltip("Connects  thin blocks and surface blocks in a special 4 way case")]
        public TileBlockConfig block4ConnSpecial;
        [SerializeField]
        [Tooltip("Connects  thin blocks and surface blocks in special cases(Righ case)")]
        public TileBlockConfig block3SpecialConnR;
        [SerializeField]
        [Tooltip("Connects  thin blocks and surface blocks in special cases(Left case)")]
        public TileBlockConfig block3SpecialConnL;

        //////////////////////SURFACE////////////////////////////
        [Header("3. Surface blocks")]
        [SerializeField]
        [Tooltip("Connects 2  blocks making a corner")]
        public TileBlockConfig block2Corner;
        [SerializeField]
        [Tooltip("Connects 3(one floor block)  blocks making a balcony")]
        public TileBlockConfig block3Balcony;
        [Tooltip("Connects 2 balcony blocks and a end of path block of blocks making a T shape")]
        public TileBlockConfig block3ConnTBalcony;
        [SerializeField]
        [Tooltip("Connects 4 blocks making a inner corner")]
        public TileBlockConfig block4InnerCorner;
        [SerializeField]
        [Tooltip("Connects 4 blocks making floor")]
        public TileBlockConfig block4Floor;
        //////////////////////TERRAIN////////////////////////////
        [Header("3. Terrain blocks")]
        [Header("3.0 Block Shape Terrain")]
        [SerializeField]
        [Tooltip("Basic shape with 0 connections")]
        public TileBlockConfig block0ConnTerrain;
        //////////////////////////////////////////////////
        [Header("3.1 Thin/Path blocks")]
        [SerializeField]
        [Tooltip("End of a path")]
        public TileBlockConfig block1ConnTerrain;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks of blocks")]
        public TileBlockConfig block2ConnStraightTerrain;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks of blocks making a L shape")]
        public TileBlockConfig block2ConnCornerThinTerrain;
        [SerializeField]
        [Tooltip("Connects 3 thin blocks of blocks making a T shape")]
        public TileBlockConfig block3ConnTTerrain;
        [SerializeField]
        [Tooltip("Connects 4 thin blocks making a crosspath")]
        public TileBlockConfig block4ConnTerrain;
        [Header("3.2 Special cases")]
        [SerializeField]
        [Tooltip("Connects 4 corner blocks making a four way")]
        public TileBlockConfig block4ConnSpecialFourWayTerrain;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks and 1 surface corner making a T shape(Righ case)")]
        public TileBlockConfig block3ConnTSpecialRTerrain;
        [SerializeField]
        [Tooltip("Connects 2 thin blocks and 1 surface corner making a T shape(Left case)")]
        public TileBlockConfig block3ConnTSpecialLTerrain;
        [SerializeField]
        [Tooltip("Connects  thin blocks and surface blocks in a special 4 way case")]
        public TileBlockConfig block4ConnSpecialTerrain;
        [SerializeField]
        [Tooltip("Connects  thin blocks and surface blocks in special cases")]
        public TileBlockConfig block3SpecialConnRTerrain;
        [SerializeField]
        [Tooltip("Connects  thin blocks and surface blocks in special cases")]
        public TileBlockConfig block3SpecialConnLTerrain;
        //////////////////////////////////////////////////
        [Header("3.3 Surface blocks")]
        [SerializeField]
        [Tooltip("Connects 2 thin blocks making a corner")]
        public TileBlockConfig block2CornerTerrain;
        [SerializeField]
        [Tooltip("Connects 3(one floor block)  blocks making a balcony")]
        public TileBlockConfig block3BalconyTerrain;
        [SerializeField]
        [Tooltip("Connects 4 blocks making a inner corner")]
        public TileBlockConfig block4InnerCornerTerrain;
        //BOTTOM PART????


    }

    public class BlockMatrix
    {
        public bool[][] TopLayer;
        public bool[][] MiddleLayer;
        public bool[][] BottomLayer;
        int Tbitmask = -1;
        int Mbitmask = -1;

        int Bbitmask = -1;
        public BlockMatrix()
        {
            TopLayer = new bool[3][];
            MiddleLayer = new bool[3][];
            BottomLayer = new bool[3][];
            for (int i = 0; i < 3; i++)
            {
                TopLayer[i] = new bool[3];
                MiddleLayer[i] = new bool[3];
                BottomLayer[i] = new bool[3];

                for (int j = 0; j < 3; j++)
                {
                    TopLayer[i][j] = false;
                    MiddleLayer[i][j] = false;
                    BottomLayer[i][j] = false;
                }
            }

        }
        public override string ToString()
        {

            string stringMatrix = "MiddleLayer\n+--+--+--+\n";
            for (int i = 0; i < 3; i++)
            {
                stringMatrix += "|";
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1) stringMatrix += " C ";
                    else stringMatrix += MiddleLayer[i][j] ? " 1 " : " 0 ";

                    stringMatrix += "|";
                }

                stringMatrix += "\n+--+--+--+\n";
            }
            stringMatrix += "TopLayer\n+--+--+--+\n";
            for (int i = 0; i < 3; i++)
            {
                stringMatrix += "|";
                for (int j = 0; j < 3; j++)
                {

                    stringMatrix += TopLayer[i][j] ? " 1 " : " 0 ";

                    stringMatrix += "|";
                }

                stringMatrix += "\n+--+--+--+\n";
            }

            return stringMatrix;

        }
        public void SetMiddleLayer(int i, int j, bool value)
        {
            MiddleLayer[i][j] = value;
        }
        public void SetTopLayer(int i, int j, bool value)
        {
            TopLayer[i][j] = value;
        }
        public (GameObject, float) FindProperBlockWHeight(BlockData block)
        {
            if (Bbitmask == -1) GetBottomLayerBitMask();
            if (Mbitmask == -1) GetMiddleLayerBitMask();
            if (Tbitmask == -1) GetTopLayerBitMask();
            // Debug.Log("Block has Bbitmask: " + Bbitmask);
            // Debug.Log("Block has Mbitmask: " + Mbitmask);
            // Debug.Log("Block has Tbitmask: " + Tbitmask);
            if (Tbitmask == 0 && Bbitmask == 0)
            {
                return FindProperBlock(block);//Its the same base cases
            }
            switch (Tbitmask, Mbitmask, Bbitmask)
            {
                case (0, 0, 0):
                    break;

            }
            return (null, 0);
        }
        public (GameObject, float) FindProperBlock(BlockData block)
        {
            if (Mbitmask <= -1) GetMiddleLayerBitMask();

            try
            {
                switch (GetNumberOfConnectionsMiddleLayer())
                {
                    case 0:
                        return (block.block0Conn.prefab, block.block0Conn.rotationOffset);

                    case 1:
                        switch (Mbitmask)
                        {
                            case 1:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset);
                            case 2:
                                return (block.block0Conn.prefab, block.block0Conn.rotationOffset);

                        }
                        break;


                    case 2:
                        switch (Mbitmask)
                        {
                            case 3:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset);
                            case 5:
                                return (block.block2ConnCornerThin.prefab, block.block2ConnCornerThin.rotationOffset);
                            case 17:
                                return (block.block2ConnStraight.prefab, block.block2ConnStraight.rotationOffset);
                            case 33:

                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset);
                            case 36:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset + 90);
                            case 129:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset);
                        }
                        break;

                    case 3:
                        switch (Mbitmask)
                        {


                            case 7:

                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset);
                            case 69:
                                return (block.block3ConnT.prefab, block.block3ConnT.rotationOffset);

                            case 13:
                            case 37:
                            case 133:
                                return (block.block2ConnCornerThin.prefab, block.block2ConnCornerThin.rotationOffset);
                            case 25:
                            case 49:
                                return (block.block2ConnStraight.prefab, block.block2ConnStraight.rotationOffset);
                            case 41:
                            case 131:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset);
                            case 38:
                            case 134:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset + 90);
                            case 194:
                            case 200:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset - 90);

                        }
                        break;
                    case 4:
                        switch (Mbitmask)
                        {

                            case 23:
                                return (block.block3SpecialConnR.prefab, block.block3SpecialConnR.rotationOffset);
                            case 29:
                                return (block.block3SpecialConnL.prefab, block.block3SpecialConnL.rotationOffset);
                            case 85:
                                return (block.block4Conn.prefab, block.block4Conn.rotationOffset);
                            case 105:
                                return (block.block2ConnCornerThin.prefab, block.block2ConnCornerThin.rotationOffset - 90f);
                            case 15:
                            case 135:

                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset);
                            case 45:

                            case 141:
                                return (block.block2ConnCornerThin.prefab, block.block2ConnCornerThin.rotationOffset);
                            case 154:
                            case 178:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset + 180f);
                            case 51:
                            case 147:
                            case 153:
                                return (block.block2ConnStraight.prefab, block.block2ConnStraight.rotationOffset);
                            case 156:
                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset + 90f);
                            case 139:

                            case 163:

                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset);
                            case 170:
                                return (block.block0Conn.prefab, block.block0Conn.rotationOffset);
                            case 198:
                                return (block.block2ConnStraight.prefab, block.block2ConnStraight.rotationOffset + 90f);
                            case 86:
                            case 212:
                                return (block.block3ConnT.prefab, block.block3ConnT.rotationOffset + 180f);
                        }
                        break;

                    case 5:
                        switch (Mbitmask)
                        {
                            case 55:
                                return (block.block3SpecialConnR.prefab, block.block3SpecialConnR.rotationOffset);
                            case 93:
                                return (block.block4ConnSpecial.prefab, block.block4ConnSpecial.rotationOffset);
                            case 94:
                                return (block.block3SpecialConnR.prefab, block.block3SpecialConnR.rotationOffset + 90f);
                            case 107:
                                return (block.block2ConnCornerThin.prefab, block.block2ConnCornerThin.rotationOffset - 90f);
                            case 109:
                                return (block.block3ConnT.prefab, block.block3ConnT.rotationOffset);
                            case 143:
                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset);
                            case 157:
                                return (block.block3SpecialConnL.prefab, block.block3SpecialConnL.rotationOffset);
                            case 158:
                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset + 90f);
                            case 186:
                                return (block.block1Conn.prefab, block.block1Conn.rotationOffset + 180f);
                            case 199:
                                return (block.block3Balcony.prefab, block.block3Balcony.rotationOffset);
                            case 110:
                            case 236:
                                return (block.block2ConnStraight.prefab, block.block2ConnStraight.rotationOffset + 90f);
                            case 242:
                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset + 180f);
                            case 244:
                                return (block.block3SpecialConnL.prefab, block.block3SpecialConnL.rotationOffset + 90f);
                        }
                        break;
                    case 6:
                        switch (Mbitmask)
                        {
                            case 119:
                                return (block.block4ConnSpecialFourWay.prefab, block.block4ConnSpecialFourWay.rotationOffset);
                            case 125:
                                return (block.block3ConnTBalcony.prefab, block.block3ConnTBalcony.rotationOffset);
                            case 126:
                                return (block.block3Balcony.prefab, 180f + block.block3Balcony.rotationOffset);
                            case 190:
                                return (block.block2Corner.prefab, block.block2Corner.rotationOffset + 90f);
                            case 222:
                                return (block.block3ConnTSpecialL.prefab, block.block3ConnTSpecialL.rotationOffset);
                            case 238:
                                return (block.block2ConnStraight.prefab, block.block2ConnStraight.rotationOffset + 90);
                            case 243:
                                return (block.block3Balcony.prefab, block.block3Balcony.rotationOffset - 90f);
                            case 246:
                                return (block.block3ConnTSpecialR.prefab, block.block3ConnTSpecialR.rotationOffset);
                        }
                        break;

                    case 7:
                        switch (Mbitmask)

                        {
                            case 127:
                                return (block.block4InnerCorner.prefab, block.block4InnerCorner.rotationOffset);
                            case 254:
                                return (block.block3Balcony.prefab, 180f + block.block3Balcony.rotationOffset);

                        }
                        break;

                    case 8:
                        return (block.block4Floor.prefab, block.block4Floor.rotationOffset);

                }
            }
            catch (Exception)
            {
                Debug.LogWarning($"Using basic shape for this combination {Mbitmask} as there is no tile block assigned in " + block.name);
                return (block.block0Conn.prefab, block.block0Conn.rotationOffset);
            }


            return (null, 0);
        }
        public int GetBottomLayerBitMask()
        {
            Bbitmask = 0;
            //First Row 128 | 1 | 2
            Bbitmask = BottomLayer[0][0] ? 128 : 0;
            Bbitmask = BottomLayer[0][1] ? Bbitmask + 1 : Bbitmask;
            Bbitmask = BottomLayer[0][2] ? Bbitmask + 2 : Bbitmask;
            //Second Row 64 | 256 | 4
            Bbitmask = BottomLayer[1][0] ? Bbitmask + 64 : Bbitmask;
            Bbitmask = BottomLayer[1][1] ? Bbitmask + 256 : Bbitmask;
            Bbitmask = BottomLayer[1][2] ? Bbitmask + 4 : Bbitmask;
            //Third Row 32 | 16 | 8
            Bbitmask = BottomLayer[2][0] ? Bbitmask + 32 : Bbitmask;
            Bbitmask = BottomLayer[2][1] ? Bbitmask + 16 : Bbitmask;
            Bbitmask = BottomLayer[2][2] ? Bbitmask + 8 : Bbitmask;


            return Bbitmask;

        }
        public int GetMiddleLayerBitMask()
        {
            Mbitmask = 0;
            //First Row 128 | 1 | 2
            Mbitmask = MiddleLayer[0][0] ? 128 : 0;
            Mbitmask = MiddleLayer[0][1] ? Mbitmask + 1 : Mbitmask;
            Mbitmask = MiddleLayer[0][2] ? Mbitmask + 2 : Mbitmask;
            //Second Row 64 | C | 4
            Mbitmask = MiddleLayer[1][0] ? Mbitmask + 64 : Mbitmask;
            Mbitmask = MiddleLayer[1][2] ? Mbitmask + 4 : Mbitmask;
            //Third Row 32 | 16 | 8
            Mbitmask = MiddleLayer[2][0] ? Mbitmask + 32 : Mbitmask;
            Mbitmask = MiddleLayer[2][1] ? Mbitmask + 16 : Mbitmask;
            Mbitmask = MiddleLayer[2][2] ? Mbitmask + 8 : Mbitmask;



            return Mbitmask;

        }
        public int GetTopLayerBitMask()
        {
            Tbitmask = 0;
            //First Row 128 | 1 | 2
            Tbitmask = TopLayer[0][0] ? 128 : 0;
            Tbitmask = TopLayer[0][1] ? Tbitmask + 1 : Tbitmask;
            Tbitmask = TopLayer[0][2] ? Tbitmask + 2 : Tbitmask;
            //Second Row 64 | 256 | 4
            Tbitmask = TopLayer[1][0] ? Tbitmask + 64 : Tbitmask;
            Tbitmask = TopLayer[1][1] ? Tbitmask + 256 : Tbitmask;
            Tbitmask = TopLayer[1][2] ? Tbitmask + 4 : Tbitmask;
            //Third Row 32 | 16 | 8
            Tbitmask = TopLayer[2][0] ? Tbitmask + 32 : Tbitmask;
            Tbitmask = TopLayer[2][1] ? Tbitmask + 16 : Tbitmask;
            Tbitmask = TopLayer[2][2] ? Tbitmask + 8 : Tbitmask;



            return Tbitmask;

        }

        public void ShiftBitMask()
        {
           // Debug.Log("Before shift: " + Mbitmask);
            if (Mbitmask == -1) GetMiddleLayerBitMask();
            if (Mbitmask == 0) return;
            Mbitmask = ((Mbitmask << 1) | (Mbitmask >> 7)) & 255;
           // Debug.Log("After shift: " + Mbitmask);

        }
        public void ShiftBitMask90()
        {
            if (Bbitmask == -1) GetBottomLayerBitMask();
            if (Mbitmask == -1) GetMiddleLayerBitMask();
            if (Tbitmask == -1) GetTopLayerBitMask();
            if (Mbitmask == 0) return;


            Bbitmask = ((Bbitmask >> 2) | (Bbitmask << 6)) & 512;
            Mbitmask = ((Mbitmask >> 2) | (Mbitmask << 6)) & 255;
            Tbitmask = ((Tbitmask >> 2) | (Tbitmask << 6)) & 512;

        }
        public int GetNumberOfConnectionsMiddleLayer()
        {
            int counter = 0;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (MiddleLayer[i][j])
                        counter++;
            return counter;
        }
        //Rotate matrix
        //Give block solution based on matrix position in middle layer
        // 128   | 1   | 2
        //  64   + 0   + 4 -> bitmask, each cell is (2^n-1), to rotate it is (n*2-1)mod128
        //  32   | 16  | 8
        //^
        //|
        //+--> forward and right vectors
        //1.Connection cases
        //Edge(1)
        // 0 | 1 | 0
        // 0 + 0 + 0 -> block1Conn
        // 0 | 0 | 0
        ////////////////////////////////////////
        ///Corner(2)
        // 0 | 0 | 1
        // 0 + 0 + 0 -> block0Conn (only edges, no corner neighbours)
        // 0 | 0 | 0
        ////////////////////////////////////////
        //2.Connection cases
        //Edge and corner(3)
        // 0 | 1 | 1
        // 0 + 0 + 0 -> block1Conn
        // 0 | 0 | 0
        //(129)
        // 0 | 1 | 1
        // 0 + 0 + 0 -> block1Conn
        // 0 | 0 | 0
        //2 Edges straight(17)
        // 0 | 1 | 0
        // 0 + 0 + 0 -> block2ConnStraight
        // 0 | 1 | 0
        //2 Edges L shape(5)
        // 0 | 1 | 0
        // 0 + 0 + 1 -> block2ConnCornerThin
        // 0 | 0 | 0
        //(33)
        // 0 | 1 | 0
        // 0 + 0 + 0 -> block1Conn
        // 1 | 0 | 0
        //(36)
        // 0 | 0 | 0
        // 0 + 0 + 1 -> block1Conn
        // 1 | 0 | 0
        ////////////////////////////////////////
        //3. Connection cases
        //2 Edges T shape(69)
        // 0 | 1 | 0
        // 1 + 0 + 1 -> block3ConnT
        // 0 | 0 | 0
        //(133)
        // 1 | 1 | 0
        // 0 + 0 + 1 -> block2ConnCornerThin
        // 0 | 0 | 0
        //(134)
        // 1 | 0 | 1
        // 0 + 0 + 1 -> block1Conn
        // 0 | 0 | 0
        //(194)
        // 1 | 0 | 1
        // 1 + 0 + 0 -> block1Conn
        // 0 | 0 | 0
        //2 Edges and corner(7)
        // 0 | 1 | 1
        // 0 + 0 + 1 -> block2Corner
        // 0 | 0 | 0
        //(37)
        // 0 | 1 | 0
        // 0 + 0 + 1 -> block2ConnCornerThin
        // 1 | 0 | 0
        //(38)
        // 0 | 0 | 1
        // 0 + 0 + 1 -> block1Conn
        // 1 | 0 | 0
        //(41)
        // 0 | 1 | 0
        // 0 + 0 + 0 -> block1Conn
        // 1 | 0 | 1
        //(200)
        // 1 | 0 | 0
        // 1 + 0 + 0 -> block1Conn
        // 0 | 0 | 1
        //(49)
        // 0 | 1 | 0
        // 0 + 0 + 0 -> block2ConnStraight
        // 1 | 1 | 0
        //(25)
        // 0 | 1 | 0
        // 0 + 0 + 0 -> block2ConnStraight
        // 1 | 1 | 0
        //(131)
        // 1 | 1 | 1
        // 0 + 0 + 0 -> block1Conn
        // 0 | 0 | 0
        //(13)
        // 0 | 1 | 0
        // 0 + 0 + 1 -> block2ConnCornerThin
        // 0 | 0 | 1
        //Any other case with L shape and neighbour in other corners -> block2ConnCornerThin
        //4. Connection cases
        //4Edges(85)
        // 0 | 1 | 0
        // 1 + 0 + 1 -> block4Conn
        // 0 | 1 | 0
        //(86)
        // 0 | 0 | 1
        // 1 + 0 + 1 -> block3ConnT
        // 0 | 1 | 0
        //(212)
        // 1 | 0 | 0
        // 1 + 0 + 1 -> block3ConnT
        // 0 | 1 | 0
        //(15)
        // 0 | 1 | 1
        // 0 + 0 + 1 -> block2Corner
        // 0 | 0 | 1
        //(29)
        // 0 | 1 | 0
        // 0 + 0 + 1 -> block3SpecialConnL
        // 0 | 1 | 1
        //(135)
        // 1 | 1 | 1
        // 0 + 0 + 1 -> block2Corner
        // 0 | 0 | 0
        //(139)
        // 1 | 1 | 1
        // 0 + 0 + 0 -> block1Conn
        // 0 | 0 | 1
        //(178)
        // 1 | 0 | 1
        // 0 + 0 + 0 -> block1Conn
        // 1 | 1 | 0
        //(154)
        // 1 | 0 | 1
        // 0 + 0 + 0 -> block1Conn
        // 0 | 1 | 1
        //(156)
        // 1 | 0 | 0
        // 0 + 0 + 1 -> block2Corner
        // 0 | 1 | 1
        //(163)
        // 1 | 1 | 1
        // 0 + 0 + 0 -> block1Conn
        // 1 | 0 | 0
        //(141)
        // 1 | 1 | 0
        // 0 + 0 + 1 -> block2ConnCornerThin
        // 0 | 0 | 1
        //(105)
        // 0 | 1 | 0
        // 1 + 0 + 0 -> block2ConnCornerThin
        // 1 | 0 | 1
        //(45)
        // 0 | 1 | 0
        // 0 + 0 + 1 -> block2ConnCornerThin
        // 1 | 0 | 1
        //(51)
        // 0 | 1 | 1
        // 0 + 0 + 0 -> block2ConnStraight
        // 1 | 1 | 0
        //(153)
        // 1 | 1 | 0
        // 0 + 0 + 0 -> block2ConnStraight
        // 0 | 1 | 1
        //(23)
        // 0 | 1 | 1
        // 0 + 0 + 1 -> block3Balcony
        // 0 | 1 | 0
        //(170)
        // 1 | 0 | 1
        // 0 + 0 + 0 -> block0Conn
        // 1 | 0 | 1
        //(147)
        // 1 | 1 | 1
        // 0 + 0 + 0 -> block2ConnStraight
        // 0 | 1 | 0
        //(198)
        // 1 | 0 | 1
        // 1 + 0 + 1 -> block2ConnStraight
        // 0 | 0 | 0

        //any other->block2Corner
        //5. Connection cases
        //5Edges(93)
        // 0 | 1 | 0
        // 1 + 0 + 1 -> block4ConnSpecial
        // 0 | 1 | 1
        //5Edges(109)
        // 0 | 1 | 0
        // 1 + 0 + 1 -> block3ConnT
        // 1 | 0 | 1
        //(107)
        // 0 | 1 | 1
        // 1 + 0 + 0 -> block2ConnCornerThin
        // 1 | 0 | 1
        //(236)
        // 1 | 0 | 0
        // 1 + 0 + 1 -> block2ConnStraight
        // 1 | 0 | 1
        //(110)
        // 0 | 0 | 1
        // 1 + 0 + 1 -> block2ConnStraight
        // 1 | 0 | 1
        //(186)
        // 1 | 0 | 1
        // 0 + 0 + 0-> block1Conn
        // 1 | 1 | 1
        //5Edges(199)
        // 1 | 1 | 1
        // 1 + 0 + 1 -> block3Balcony
        // 0 | 0 | 0
        //(158)
        // 1 | 0 | 1
        // 0 + 0 + 1 -> block2Corner
        // 0 | 1 | 1
        //(242)
        // 1 | 0 | 1
        // 1 + 0 + 0 -> block2Corner
        // 1 | 1 | 0
        //(143)
        // 1 | 1 | 1
        // 0 + 0 + 1 -> block2Corner
        // 0 | 0 | 1
        //(94)
        // 0 | 0 | 1
        // 1 + 0 + 1 -> block3SpecialConnR
        // 0 | 1 | 1
        //(244)
        // 1 | 0 | 0
        // 1 + 0 + 1 -> block3SpecialConnL
        // 1 | 1 | 0
        //(55)
        // 1 | 1 | 1
        // 0 + 0 + 1 -> block3SpecialConnR
        // 1 | 0 | 0
        //(157)
        // 1 | 1 | 0
        // 0 + 0 + 1 -> block3SpecialConnL
        // 0 | 1 | 1


        //6. Connection cases
        //(126)
        // 0 | 0 | 1
        // 1 + 0 + 1 -> block3Balcony
        // 1 | 1 | 1
        //(243)
        // 1 | 1 | 1
        // 1 + 0 + 0 -> block3Balcony
        // 1 | 1 | 0
        //(238)
        // 1 | 0 | 1
        // 1 + 0 + 1 -> block2ConnStraight
        // 1 | 0 | 1
        //(246)
        // 1 | 0 | 1
        // 1 + 0 + 1 -> block3ConnTSpecialR
        // 1 | 1 | 0
        //(222)
        // 1 | 0 | 1
        // 1 + 0 + 1 -> block3ConnTSpecialL
        // 0 | 1 | 1
        //(125)
        // 0 | 1 | 0
        // 1 + 0 + 1 -> block3ConnTBalcony
        // 1 | 1 | 1
        //(119)
        // 0 | 1 | 1
        // 1 + 0 + 1 -> block4ConnSpecialFourWay
        // 1 | 1 | 0
        //(190)
        // 1 | 0 | 1
        // 0 + 0 + 1 -> block2Corner
        // 1 | 1 | 1

        //7. Connection cases
        //(127)
        // 0 | 1 | 1
        // 1 + 0 + 1 -> block4Conn
        // 1 | 1 | 1
        //(254)
        // 1 | 0 | 1
        // 1 + 0 + 1 -> block3Balcony
        // 1 | 1 | 1
        //8. Connection cases-> block4Floor

    }


    [Serializable]
    public class TileBlockConfig
    {
        [SerializeField]
        public GameObject prefab;
        [SerializeField]
        [Tooltip("Rotation adjustment in Y axis(0, 90, 180, 270)")]
        public float rotationOffset;
    }
}