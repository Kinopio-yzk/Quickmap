using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public class moProjection
    {

        private static double DegreeToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        private static double RadToDegree(double rad)
        {
            return rad / Math.PI * 180;
        }

        public static moPoint PtoG(moPoint P)
        {
            double lamda0 = 2 * Math.PI / 3;
            double lamda = DegreeToRad(P.X);
            double fai = DegreeToRad(P.Y);
            double x, y;
            x = lamda - lamda0;
            y = Math.Log(Math.Tan(fai) + 1 / Math.Cos(fai));
            moPoint G = new moPoint(x*10000, y*10000);
            return G;
        }

        public static moPoint GtoP(moPoint G)
        {
            double lamda0 = 2 * Math.PI / 3;
            double x = G.X / 10000;
            double y = G.Y / 10000;
            double fai, lamda;
            lamda = RadToDegree(x + lamda0);
            fai = RadToDegree(Math.Atan(Math.Sinh(y)));
            moPoint P = new moPoint(lamda,fai);
            return P;
           
        }
    }
}
