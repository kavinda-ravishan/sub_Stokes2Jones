using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;

namespace sub_Stokes2Jones
{
    static class Program
    {

        static string MsgWaveLenghtSrc(double waveLenght = 1551.120)
        {
            return ":WAVElength " + waveLenght.ToString() + "nm";
        }

        static string MsgPowerSrc(double power = 1000)
        {
            return ":POWer " + power.ToString() + "uW";
        }

        static string MsgWaveLenghtPol(double waveLenght = 1551.12)
        {
            return "L " + waveLenght.ToString() + ";X;";
        }

        static string MsgPolPosition(double theta = 179.82)
        {
            //POS 0;X;
            return "POS " + theta.ToString() + ";X;";
        }


        static void InitMesure(Device Source, Device PolarizationAnalyzer, double power = 1000)
        {
            Source.Write(Utility.ReplaceCommonEscapeSequences(MsgPowerSrc(power))); // set power to 1200uW
            Source.Write(Utility.ReplaceCommonEscapeSequences(":OUTPut 1")); // turn on the laser
            Console.WriteLine("Laser is ON !");

            PolarizationAnalyzer.Write(Utility.ReplaceCommonEscapeSequences("R 8;X;"));
            PolarizationAnalyzer.Write(Utility.ReplaceCommonEscapeSequences("A 8;X;"));
        }

        static ComplexCar KMesure(Device PolarizationAnalyzer, double polPos, int polDelay = 3000)
        {
            Console.WriteLine("Set Polarizer to - " + polPos.ToString());
            PolarizationAnalyzer.Write(Utility.ReplaceCommonEscapeSequences(MsgPolPosition(polPos)));//change pol position

            System.Threading.Thread.Sleep(polDelay);

            Console.WriteLine("Read SB at - " + polPos.ToString());
            PolarizationAnalyzer.Write(Utility.ReplaceCommonEscapeSequences("SB;"));

            string stokesString = Utility.InsertCommonEscapeSequences(PolarizationAnalyzer.ReadString());//read Stokes
            double[] stokes = Utility.SB2Stokes(stokesString);

            return Utility.Stokes2K(stokes[0], stokes[1], stokes[2]);
        }

        static JonesMatCar MesureJonesMat(Device Source, Device PolarizationAnalyzer, double wavelenght, int delay)
        {
            Console.WriteLine("Set Source  WL - " + wavelenght.ToString());
            Source.Write(Utility.ReplaceCommonEscapeSequences(MsgWaveLenghtSrc(wavelenght)));//change wavelength source

            Console.WriteLine("Set PAT9000 WL - " + wavelenght.ToString());
            PolarizationAnalyzer.Write(Utility.ReplaceCommonEscapeSequences(MsgWaveLenghtPol(wavelenght)));//change wavelength pol
            System.Threading.Thread.Sleep(delay);

            ComplexCar k0 = KMesure(PolarizationAnalyzer, 0);
            ComplexCar k45 = KMesure(PolarizationAnalyzer, 45);
            ComplexCar k90 = KMesure(PolarizationAnalyzer, 90);

            Console.WriteLine();

            return Utility.K2JonesMat(k0, k90, k45);
        }

        static void Done(Device Source)
        {
            Source.Write(Utility.ReplaceCommonEscapeSequences(":OUTPut 0")); // turn off the laser
            Console.WriteLine("Laser is OFF !");
        }

        static void Mesure()
        {
            Device PolarizationAnalyzer = new Device(0, 9, 0);
            Device Source = new Device(0, 24, 0);

            double start = 1550;
            double end = 1560;
            double stepSize = 1;

            int steps = (int)((end - start) / stepSize) + 3;

            double[] wavelenght = new double[steps];

            //find wavelenghts need to mesure
            for (int i = 0; i < steps; i++)
            {
                wavelenght[i] = (start - stepSize) + (i * stepSize);
            }

            //for save PAT9300 JM information
            JonesMatCar[] jMat = new JonesMatCar[steps];

            double[] DGDval = new double[2];
            double[,] DGDs = new double[steps - 2, 2];

            int delay = 5000;

            InitMesure(Source, PolarizationAnalyzer);

            for (int i = 0; i < steps; i++)
            {
                if (i < 2)
                {
                    jMat[i] = MesureJonesMat(Source, PolarizationAnalyzer, wavelenght[i], delay);
                }
                else
                {
                    jMat[i] = MesureJonesMat(Source, PolarizationAnalyzer, wavelenght[i], delay);

                    DGDval = Utility.DGD(jMat[i - 2], jMat[i], wavelenght[i - 2], wavelenght[i]);
                    DGDs[i - 2, 0] = DGDval[0];
                    DGDs[i - 2, 1] = DGDval[1];

                    Console.WriteLine(DGDval[0]);
                    Console.WriteLine(DGDval[1]);
                    Console.WriteLine();
                }
            }

            Done(Source); // turn off the laser


            PolarizationAnalyzer.Dispose();
            Source.Dispose();
        }


        static void Main(string[] args)
        {
            //Mesure();


            double[] values = Utility.JM2JonesMatValues(Utility.text_J1);

            for (int i = 0; i < values.Length; i++)
            {
                Console.WriteLine(values[i]);
            }

            Console.Read();
        }
    }
}