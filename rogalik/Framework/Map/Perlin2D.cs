using System;

namespace rogalik.Framework.Map;

class Perlin2D
{
    private readonly byte[] _permutationTable;

    public Perlin2D(Random rnd)
    {
        _permutationTable = new byte[1024];
        rnd.NextBytes(_permutationTable);
    }

    private record struct PointFloat(float X, float Y);
    
    private PointFloat GetPseudoRandomGradientVector(int x, int y)
    {
        var v = (int)(((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023);
        v = _permutationTable[v]&3;

        return v switch
        {
            0 => new PointFloat( 1, 0 ),
            1 => new PointFloat( -1, 0 ),
            2 => new PointFloat( 0, 1 ),
            _ => new PointFloat( 0, -1 )
        };
    }

    private static float Smooth(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private static float ScalarProduct(PointFloat a, PointFloat b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public float Noise(float fx, float fy)
    {
        var left = (int)Math.Floor(fx);
        var top  = (int)Math.Floor(fy);
        var pointInQuadX = fx - left;
        var pointInQuadY = fy - top;

        var topLeftGradient     = GetPseudoRandomGradientVector(left,   top  );
        var topRightGradient    = GetPseudoRandomGradientVector(left+1, top  );
        var bottomLeftGradient  = GetPseudoRandomGradientVector(left,   top+1);
        var bottomRightGradient = GetPseudoRandomGradientVector(left+1, top+1);

        var toTopLeft     = new PointFloat(pointInQuadX, pointInQuadY);
        var toTopRight    = new PointFloat(pointInQuadX-1, pointInQuadY);
        var toBottomLeft  = new PointFloat(pointInQuadX, pointInQuadY-1);
        var toBottomRight = new PointFloat(pointInQuadX-1, pointInQuadY-1);

        var tx1 = ScalarProduct(toTopLeft,     topLeftGradient);
        var tx2 = ScalarProduct(toTopRight,    topRightGradient);
        var bx1 = ScalarProduct(toBottomLeft,  bottomLeftGradient);
        var bx2 = ScalarProduct(toBottomRight, bottomRightGradient);

        pointInQuadX = Smooth(pointInQuadX);
        pointInQuadY = Smooth(pointInQuadY);

        var tx = Lerp(tx1, tx2, pointInQuadX);
        var bx = Lerp(bx1, bx2, pointInQuadX);
        var tb = Lerp(tx, bx, pointInQuadY);

        return tb;
    }

    public float Noise(float fx, float fy, int octaves, float persistence = 0.5f)
    {
        float amplitude = 1;
        float max = 0;
        float result = 0;

        while (octaves-- > 0)
        {
            max += amplitude;
            result += Noise(fx, fy) * amplitude;
            amplitude *= persistence;
            fx *= 2;
            fy *= 2;
        }

        return result/max;
    }
}