using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Management;

namespace denali_program {
    class Program {
        private static Process proc = new Process();
        private static bool debug = false;
        private static System.Threading.Timer close_program;
        private static string head = "1";
        private static GenBatFile genBatFile = new GenBatFile();
        static void Main(string[] args) {
            while (true) {
                try { head = File.ReadAllText("../../config/head.txt"); break; } catch { Thread.Sleep(50); }
            }
            //File.WriteAllText("../../config/denali_program_hex.txt", "Denali_v0000000031.mot");
            //File.WriteAllText("../../config/denali_program_e2lite.txt", "5ES009750");
            //File.WriteAllText("../../config/denali_program_checksum.txt", "7D9BA9B9");
            //File.WriteAllText("../../config/denali_program_name_project.txt", "Denali.rpj");

            File.Delete("../../config/head.txt");
            File.WriteAllText("call_exe_tric.txt", "");

            string hex_file = "Denali_v0000000031.mot";
            string e2lite = "5ES009750";
            string name_project = "Denali.rpj";
            int timeout = 20000;
            int timeout_bat = 20000;
            string checksum = "7D9BA9B9";
            string path_file = "D:\\svn\\2020_SENSITECTH_SugarLoaf_Automation\\2.Design Documents\\" +
                               "7.Test Application Program\\Bat file for program\\sugarloaf\\";
            try { hex_file = File.ReadAllText("../../config/denali_program_hex.txt"); } catch { }
            try { e2lite = File.ReadAllText("../../config/denali_program_e2lite.txt"); } catch { }
            try { timeout = Convert.ToInt32(File.ReadAllText("../../config/test_head_" + head + "_timeout.txt")); } catch { }
            try { debug = Convert.ToBoolean(File.ReadAllText("../../config/test_head_" + head + "_debug.txt")); } catch { }
            try { checksum = File.ReadAllText("../../config/denali_program_checksum.txt"); } catch { }
            try { name_project = File.ReadAllText("../../config/denali_program_name_project.txt"); } catch { }
            config_path:
            try { path_file = File.ReadAllText("../../config/denali_program_path_file.txt"); } catch { }
            try { timeout_bat = Convert.ToInt32(File.ReadAllText("../../config/test_head_" + head + "_timeout_bat.txt")); } catch { }
            close_program = new System.Threading.Timer(TimerCallback, null, 0, timeout);
            File.WriteAllText("denali_program_" + head + ".bat",
                              "\"C:\\Program Files (x86)\\Renesas Electronics\\programming Tools\\Renesas Flash Programmer " +
                              "V3.13\\RFPV3.exe\" /silent \"" + path_file + "\\" + name_project + "\" " +
                              "/file \"" + path_file + "\\" + hex_file + "\" /tool " + e2lite + " /log \"denali_program_" + head + ".log");

            while (true) {
                bool gggg = false;
                for (int hj = 1; hj <= 4; hj++) {
                    try {
                        File.ReadAllText("denali_program_discom_" + hj + ".txt");
                        gggg = true;
                    } catch { }
                }
                if (!gggg) break;
                Thread.Sleep(50);
            }
            File.WriteAllText("denali_program_running_" + head + ".txt", "");

            Console.WriteLine("head = " + head);
            Console.WriteLine("e2lite = " + e2lite);
            Console.WriteLine("hex_file = " + hex_file);
            Console.WriteLine("checksum = " + checksum);
            Console.WriteLine("*config path file enter \"path\"");
            if (debug) {
                string s = Console.ReadLine();
            }

            Stopwatch timeout_ = new Stopwatch();
            proc.StartInfo.WorkingDirectory = "";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = "denali_program_" + head + ".bat";
            bool flag_e2 = false;
            bool flag_checksum = true;
            string checksum_sup = "";
            string data = "";
            for (int i = 1; i <= 2; i++) {
                File.Delete("denali_program_" + head + ".log");
                while (true) {
                    try { proc.Start(); break; } catch { Thread.Sleep(50); }
                }
                Console.Write("wait log file");
                data = "";
                timeout_.Restart();
                while (timeout_.ElapsedMilliseconds < timeout_bat) {
                    try {
                        data = File.ReadAllText("denali_program_" + head + ".log");
                    } catch {
                        Console.Write(".");
                        Thread.Sleep(50);
                        continue;
                    }
                    timeout_.Stop();
                    break;
                }
                if (timeout_.IsRunning) { File.WriteAllText("test_head_" + head + "_result.txt", "timeout run batFile\r\nFAIL"); return; }
                bool flag_e2lite = true;
                if (!data.Contains("Operation completed")) {
                    flag_e2lite = false; Console.WriteLine("");
                    Console.WriteLine("not Operation completed");
                    if (i == 1) wait_discom();
                    continue;
                } else { Console.WriteLine(""); Console.WriteLine("Operation completed"); }
                if (!data.Contains(checksum)) {
                    flag_e2lite = false;
                    Console.WriteLine("checksum not equal");
                    if (!data.Contains("CRC-32 : ")) { Console.WriteLine("no \"CRC-32 : \""); Console.ReadLine(); }
                    string[] ss = data.Replace("CRC-32 : ", "$").Split('$');
                    string[] vv = ss[1].Split('\n');
                    checksum_sup = vv[0].Trim().Replace("\r", "").Replace("\n", "");
                    Console.WriteLine("checksum = " + checksum_sup);
                    flag_checksum = false;
                } else Console.WriteLine("checksum equal");
                Console.WriteLine(hex_file);
                if (debug == true) Console.ReadKey();
                if (flag_e2lite == true) { flag_e2 = true; break; }
            }
            File.Delete("denali_program_" + head + ".bat");
            if (flag_e2 == true) File.WriteAllText("test_head_" + head + "_result.txt", hex_file + "\r\nPASS");
            else {
                data = data.Replace("'", "").Replace(",", "").Replace("\n", "").Replace("\r", "");
                if (flag_checksum) File.WriteAllText("test_head_" + head + "_result.txt", data + "\r\nFAIL");
                else File.WriteAllText("test_head_" + head + "_result.txt", checksum_sup + "\r\nFAIL");
                File.WriteAllText("denali_program_disconnect_" + head + ".txt", "");
            }
            File.Delete("denali_program_running_" + head + ".txt");
        }

