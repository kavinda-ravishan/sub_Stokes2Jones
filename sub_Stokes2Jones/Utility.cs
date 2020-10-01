using System.Collections.Generic;

namespace sub_GPIB
{
    static class Utility
    {
        //---------------------- For testing ------------------------------------------------------------------------------------------------//

        public static string text_S0 = "VAL00  77.204;VAL01  16.427;VAL02   0.295;VAL03  39.486;VAL04   0.371;VAL05   0.121;VAL06  56.222;VAL07   0.000;VAL08  10.609;VAL09  -0.758;VAL10   0.363;VAL11   0.543;VAL12 -75.284;VAL13 -71.248;VAL14 -73.429;1000;E08\n";

        public static string text_SB = "S1  0.849;S2  0.528;S3  0.007;PDB -76.34;1000;E00\n";

        public static string text_J1_1 = "J[11]  0.902 -45.363;J[12]  0.383 -179.843;J[21]  0.420 -4.662;J[22]  0.931 46.226;1000;E00\n";
        //1550.01 nm
        public static string text_J1_2 = "J[11]  0.907 -44.460;J[12]  0.405 -178.349;J[21]  0.416 -3.936;J[22]  0.917 44.923;1000;E00\n";

        //1550.00 nm
        public static string text_J1 = "J[11]  0.870  7.368;J[12]  0.557 -138.518;J[21]  0.646 -39.584;J[22]  0.736 -8.434;1000;E00\n";
        //1551.00 
        public static string text_J2 = "J[11]  0.797 -64.374;J[12]  0.605 -66.324;J[21]  0.600 -111.640;J[22]  0.799 63.215;1000;E00\n";

        //-----------------------------------------------------------------------------------------------------------------------------------//
        public static string ReplaceCommonEscapeSequences(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }
        public static string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }

        private const double C = 299792458;

        private static List<string> DataSeparator(string text)
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

        private static string[] SB_filter(List<string> text)
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

        private static double[] SB_String2Double(string[] values)
        {
            double[] stokes = new double[4];

            for(int i = 0; i < 4; i++)
            {
                stokes[i] = System.Convert.ToDouble(values[i]);
            }

            return stokes;
        }

        public static double[] SB2Stokes(string text)
        {
            return SB_String2Double(SB_filter(DataSeparator(text)));
        }// S1, S2, S3, PDB

        private static double Wavelength2Frequency(double wavelength)//wavelength in nm and frequency in THz
        {
            return C / (wavelength * 1000);
        }
    }
}
