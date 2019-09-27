using System;
using System.IO;
using CommandLine;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.BTMX;
using NationalInstruments.RFmx.WlanMX;
using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.RFmx.NRMX;
using NationalInstruments.RFmx.LteMX;
using NationalInstruments.ModularInstruments.NIRfsa;
using NationalInstruments;
using CommandLine.Text;
using System.Collections.Generic;

namespace RFmxIQCaptureTool
{
    class Program
    {
        public class Options
        {
            [Option('i', "instr", Required = true, HelpText = "The instrument name to use for acquisition.")]
            public string instrName { get; set; }
            [Option('f', "path", Required = true, HelpText = "The TDMS file path defining the RFmx configuration.")]
            public string inputPath { get; set; }
            [Option('o', "output", Required = false, HelpText = "The output folder for the IQ data.")]
            public string outputPath { get; set; }

            [Usage(ApplicationAlias = "RFmxIQCaptureTool")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    return new List<Example>()
                    {
                        new Example("Load an RFmx configuration and output IQ data", new Options { instrName = "5840", inputPath = "SampleConfig.tdms" }),
                        new Example("Load an RFmx configuration and output IQ data to a specified folder", 
                                new Options { instrName = "5840", inputPath = "SampleConfig.tdms", outputPath = "..\\Files" })
                    };
                }
            }
        }
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                // Only run if all arguments are present
                LoadAndRunMeasurements(o.instrName, o.inputPath, o.outputPath);
            });
            Console.WriteLine("\n\nPress any key to exit.");
            Console.ReadKey();
        }
        static void LoadAndRunMeasurements(string instrName, string inputPath, string outputPath)
        { 
            inputPath = Path.GetFullPath(inputPath);
            RFmxInstrMX instr = null;
            NIRfsa rfsa = null;
            IntPtr rfsaHandle;

            try
            {
                Console.WriteLine($"Initializing RFmx session with instrument \"{instrName}\"...");
                instr = new RFmxInstrMX(instrName, "");

                Console.WriteLine($"Loading configuration from \"{Path.GetFileName(inputPath)}\"...");
                instr.LoadAllConfigurations(inputPath, true);
                instr.DangerousGetNIRfsaHandle(out rfsaHandle);
                rfsa = new NIRfsa(rfsaHandle);

                string[] signalNames = new string[0];
                RFmxInstrMXPersonalities[] personalities = new RFmxInstrMXPersonalities[0];

                Console.WriteLine("Configuration loaded successfully.");

                instr.GetSignalConfigurationNames("", RFmxInstrMXPersonalities.All, ref signalNames, ref personalities);
                for (int i = 0; i < signalNames.Length; i++)
                {
                    Console.WriteLine("");
                    ConsoleKeyInfo info;
                    switch (personalities[i])
                    {
                        case RFmxInstrMXPersonalities.BT:
                            RFmxBTMX bt = instr.GetBTSignalConfiguration(signalNames[i]);
                            Console.WriteLine($"Enter 'y' to initiate acquisition for RFmx Bluetooth with signal \"{signalNames[i]}\"; any other key to skip.");
                            info = Console.ReadKey();
                            Console.WriteLine();
                            if (info.KeyChar == 'y')
                            { 
                                bt.Initiate("", "");
                                bt.WaitForMeasurementComplete("", 10);
                                FetchAndLog(rfsa, personalities[i], signalNames[i], outputPath);
                            }
                            bt.Dispose();
                            break;
                        case RFmxInstrMXPersonalities.Wlan:
                            RFmxWlanMX wlan = instr.GetWlanSignalConfiguration(signalNames[i]);
                            Console.WriteLine($"Enter 'y' to initiate acquisition for RFmx WLAN with signal \"{signalNames[i]}\"; any other key to skip.");
                            info = Console.ReadKey();
                            Console.WriteLine();
                            if (info.KeyChar == 'y')
                            {
                                wlan.Initiate("", "");
                                wlan.WaitForMeasurementComplete("", 10);
                                FetchAndLog(rfsa, personalities[i], signalNames[i], outputPath);
                            }
                            wlan.Dispose();
                            break;
                        case RFmxInstrMXPersonalities.SpecAn:
                            RFmxSpecAnMX specAn = instr.GetSpecAnSignalConfiguration(signalNames[i]);
                            Console.WriteLine($"Enter 'y' to initiate acquisition for RFmx SpecAn with signal \"{signalNames[i]}\"; any other key to skip.");
                            info = Console.ReadKey();
                            Console.WriteLine();
                            if (info.KeyChar == 'y')
                            {
                                specAn.Initiate("", "");
                                specAn.WaitForMeasurementComplete("", 10);
                                FetchAndLog(rfsa, personalities[i], signalNames[i], outputPath);
                            }
                            specAn.Dispose();
                            break;
                        case RFmxInstrMXPersonalities.NR:
                            RFmxNRMX nr = instr.GetNRSignalConfiguration(signalNames[i]);
                            Console.WriteLine($"Enter 'y' to initiate acquisition for RFmx NR with signal \"{signalNames[i]}\"; any other key to skip.");
                            info = Console.ReadKey();
                            Console.WriteLine();
                            if (info.KeyChar == 'y')
                            {
                                nr.Initiate("", "");
                                nr.WaitForMeasurementComplete("", 10);
                                FetchAndLog(rfsa, personalities[i], signalNames[i], outputPath);
                            }
                            nr.Dispose();
                            break;
                        case RFmxInstrMXPersonalities.Lte:
                            RFmxLteMX lte = instr.GetLteSignalConfiguration(signalNames[i]);
                            Console.WriteLine($"Enter 'y' to initiate acquisition for RFmx LTE with signal \"{signalNames[i]}\"; any other key to skip.");
                            info = Console.ReadKey();
                            Console.WriteLine();
                            if (info.KeyChar == 'y')
                            {
                                lte.Initiate("", "");
                                lte.WaitForMeasurementComplete("", 10);
                                FetchAndLog(rfsa, personalities[i], signalNames[i], outputPath);
                            }
                            lte.Dispose();
                            break;
                        default:
                            throw new System.NotImplementedException($"The \"{personalities[i].ToString()}\" personality has not been implemented.");
                    }
                }
                Console.WriteLine("All measurements complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred: " + ex.Message);
                Console.WriteLine("Location: " + ex.StackTrace);
            }
            finally
            {
                if (instr != null) instr.Dispose();
            }
        }
        private static void FetchAndLog(NIRfsa rfsa, 
            RFmxInstrMXPersonalities personality, string signalName, string folderPath)
        {
            RfsaAcquisitionType aqType = rfsa.Configuration.AcquisitionType;

            if (aqType == RfsaAcquisitionType.IQ)
            {
                ComplexDouble[,] data;

                PrecisionTimeSpan timeout = new PrecisionTimeSpan(10.0);
                double iqRate = rfsa.Configuration.IQ.IQRate;
                long records = rfsa.Configuration.IQ.NumberOfRecords;
                long samples = rfsa.Configuration.IQ.NumberOfSamples;

                // Fetch IQ data
                data = rfsa.Acquisition.IQ.FetchIQMultiRecordComplex<ComplexDouble>(0, records, samples, timeout);

                // Create file name
                string path = "";
                string fileName = $"{personality.ToString()}_{signalName}_IQ_{iqRate}.txt";

                // Combine the folder and file path if folder path is not empty; otherwise, just use the filename for path
                if (!string.IsNullOrEmpty(folderPath))
                    path = Path.Combine(folderPath, fileName);
                else
                    path = fileName;

                // Create an absolute path
                path = Path.GetFullPath(path);

                // Make sure the output directory exists if not currently
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                StreamWriter writer = new StreamWriter(path);

                for (int r = 0; r <= data.GetUpperBound(0); r++) // r = record
                {
                    for (int s = 0; s <= data.GetUpperBound(1); s++) // s = sample
                    {
                        writer.WriteLine(data[r, s].Real);
                        writer.WriteLine(data[r, s].Imaginary);
                    }
                }
                writer.Flush();
                writer.Close();
                Console.WriteLine($"IQ data successfully saved to \"{path}\".");
            }
            else
                Console.WriteLine("This measurement uses a spectral acquisition mode, which is not possible to fetch. This measurement will be skipped.");
        }
    }
}
