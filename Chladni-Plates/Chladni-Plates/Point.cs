namespace Chladni_Plates
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int I { get; set; }

        public Point(int x, int y, int i)
        {
            X = x;
            Y = y;
            I = i;
        }
    }
}
