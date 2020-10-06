using System;
using System.Collections.Generic;

namespace sub_Stokes2Jones
{
    static class Utility
    {
        #region For testing
        public static string text_S0 = "VAL00  77.204;VAL01  16.427;VAL02   0.295;VAL03  39.486;VAL04   0.371;VAL05   0.121;VAL06  56.222;VAL07   0.000;VAL08  10.609;VAL09  -0.758;VAL10   0.363;VAL11   0.543;VAL12 -75.284;VAL13 -71.248;VAL14 -73.429;1000;E08\n";

        public static string text_SB = "S1  0.849;S2  0.528;S3  0.007;PDB -76.34;1000;E00\n";

        public static string text_J1_1 = "J[11]  0.902 -45.363;J[12]  0.383 -179.843;J[21]  0.420 -4.662;J[22]  0.931 46.226;1000;E00\n";
        //1550.01 nm
        public static string text_J1_2 = "J[11]  0.907 -44.460;J[12]  0.405 -178.349;J[21]  0.416 -3.936;J[22]  0.917 44.923;1000;E00\n";

        //1550.00 nm
        public static string text_J1 = "J[11]  0.870  7.368;J[12]  0.557 -138.518;J[21]  0.646 -39.584;J[22]  0.736 -8.434;1000;E00\n";
        //1551.00 nm
        public static string text_J2 = "J[11]  0.797 -64.374;J[12]  0.605 -66.324;J[21]  0.600 -111.640;J[22]  0.799 63.215;1000;E00\n";

        public static string text_SC1 = "PSR 0.752;DEL   -9.137;TAN 0.575;PDB -74.55;1000;E00\n";
        public static string text_SC2 = "PSR 0.449;DEL    4.708;TAN 0.903;PDB -72.50;1000;E00\n";
        public static string text_SC3 = "PSR 0.714;DEL  170.466;TAN 0.632;PDB -74.29;1000;E00\n";
        #endregion

        public static string ReplaceCommonEscapeSequences(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }
        public static string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }

        public const double C = 299792458;

        public static List<string> DataSeparator(string text)
        {
            List<string> info = new List<string>();
            string value = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ';' || i == text.Length - 1)
                {
                    info.Add(value);
                    value = "";
                }
                else
                {
                    value += text[i];
                }
            }

            return info;
        }


        public static string[] SB_filter(List<string> text)
        {
            string[] SB = new string[6];

            for (int i = 0; i < 3; i++)
            {
                SB[i] = text[i].Substring(2);
            }

            SB[3] = text[3].Substring(3);
            SB[4] = text[4];
            SB[5] = text[5].Substring(0, 3);

            return SB;
        }

        public static string[] S0_filter(List<string> text)
        {
            string[] S0 = new string[17];

            for (int i = 0; i < 15; i++)
            {
                S0[i] = text[i].Substring(5);
            }
            S0[15] = text[15];
            S0[16] = text[16].Substring(0, 3);

            return S0;
        }

        public static string[] JM_filter(List<string> text)
        {
            string[] JMat = new string[10];

            for (int i = 0; i < 4; i++)
            {
                JMat[i * 2] = text[i].Substring(5, 7);
                JMat[(i * 2) + 1] = text[i].Substring(12, 7);
            }
            JMat[8] = text[4];
            JMat[9] = text[5].Substring(0, 3);

            return JMat;
        }

        public static string[] SC_filter(List<string> text)
        {
            string[] SC = new string[6];

            for (int i = 0; i < 4; i++)
            {
                SC[i] = text[i].Substring(3);
            }

            SC[4] = text[4];
            SC[5] = text[5].Substring(0, 3);

            return SC;
        }


        public static double[] SB_String2Double(string[] values)
        {
            double[] stokes = new double[4];

            for (int i = 0; i < 4; i++)
            {
                stokes[i] = System.Convert.ToDouble(values[i]);
            }

            return stokes;
        }

        public static double[] S0_String2Double(string[] values)
        {
            double[] S0 = new double[15];

            for (int i = 0; i < 15; i++)
            {
                S0[i] = System.Convert.ToDouble(values[i]);
            }

            return S0;
        }

        public static double[] JM_String2Double(string[] values)
        {
            double[] JM = new double[8];

            for (int i = 0; i < 8; i++)
            {
                JM[i] = System.Convert.ToDouble(values[i]);
            }

            return JM;
        }

        public static double[] SC_String2Double(string[] values)
        {
            double[] SC = new double[4];

            for (int i = 0; i < 4; i++)
            {
                SC[i] = System.Convert.ToDouble(values[i]);
            }

            return SC;
        }


        public static double[] SB2Stokes(string text)
        {
            return SB_String2Double(SB_filter(DataSeparator(text)));
        }// S1, S2, S3, PDB

        public static double[] S02PrimaryMeasurements(string text)
        {
            return S0_String2Double(S0_filter(DataSeparator(text)));
        }

        public static double[] JM2JonesMatValues(string text)
        {
            return JM_String2Double(JM_filter(DataSeparator(text)));
        }

        public static double[] SC2EybyExDelta(string text)
        {
            return SC_String2Double(SC_filter(DataSeparator(text)));
        }



        public static ComplexCar Stokes2K(double s1, double s2, double s3, double s0 = 1)
        {
            ComplexCar complexCar = new ComplexCar();

            double r = Math.Sqrt((s0 + s1) / (s0 - s1));
            double theta = -1 * Math.Atan(s3 / s2);

            complexCar.real = r * Math.Cos(theta);
            complexCar.imag = r * Math.Sin(theta);

            return complexCar;
        }

        public static ComplexCar TanPiDelta2K(double tanPi, double delta)//tanPi in deg
        {
            ComplexCar complexCar = new ComplexCar();

            double r = 1/tanPi;
            double theta = -1 * CMath.Deg2Red(delta);

            complexCar.real = r * Math.Cos(theta);
            complexCar.imag = r * Math.Sin(theta);

            return complexCar;
        }

        public static JonesMatCar K2JonesMat(ComplexCar k0,ComplexCar k90,ComplexCar k45)
        {
            JonesMatCar jonesMatCar = new JonesMatCar();

            ComplexCar k4 = (k90 - k45) / (k45 - k0);

            jonesMatCar.J11 = k0 * k4;
            jonesMatCar.J12 = k90;
            jonesMatCar.J21 = k4;
            jonesMatCar.J22 = new ComplexCar(1, 0);

            return jonesMatCar;
        }


        private static double Wavelength2Frequency(double wavelength)//wavelength in nm and frequency in THz
        {
            return C / (wavelength * 1000);
        }

        public static double[] DGD(JonesMatCar J1, JonesMatCar J2, double w1, double w2)
        {
            JonesMatCar J1Inv = CMath.Inverse(J1);

            JonesMatCar J2_J1Inv = J2 * J1Inv;

            ComplexCar[] complexCars = CMath.Eigenvalues(J2_J1Inv);

            ComplexPol[] complexPols = new ComplexPol[2];

            complexPols[0] = CMath.Car2Pol(complexCars[0]);
            complexPols[1] = CMath.Car2Pol(complexCars[1]);

            double Ang = complexPols[0].ang - complexPols[1].ang;

            double f1 = Wavelength2Frequency(w1);
            double f2 = Wavelength2Frequency(w2);

            double[] DGD_WL = { CMath.Abs(Ang / (f1 - f2)), (w1 + w2) / 2 };

            return DGD_WL;
        }
    }
}