        private static bool flag_close = false;
        private static void TimerCallback(Object o) {
            if (!flag_close) { flag_close = true; return; }
            if (debug || flag_wait_discom) return;
            File.WriteAllText("test_head_" + head + "_result.txt", "timeout mainProgarm\r\nFAIL");
            File.Delete("denali_program_discom_" + head + ".txt");
            Environment.Exit(0);
        }
        private static bool flag_wait_discom = false;
        private static void wait_discom() {
            flag_wait_discom = true;
            File.WriteAllText("denali_program_discom_" + head + ".txt", "");
            Thread.Sleep(250);
            List<int> head_all_sup = new List<int>();
            for (int kj = 1; kj <= 4; kj++) {
                head_all_sup.Add(kj);
            }
            head_all_sup.Remove(Convert.ToInt32(head));
            int[] head_all = head_all_sup.ToArray();
            int[] running = { 0, 0, 0 };
            int[] disprogarm = { 0, 0, 0 };
            while (true) {
                for (int bbb = 0; bbb < 3; bbb++) {
                    try {
                        File.ReadAllText("denali_program_running_" + head_all[bbb] + ".txt");
                        running[bbb] = 1;
                    } catch { running[bbb] = 0; }
                    try {
                        File.ReadAllText("denali_program_discom_" + head_all[bbb] + ".txt");
                        disprogarm[bbb] = 1;
                    } catch { disprogarm[bbb] = 0; }
                }
                if (running[0] == 0 && running[1] == 0 && running[2] == 0) {
                    //get_name_e2lite();
                    discom("disable");
                    discom("enable");
                    break;
                } else {
                    bool xcv = false;
                    for (int ddf = 0; ddf < 3; ddf++) {
                        if (running[ddf] == 1 && disprogarm[ddf] == 0) xcv = true;
                    }
                    if (xcv) { Thread.Sleep(100); continue; }
                    bool min = false;
                    for (int cce = 0; cce < 3; cce++) {
                        if (disprogarm[cce] == 1)
                            if (Convert.ToInt32(head) > head_all[cce]) min = true;
                    }
                    if (min) {
                        while (true) {
                            try {
                                File.ReadAllText("progarm_discom_ok.txt");
                                break;
                            } catch { }
                            Thread.Sleep(50);
                        }
                    } else {
                        //get_name_e2lite();
                        discom("disable");
                        discom("enable");
                        File.WriteAllText("progarm_discom_ok.txt", "");
                        Thread.Sleep(2000);
                        File.Delete("progarm_discom_ok.txt");
                    }
                    break;
                }
            }
            File.Delete("denali_program_discom_" + head + ".txt");
            flag_wait_discom = false;
        }

        private static void get_name_e2lite() {
            ManagementObjectSearcher objOSDetails2 =
               new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity where DeviceID Like ""USB%""");
            ManagementObjectCollection osDetailsCollection2 = objOSDetails2.Get();
            foreach (ManagementObject usblist in osDetailsCollection2) {
                string arrport = usblist.GetPropertyValue("NAME").ToString();
                if (arrport.Contains("Renesas")) {
                    name_st = arrport;
                }
            }
        }
        private static string name_st = "Renesas E2 Lite";
        private static void discom(string cmd) {//enable disable//
            Process devManViewProc = new Process();
            devManViewProc.StartInfo.FileName = "DevManView.exe";
            devManViewProc.StartInfo.Arguments = "/" + cmd + " \"" + name_st + "\"";
            devManViewProc.Start();
            devManViewProc.WaitForExit();
            devManViewProc.StartInfo.Arguments = "/" + cmd + " \"" + "Renesas E-Series USB Driver" + "\"";
            devManViewProc.Start();
            devManViewProc.WaitForExit();
            //if(cmd == "enable") Thread.Sleep(5000);
        }
    }
}
