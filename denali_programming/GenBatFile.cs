using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denali_program {
    public class GenBatFile {

        public GenBatFile() {
            ExampleCreateBatE2Lite();
        }

        /// <summary>
        /// Example create bat file
        /// </summary>
        private void ExampleCreateBatE2Lite() {
            WriteBatFileE2Lite(new BatE2Lite {
                pathProject = "D:\\svn\\Firmware\\",
                nameProject = "programFW.rpj",
                pathHex = "D:\\svn\\Firmware\\",
                nameHex = "Firmware_v0000000031.hex",
                numberE2Lite = String.Empty
            });
        }

        /// <summary>
        /// Write bat file
        /// </summary>
        /// <param name="batE2Lite"></param>
        private void WriteBatFileE2Lite(BatE2Lite batE2Lite) {

            if (batE2Lite.numberE2Lite == String.Empty)
            {
                File.WriteAllText(batE2Lite.nameBat,
                              "\"C:\\Program Files (x86)\\Renesas Electronics\\programming Tools\\Renesas Flash Programmer " +
                              batE2Lite.versionRenesas + "\\RFPV3.exe\" " +
                              "/silent \"" + batE2Lite.pathProject + "\\" + batE2Lite.nameProject + "\" " +
                              "/file \"" + batE2Lite.pathHex + "\\" + batE2Lite.nameHex + "\" " +
                              "/log \"" + batE2Lite.nameLog + "\"");
            }
            else
            {
                File.WriteAllText(batE2Lite.nameBat,
                              "\"C:\\Program Files (x86)\\Renesas Electronics\\programming Tools\\Renesas Flash Programmer " +
                              batE2Lite.versionRenesas + "\\RFPV3.exe\" " +
                              "/silent \"" + batE2Lite.pathProject + "\\" + batE2Lite.nameProject + "\" " +
                              "/file \"" + batE2Lite.pathHex + "\\" + batE2Lite.nameHex + "\" " +
                              "/tool " + batE2Lite.numberE2Lite + " " +
                              "/log \"" + batE2Lite.nameLog + "\"");
            }
        }

        /// <summary>
        /// Object Gen Bat File
        /// </summary>
        public class BatE2Lite {

            public string nameBat = "programFW.bat";

            public string nameLog = "programFW.log";

            public string versionRenesas = "V3.09";

            public string pathProject = "D:\\svn\\Firmware\\";

            public string nameProject = "programFW.rpj";

            public string pathHex = "D:\\svn\\Firmware\\";

            public string nameHex = "Firmware_v0000000031.hex";

            public string numberE2Lite = "5ES009750";
        }
    }
}
