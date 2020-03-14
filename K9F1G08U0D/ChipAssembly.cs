using NAND_Prog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K9F1G08U0D
{
    // This dll compiles all the components of the K9F1G08U0D chip (via the DI interface) and implements the DI interface to NAND_Prog.exe



    #region Help
    /*  
    Інтерфейс , який торчить з NAND_Prog (він незмінний (утворюється при копміляції NAND_Prog.ехе)):
      обовязкові для задоволення імпорти :

       [Import("NAND_Prog.Organization", typeof(MemoryOrg))]         - організація памяті

       [Import("NAND_Prog.Bytes per page", typeof(MemoryOrg))]       - байт в сторінці

       [Import("NAND_Prog.Spare bytes per page", typeof(MemoryOrg))] - додаткових байт в сторінці

       [Import("NAND_Prog.Pages per block", typeof(MemoryOrg))]      - кількість сторінок в блоці

       [Import("NAND_Prog.Bloks per LUN", typeof(MemoryOrg))]        - кількість блоків в СЕ

       [Import("NAND_Prog.LUNs", typeof(MemoryOrg))]                 - кількість СЕ в чіпі

       [Import("NAND_Prog.Column address cycles", typeof(MemoryOrg))] - кількість циклів для адресації колонок

       [Import("NAND_Prog.Row address cycles", typeof(MemoryOrg))]    - кількість циклів для адресації рядків



       [Import("NAND_Prog.Device Manufacturing" , typeof(Chip))]      - виробник чіпа

       [Import("NAND_Prog.Chip name", typeof(Chip))]                  - імя чіпа


       <int>("NAND_Prog.Bad Block Mark")                              - признак НЕ bad blok


        
       [Import("ChipDependency", typeof(List<string>))]               - залежності чіпа (список dll-ок з яких складається чіп)

      не обовязкові імпорти :

        [ImportMany("NAND_Prog.Chip", typeof(Operation))]             - набір операцій для цього чіпа            

        [ImportMany("NAND_Prog.Sub part", typeof(ChipPart))]          - набір додаткових частин , які є в складі чіпа (наприклад ID Register , Status Register і інш.)

        [Import("NAND_Prog.activeSR", typeof(SRregister), AllowDefault = true)]  -  статус-регістр для оцінки статуса операцій Programm i Erase

        [Import("Interpreted" ,typeof(SRregister), AllowDefault = true)]         - інтерпретація вмісту статус-регістра

        [Import("NAND_Prog.Algo", typeof(AlgoMapBB), AllowDefault = true)]       - алгоритм обходу бед блоків


        ---------------------------------------------------------------------------------------------------------------------------------

        Через змінну private object objX відбувається задоволення всіх (обовязкових і не обовязкових) імпортів для NAND_Prog.exe .
        Міняючи контракт в секції [Import] для кожної змінної objX можна динамічно підключати ту чи іншу реалізацію відповідного Importa в NAND_Prog.exe
        Основне правило при цьому типи контрактів мають співпадати а імя контракту в секції [Import] береться з DLL з якої хочу щось імпортувати

     */
    #endregion

    public class ChipDependency
    {
        [Export("ChipDependency", typeof(List<string>))]
        private List<string> chip_dep;

        public ChipDependency()
        {
            chip_dep = new List<string>();

            chip_dep.Add("BadBlockImplement.dll");
        
            chip_dep.Add("ChipErase.dll");
            chip_dep.Add("ChipProgramm.dll");
            chip_dep.Add("ChipRead.dll");
            chip_dep.Add("ChipReset.dll");

            chip_dep.Add("ID_Interpreted.dll");
            chip_dep.Add("ID_Read.dll");
            chip_dep.Add("ID_Register.dll");

            chip_dep.Add("SR_Interpreted.dll");
            chip_dep.Add("SR_Read.dll");
            chip_dep.Add("StatusRegister.dll");

        }
    }


    #region Requared
    #region Don't edit !!!

    // DO NOT EDIT THIS CODE    !!!!
    public class ChipStructure
    {       

        [Export("NAND_Prog.Device Manufacturing", typeof(NAND_Prog.Chip))]
   //     [Import("ChipDescriptor.Device Manufacturing", typeof(NAND_Prog.Chip))]
        private object devManuf;


        [Export("NAND_Prog.Chip name", typeof(NAND_Prog.Chip))]
    //    [Import("ChipDescriptor.Chip name", typeof(NAND_Prog.Chip))]
        private object name;


      

        //-----------------------------------------------------

        [Export("MemoryOrg.Organization", typeof(MemoryOrg))]               
        public Organization width;

        [Export("MemoryOrg.Bytes per page", typeof(MemoryOrg))]
        public int bytesPP;

        [Export("MemoryOrg.Spare bytes per page", typeof(MemoryOrg))]
        public UInt16 spareBytesPP;

        [Export("MemoryOrg.Pages per block", typeof(MemoryOrg))]
        public UInt32 pagesPB;

        [Export("MemoryOrg.Bloks per LUN", typeof(MemoryOrg))]
        public UInt32 bloksPLUN;

        [Export("MemoryOrg.LUNs", typeof(MemoryOrg))]
        public byte LUNs;

        [Export("MemoryOrg.Column address cycles", typeof(MemoryOrg))]
        public byte colAdrCycles;

        [Export("MemoryOrg.Row address cycles", typeof(MemoryOrg))]
        public byte rowAdrCycles;
        
        #endregion


        public ChipStructure()
        {
         

            devManuf = "SAMSUNG";
            name = "K9F1G08U0D";

            width = Organization.x8;
            bytesPP = 0x0800;      // розмір сторінки - 2048 байт (2Kb)
            spareBytesPP = 0x40;   // розмір Spare Area - 64 байт
            pagesPB = 0x40;        // кількість сторінок в блоці - 64 
            bloksPLUN = 0x0400;    // кількість блоків в CE - 1024
            LUNs = 0x01;           // кількість CE в чіпі
            colAdrCycles = 0x02;   // адресація колонок 
            rowAdrCycles = 0x03;   // адресація рядків

            

        }
    }

    public class BadBolockImplement
    {
        //задоволення імпорту GetExportedValue<int>("BadBlockProvider.BadBlockMark")  в NAND_Prog.exe     
        //---------------------------------------------------------------
        [Export("BadBlockProvider.BadBlockMark", typeof(int))]
        [Import("SomeDll.BadBlockMark", typeof(int))]
        private object obj1;
    }


    #endregion


    #region Not requared
    public class ChipOperation
    {
       
        [Export("JuliProg.Chip", typeof(List<Operation>))]
        [Import("Reset_FFh", typeof(Operation))]
        private object reset_command;

        [Export("JuliProg.Chip", typeof(List<Operation>))]
        [Import("Read_00h_30h", typeof(Operation))]
        private object read_command;

        [Export("JuliProg.Chip", typeof(List<Operation>))]
        [Import("PageProgram_80h_10h", typeof(Operation))]
        private object page_program_command;

        [Export("JuliProg.Chip", typeof(List<Operation>))]
        [Import("Erase_60h_D0h", typeof(Operation))]
        private object erase_command;

    }


    
    public class ChipSubParts
    {
           
        //---------------------------------------------------------------

        [Export("Chip.Sub parts", typeof(ChipPart))]            // to JuliProg
        [Import("ID Register", typeof(ChipPart))]               // from [some].dll
        public object _id_register;

        //--------------------------------------------------

        [Export("Chip.Sub parts", typeof(ChipPart))]            // to JuliProg
        [Export("Chip.activeSR", typeof(SRregister))]           // to JuliProg            
        [Import("Status Register", typeof(ChipPart))]           // from [some].dll
        public object _sr_register;

        

    }

    
    public class Setup_ID_Register
    {

        #region Required

        [Export("ID Register name", typeof(string))]         //  to ID Register
        string ID_Register_name = "ID register";

        [Export("ID Register size", typeof(int))]            //  to ID Register
        int ID_Register_size = 0x05;


        #endregion

        #region Not required (Option)

        [Export("ID Register Operation", typeof(Operation))]      //  to ID Register
        [Import("ID Read", typeof(Operation))]                    // from [some].dll
        private object obj1;



        [Export("ID Register Interpreted", typeof(Func<Register, string>))]    //  to ID Register
        [Import("K9F1G08U0D_IDInterpreted", typeof(Func<Register, string>))]                 // from here
        private object obj2;

        #endregion
    }

    public class Setup_Status_Register
    {
        #region Required


        [Export("Status Register name", typeof(string))]           //  to Status Register
        string Status_Register_name = "Status Register";           

        [Export("Status Register size", typeof(int))]              //  to Status Register
        int Status_Register_size = 0x01;

        #endregion

        #region Not required (Option)
        
        
        [Export("Status Register Operations", typeof(Operation))]          // to Status Register
        [Import("Status Read", typeof(Operation))]                         // from [some].dll
        private object obj1;

        [Export("Status Register Interpreted", typeof(Func<Operation, SRregister, bool?>))]  // to Status Register
        [Import("SRInterpreted", typeof(Func<Operation, SRregister, bool?>))]                // from [some].dll
        private object obj2;

        #endregion


    }

    public class IDInterpreted
    {
        [Export("K9F1G08U0D_IDInterpreted", typeof(Func<Register, string>))]
        public string Interpreted(Register register)
        {
            string messsage = "1st Byte    Maker Code = " + BitConverter.ToString(register.GetContent(), 0, 1) + Environment.NewLine;
            messsage += "2st Byte    Device Code = " + BitConverter.ToString(register.GetContent(), 1, 1) + Environment.NewLine;
            messsage += "3rd ID Data = " + BitConverter.ToString(register.GetContent(), 2, 1) + Environment.NewLine;
            messsage += Encoding(register.GetContent()[2], 2) + Environment.NewLine;
            messsage += "4rd ID Data = " + BitConverter.ToString(register.GetContent(), 3, 1) + Environment.NewLine;
            messsage += Encoding(register.GetContent()[3], 3) + Environment.NewLine;
            messsage += "5rd ID Data = " + BitConverter.ToString(register.GetContent(), 4, 1) + Environment.NewLine;
            messsage += Encoding(register.GetContent()[4], 4) + Environment.NewLine;

            return messsage;
        }
        private string Encoding(byte bt, int pos)
        {
            string str_result = String.Empty;

            var IO = new System.Collections.BitArray(new[] { bt });

            switch (pos)
            {
                case 2:
                    str_result += " Internal Chip Number = ";
                    if (IO[1] == false && IO[0] == false)
                        str_result += "1";
                    if (IO[1] == false && IO[0] == true)
                        str_result += "2";
                    if (IO[1] == true && IO[0] == false)
                        str_result += "4";
                    if (IO[1] == true && IO[0] == true)
                        str_result += "8";
                    str_result += Environment.NewLine;


                    str_result += " Cell Type = ";
                    if (IO[3] == false && IO[2] == false)
                        str_result += "2 Level Cell";
                    if (IO[3] == false && IO[2] == true)
                        str_result += "4 Level Cell";
                    if (IO[3] == true && IO[2] == false)
                        str_result += "8 Level Cell";
                    if (IO[3] == true && IO[2] == true)
                        str_result += "16 Level Cell";
                    str_result += Environment.NewLine;


                    str_result += " Number of Simultaneously Programmed Pages = ";
                    if (IO[5] == false && IO[4] == false)
                        str_result += "1";
                    if (IO[5] == false && IO[4] == true)
                        str_result += "2";
                    if (IO[5] == true && IO[4] == false)
                        str_result += "4";
                    if (IO[5] == true && IO[4] == true)
                        str_result += "8";
                    str_result += Environment.NewLine;


                    str_result += " Interleave Program Between multiple chips = ";
                    if (IO[6] == false)
                        str_result += "Not Support";
                    if (IO[6] == true)
                        str_result += "Support";
                    str_result += Environment.NewLine;

                    str_result += " Cache Program = ";
                    if (IO[7] == false)
                        str_result += "Not Support";
                    if (IO[7] == true)
                        str_result += "Support";
                    str_result += Environment.NewLine;
                    break;

                case 3:

                    str_result += " Page Size (w/o redundant area ) = ";
                    if (IO[1] == false && IO[0] == false)
                        str_result += "1KB";
                    if (IO[1] == false && IO[0] == true)
                        str_result += "2KB";
                    if (IO[1] == true && IO[0] == false)
                        str_result += "4KB";
                    if (IO[1] == true && IO[0] == true)
                        str_result += "8KB";
                    str_result += Environment.NewLine;


                    str_result += " Block Size (w/o redundant area ) = ";
                    if (IO[5] == false && IO[4] == false)
                        str_result += "64KB";
                    if (IO[5] == false && IO[4] == true)
                        str_result += "128KB";
                    if (IO[5] == true && IO[4] == false)
                        str_result += "256KB";
                    if (IO[5] == true && IO[4] == true)
                        str_result += "512KB";
                    str_result += Environment.NewLine;


                    str_result += " Redundant Area Size ( byte/512byte) = ";
                    if (IO[2] == false)
                        str_result += "8";
                    if (IO[2] == true)
                        str_result += "16";
                    str_result += Environment.NewLine;


                    str_result += " Organization = ";
                    if (IO[6] == false)
                        str_result += "x8";
                    if (IO[6] == true)
                        str_result += "x16";
                    str_result += Environment.NewLine;

                    str_result += " Serial Access Minimum = ";
                    if (IO[7] == false && IO[3] == false)
                        str_result += "50ns/30ns";
                    if (IO[7] == true && IO[3] == false)
                        str_result += "25ns";
                    if (IO[7] == false && IO[3] == true)
                        str_result += "Reserved";
                    if (IO[7] == true && IO[3] == true)
                        str_result += "Reserved";
                    str_result += Environment.NewLine;
                    break;

                case 4:

                    str_result += " Plane Number = ";
                    if (IO[3] == false && IO[2] == false)
                        str_result += "1";
                    if (IO[3] == false && IO[2] == true)
                        str_result += "2";
                    if (IO[3] == true && IO[2] == false)
                        str_result += "4";
                    if (IO[3] == true && IO[2] == true)
                        str_result += "8";
                    str_result += Environment.NewLine;


                    str_result += " Plane Size (w/o redundant area ) = ";
                    if (IO[6] == false && IO[5] == false && IO[4] == false)
                        str_result += "64Mb";
                    if (IO[6] == false && IO[5] == false && IO[4] == true)
                        str_result += "128Mb";
                    if (IO[6] == false && IO[5] == true && IO[4] == false)
                        str_result += "256Mb";
                    if (IO[6] == false && IO[5] == true && IO[4] == true)
                        str_result += "512Mb";
                    if (IO[6] == true && IO[5] == false && IO[4] == false)
                        str_result += "1Gb";
                    if (IO[6] == true && IO[5] == false && IO[4] == true)
                        str_result += "2Gb";
                    if (IO[6] == true && IO[5] == true && IO[4] == false)
                        str_result += "4Gb";
                    if (IO[6] == true && IO[5] == true && IO[4] == true)
                        str_result += "8Gb";
                    str_result += Environment.NewLine;


                    break;
            }
            return str_result;
        }
    }



    #endregion
}
