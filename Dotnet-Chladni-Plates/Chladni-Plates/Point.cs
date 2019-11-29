namespace Chladni_Plates
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y, double i)
        {
            X = x;
            Y = y;
            Z = i;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Z = default;
        }

        public override string ToString()
        {
            return $"Point({X}, {Y}, {Z})";
        }
    }
}
