using System;
using MSI.Afterburner;


namespace AfterburnerMAHM
{
    class Program
    {
        static bool Continue = true;

        static int cpuUsageIndex    = -1;
        static int cpuTempIndex     = -1;
        static int gpuTempIndex     = -1;
        static int framerateIndex   = -1;
        static int powerIndex       = -1;
        static int gpuUsageIndex    = -1;
        static int memoryUsageIndex = -1;
        static int ramUsageIndex    = -1;

        static void Main(string[] args)
        {
            Console.Clear();
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("ctrl+c pressed");
                Continue = false;
                e.Cancel = true;
            };
            try
            {
                // connect to MACM shared memory
                HardwareMonitor mahm = new HardwareMonitor();

                // print out current MACM Header values
                Console.WriteLine("***** MSI AFTERTERBURNER HARDWARE MONITOR HEADER *****");
                Console.WriteLine(mahm.Header.ToString().Replace(";", "\n"));
                Console.WriteLine();

                // print out current MAHM GPU Entry values
                for (int i = 0; i < mahm.Header.GpuEntryCount; i++)
                {
                    Console.WriteLine("***** MSI AFTERTERBURNER GPU " + i + " *****");
                    Console.WriteLine(mahm.GpuEntries[i].ToString().Replace(";", "\n"));
                    Console.WriteLine();
                }

                // print out current Entry values
                for (int i = 0; i < mahm.Header.EntryCount; i++)
                {
                    if(mahm.Entries[i].SrcName == "CPU usage")
                    {
                        if (cpuUsageIndex == -1)
                        {
                            cpuUsageIndex = i;
                        }
                    }
                    else if(mahm.Entries[i].SrcName == "CPU1 temperature")
                    {
                        if (cpuTempIndex == -1)
                        {
                            cpuTempIndex = i;
                        }
                    }
                    else if (mahm.Entries[i].SrcName == "GPU temperature")
                    {
                        if(gpuTempIndex == -1)
                        {
                            gpuTempIndex = i;
                        }
                    }
                    else if (mahm.Entries[i].SrcName == "Framerate")
                    {
                        if (framerateIndex == -1)
                        {
                            framerateIndex = i;
                        }
                    }
                    else if (mahm.Entries[i].SrcName == "Power")
                    {
                        if (powerIndex == -1)
                        {
                            powerIndex = i;
                        }
                    }
                    else if (mahm.Entries[i].SrcName == "GPU usage")
                    {
                        if (gpuUsageIndex == -1)
                        {
                            gpuUsageIndex = i;
                        }
                    }
                    else if (mahm.Entries[i].SrcName == "Memory usage")
                    {
                        if (memoryUsageIndex == -1)
                        {
                            memoryUsageIndex = i;
                        }
                    }
                    else if (mahm.Entries[i].SrcName == "RAM usage")
                    {
                        if (ramUsageIndex == -1)
                        {
                            ramUsageIndex = i;
                        }
                    }

                    Console.WriteLine("***** MSI AFTERTERBURNER DATA SOURCE " + i + " *****");
                    Console.WriteLine(mahm.Entries[i].ToString().Replace(";", "\n"));
                    Console.WriteLine();
                }
                Console.WriteLine(mahm.Entries.Length);

                Console.WriteLine("Enter arduino COM port:");
                String comPort = Console.ReadLine();
                Console.WriteLine("Enter arduino baudRate:");
                int baudRate =Int32.Parse(Console.ReadLine());
                Arduino arduino = new Arduino();
                arduino.Connect(comPort, baudRate);
                
                // show a data source monitor several times
                HardwareMonitorEntry cpuUsageEntry = mahm.Entries[cpuUsageIndex];
                HardwareMonitorEntry cpuTempEntry = mahm.Entries[cpuTempIndex];
                HardwareMonitorEntry gpuTempEntry = mahm.Entries[gpuTempIndex]; //mahm.GetEntry(0, MONITORING_SOURCE_ID.GPU_TEMPERATURE);
                HardwareMonitorEntry framerateEntry = mahm.Entries[framerateIndex]; //mahm.GetEntry(HardwareMonitor.GPU_GLOBAL_INDEX, MONITORING_SOURCE_ID.FRAMERATE);
                HardwareMonitorEntry powerEntry = mahm.Entries[powerIndex];
                HardwareMonitorEntry gpuUsageEntry = mahm.Entries[gpuUsageIndex];
                HardwareMonitorEntry memoryUsageEntry = mahm.Entries[memoryUsageIndex];
                HardwareMonitorEntry ramUsageEntry = mahm.Entries[ramUsageIndex];

                if (framerateEntry != null && cpuUsageEntry != null && cpuTempEntry != null && gpuTempEntry != null)
                {
                    byte[] data = new byte[16];
                    UInt16 cpuUsage, gpuUsage, cpuTemp, gpuTemp, memoryUsage, ramUsage, framerate, powerUsage;
                    while (Continue)
                    {
                        cpuUsage    = (UInt16)cpuUsageEntry.Data;
                        gpuUsage    = (UInt16)gpuUsageEntry.Data;
                        cpuTemp     = (UInt16)cpuTempEntry.Data;
                        gpuTemp     = (UInt16)gpuTempEntry.Data;
                        memoryUsage = (UInt16)memoryUsageEntry.Data;
                        ramUsage    = (UInt16)ramUsageEntry.Data;
                        framerate   = (UInt16)framerateEntry.Data;
                        powerUsage  = (UInt16)powerEntry.Data;

                        data[0]     = (byte)(cpuUsage >> 8);
                        data[1]     = (byte)(cpuUsage);

                        data[2]     = (byte)(gpuUsage >> 8);
                        data[3]     = (byte)(gpuUsage);

                        data[4]     = (byte)(cpuTemp >> 8);
                        data[5]     = (byte)(cpuTemp);

                        data[6]     = (byte)(gpuTemp >> 8);
                        data[7]     = (byte)(gpuTemp);

                        data[8]     = (byte)(memoryUsage >> 8);
                        data[9]     = (byte)(memoryUsage);

                        data[10]    = (byte)(ramUsage >> 8);
                        data[11]    = (byte)(ramUsage);

                        data[12]    = (byte)(framerate >> 8);
                        data[13]    = (byte)(framerate);

                        data[14]    = (byte)(powerUsage >> 8);
                        data[15]    = (byte)(powerUsage);

                        arduino.write(data, 0, 16);

                        System.Threading.Thread.Sleep(500);
                        mahm.ReloadEntry(cpuUsageEntry);
                        mahm.ReloadEntry(gpuUsageEntry);
                        mahm.ReloadEntry(cpuTempEntry);
                        mahm.ReloadEntry(gpuTempEntry);
                        mahm.ReloadEntry(memoryUsageEntry);
                        mahm.ReloadEntry(ramUsageEntry);
                        mahm.ReloadEntry(framerateEntry);
                        mahm.ReloadEntry(powerEntry);
                    }
                }
                Console.WriteLine("Closing arduino connection!");
                arduino.close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                    Console.WriteLine(e.InnerException.Message);
            }

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
        }
    }
}
