using sub_GPIB;
using System;

namespace sub_Stokes2Jones
{
    static class Program
    {
        static void Main(string[] args)
        {
            double[] stokes = Utility.SB2Stokes(Utility.text_SB);

            for (int i = 0; i < stokes.Length; i++)
            {
                Console.WriteLine(stokes[i]);
            }

            Console.Read();
        }
    }
}
