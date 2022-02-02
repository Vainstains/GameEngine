using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VainEngine
{
    public class Atlas
    {
        public int width, height, cellWidth, cellHeight;
        public int numCellsX 
        {
            get
            {
                return width / cellWidth;
            }
        }
        public int numCellsY
        {
            get
            {
                return height / cellHeight;
            }
        }
        public Texture texture;
        public Atlas(int width_, int height_, int cellWidth_, int cellHeight_, Texture texture_)
        {
            width = width_;
            height = height_;
            cellWidth = cellWidth_;
            cellHeight = cellHeight_;
            texture = texture_;
        }
        public TexRect GetCell (int cell)
        {
            int x = cell % numCellsX;
            int y = (cell / numCellsX) % numCellsY;
            TexRect tr = new TexRect();
            tr.topLeft = new Vector2((((float)x * cellWidth)) / width, 1-((((float)y * cellHeight)) / height));
            tr.topRight = new Vector2((((float)x * cellWidth) + cellWidth) / width, 1 - ((((float)y * cellHeight)) / height));
            tr.bottomLeft = new Vector2((((float)x * cellWidth)) / width, 1 - ((((float)y * cellHeight) + cellHeight) / height));
            tr.bottomRight = new Vector2((((float)x * cellWidth) + cellWidth) / width, 1 - ((((float)y * cellHeight) + cellHeight) / height));
            return tr;
        }
        public struct TexRect 
        {
            public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        }
    }
}
