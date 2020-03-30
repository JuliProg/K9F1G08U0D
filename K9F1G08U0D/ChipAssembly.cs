using NAND_Prog;
using System;
using System.ComponentModel.Composition;

namespace K9F1G08U0D
{



    public class ChipAssembly
    {
        [Export("Chip")]
        ChipPrototype myChip = new ChipPrototype();


        ChipAssembly()
        {
            myChip.devManuf = "SAMSUNG";
            myChip.name = "K9F1G08U0D";

            myChip.width = Organization.x8;
            myChip.bytesPP = 2048;      // ����� ������� - 2048 ���� (2Kb)
            myChip.spareBytesPP = 64;   // ����� Spare Area - 64 ����
            myChip.pagesPB = 64;        // ������� ������� � ����� - 64 
            myChip.bloksPLUN = 1024;    // ������� ����� � CE - 1024
            myChip.LUNs = 1;           // ������� CE � ���
            myChip.colAdrCycles = 2;   // ��������� ������� 
            myChip.rowAdrCycles = 2;   // ��������� �����

            //------- Add chip operations ----------------------------------------------------

            myChip.Operations("Reset_FFh").
                   Operations("Erase_60h_D0h").
                   Operations("Read_00h_30h").
                   Operations("PageProgram_80h_10h");

            //------- Add chip registers ----------------------------------------------------

            myChip.registers.Add(
                "Status Register").
                Size(1).
                Operations("ReadStatus_70h").
                Interpretation("StatusInterpreting@v1");   //From ChipPart\[some].dll



            myChip.registers.Add(
                "Id Register").
                Size(5).
                Operations("ReadId_90h").               
                Interpretation(ID_interpreting);          // From here
                                            


        }

        public string ID_interpreting(Register register)
        {
            byte[] content = register.GetContent();


            //BitConverter.ToString(register.GetContent(), 0, 1)
            //BitConverter.ToString(register.GetContent(), 1, 1)
            string messsage = "1st Byte    Maker Code = " + content[0].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[0],0) + Environment.NewLine;

            messsage += "2nd Byte    Device Code = " + content[1].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[1], 1) + Environment.NewLine;

            messsage += "3rd ID Data = " + content[2].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[2], 2) + Environment.NewLine;

            messsage += "4th ID Data = " + content[3].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[3], 3) + Environment.NewLine;

            messsage += "5th ID Data = " + content[4].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[4], 4) + Environment.NewLine;

            return messsage;
        }
        private string ID_decoding(byte bt, int pos)
        {
            string str_result = String.Empty;

            var IO = new System.Collections.BitArray(new[] { bt });

            switch (pos)
            {
                case 0:
                    str_result += "Maker ";
                    if (bt == 0xEC)
                        str_result += "is Samsung";
                    else
                        str_result += "is not Samsung";
                    str_result += Environment.NewLine;
                    break;

                case 1:
                    str_result += "Device ";
                    if (bt == 0xF1)
                        str_result += "is K9F1G08U0D";
                    else
                        str_result += "is not K9F1G08U0D";
                    str_result += Environment.NewLine;
                    break;

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


    //public class ID_capability
    //{

    //    [Export(typeof(Func<Register, string>)),
    //           ExportMetadata("Name", "K9F1G08U0D.ID_Register.Interpreted")]

    //    public string Interpreting(Register register)
    //    {
    //        string messsage = "1st Byte    Maker Code = " + BitConverter.ToString(register.GetContent(), 0, 1) + Environment.NewLine;
    //        messsage += "2st Byte    Device Code = " + BitConverter.ToString(register.GetContent(), 1, 1) + Environment.NewLine;
    //        messsage += "3rd ID Data = " + BitConverter.ToString(register.GetContent(), 2, 1) + Environment.NewLine;
    //        messsage += Decoding(register.GetContent()[2], 2) + Environment.NewLine;
    //        messsage += "4rd ID Data = " + BitConverter.ToString(register.GetContent(), 3, 1) + Environment.NewLine;
    //        messsage += Decoding(register.GetContent()[3], 3) + Environment.NewLine;
    //        messsage += "5rd ID Data = " + BitConverter.ToString(register.GetContent(), 4, 1) + Environment.NewLine;
    //        messsage += Decoding(register.GetContent()[4], 4) + Environment.NewLine;

    //        return messsage;
    //    }
    //    private string Decoding(byte bt, int pos)
    //    {
    //        string str_result = String.Empty;

    //        var IO = new System.Collections.BitArray(new[] { bt });

    //        switch (pos)
    //        {
    //            case 2:
    //                str_result += " Internal Chip Number = ";
    //                if (IO[1] == false && IO[0] == false)
    //                    str_result += "1";
    //                if (IO[1] == false && IO[0] == true)
    //                    str_result += "2";
    //                if (IO[1] == true && IO[0] == false)
    //                    str_result += "4";
    //                if (IO[1] == true && IO[0] == true)
    //                    str_result += "8";
    //                str_result += Environment.NewLine;


    //                str_result += " Cell Type = ";
    //                if (IO[3] == false && IO[2] == false)
    //                    str_result += "2 Level Cell";
    //                if (IO[3] == false && IO[2] == true)
    //                    str_result += "4 Level Cell";
    //                if (IO[3] == true && IO[2] == false)
    //                    str_result += "8 Level Cell";
    //                if (IO[3] == true && IO[2] == true)
    //                    str_result += "16 Level Cell";
    //                str_result += Environment.NewLine;


    //                str_result += " Number of Simultaneously Programmed Pages = ";
    //                if (IO[5] == false && IO[4] == false)
    //                    str_result += "1";
    //                if (IO[5] == false && IO[4] == true)
    //                    str_result += "2";
    //                if (IO[5] == true && IO[4] == false)
    //                    str_result += "4";
    //                if (IO[5] == true && IO[4] == true)
    //                    str_result += "8";
    //                str_result += Environment.NewLine;


    //                str_result += " Interleave Program Between multiple chips = ";
    //                if (IO[6] == false)
    //                    str_result += "Not Support";
    //                if (IO[6] == true)
    //                    str_result += "Support";
    //                str_result += Environment.NewLine;

    //                str_result += " Cache Program = ";
    //                if (IO[7] == false)
    //                    str_result += "Not Support";
    //                if (IO[7] == true)
    //                    str_result += "Support";
    //                str_result += Environment.NewLine;
    //                break;

    //            case 3:

    //                str_result += " Page Size (w/o redundant area ) = ";
    //                if (IO[1] == false && IO[0] == false)
    //                    str_result += "1KB";
    //                if (IO[1] == false && IO[0] == true)
    //                    str_result += "2KB";
    //                if (IO[1] == true && IO[0] == false)
    //                    str_result += "4KB";
    //                if (IO[1] == true && IO[0] == true)
    //                    str_result += "8KB";
    //                str_result += Environment.NewLine;


    //                str_result += " Block Size (w/o redundant area ) = ";
    //                if (IO[5] == false && IO[4] == false)
    //                    str_result += "64KB";
    //                if (IO[5] == false && IO[4] == true)
    //                    str_result += "128KB";
    //                if (IO[5] == true && IO[4] == false)
    //                    str_result += "256KB";
    //                if (IO[5] == true && IO[4] == true)
    //                    str_result += "512KB";
    //                str_result += Environment.NewLine;


    //                str_result += " Redundant Area Size ( byte/512byte) = ";
    //                if (IO[2] == false)
    //                    str_result += "8";
    //                if (IO[2] == true)
    //                    str_result += "16";
    //                str_result += Environment.NewLine;


    //                str_result += " Organization = ";
    //                if (IO[6] == false)
    //                    str_result += "x8";
    //                if (IO[6] == true)
    //                    str_result += "x16";
    //                str_result += Environment.NewLine;

    //                str_result += " Serial Access Minimum = ";
    //                if (IO[7] == false && IO[3] == false)
    //                    str_result += "50ns/30ns";
    //                if (IO[7] == true && IO[3] == false)
    //                    str_result += "25ns";
    //                if (IO[7] == false && IO[3] == true)
    //                    str_result += "Reserved";
    //                if (IO[7] == true && IO[3] == true)
    //                    str_result += "Reserved";
    //                str_result += Environment.NewLine;
    //                break;

    //            case 4:

    //                str_result += " Plane Number = ";
    //                if (IO[3] == false && IO[2] == false)
    //                    str_result += "1";
    //                if (IO[3] == false && IO[2] == true)
    //                    str_result += "2";
    //                if (IO[3] == true && IO[2] == false)
    //                    str_result += "4";
    //                if (IO[3] == true && IO[2] == true)
    //                    str_result += "8";
    //                str_result += Environment.NewLine;


    //                str_result += " Plane Size (w/o redundant area ) = ";
    //                if (IO[6] == false && IO[5] == false && IO[4] == false)
    //                    str_result += "64Mb";
    //                if (IO[6] == false && IO[5] == false && IO[4] == true)
    //                    str_result += "128Mb";
    //                if (IO[6] == false && IO[5] == true && IO[4] == false)
    //                    str_result += "256Mb";
    //                if (IO[6] == false && IO[5] == true && IO[4] == true)
    //                    str_result += "512Mb";
    //                if (IO[6] == true && IO[5] == false && IO[4] == false)
    //                    str_result += "1Gb";
    //                if (IO[6] == true && IO[5] == false && IO[4] == true)
    //                    str_result += "2Gb";
    //                if (IO[6] == true && IO[5] == true && IO[4] == false)
    //                    str_result += "4Gb";
    //                if (IO[6] == true && IO[5] == true && IO[4] == true)
    //                    str_result += "8Gb";
    //                str_result += Environment.NewLine;


    //                break;
    //        }
    //        return str_result;
    //    }
    //}


    #region
    #endregion





    ////  This dll compiles all the components of the K9F1G08U0D chip(via the DI interface) and implements the DI interface to NAND_Prog.exe



    //  #region Help
    //   /*  
    //   ��������� , ���� ������� � NAND_Prog (�� �������� (����������� ��� ��������� NAND_Prog.���)):
    //     ��������� ��� ����������� ������� :

    //      [Import("NAND_Prog.Organization", typeof(MemoryOrg))]         - ���������� �����

    //      [Import("NAND_Prog.Bytes per page", typeof(MemoryOrg))]       - ���� � �������

    //      [Import("NAND_Prog.Spare bytes per page", typeof(MemoryOrg))] - ���������� ���� � �������

    //      [Import("NAND_Prog.Pages per block", typeof(MemoryOrg))]      - ������� ������� � �����

    //      [Import("NAND_Prog.Bloks per LUN", typeof(MemoryOrg))]        - ������� ����� � ��

    //      [Import("NAND_Prog.LUNs", typeof(MemoryOrg))]                 - ������� �� � ���

    //      [Import("NAND_Prog.Column address cycles", typeof(MemoryOrg))] - ������� ����� ��� ��������� �������

    //      [Import("NAND_Prog.Row address cycles", typeof(MemoryOrg))]    - ������� ����� ��� ��������� �����



    //      [Import("NAND_Prog.Device Manufacturing" , typeof(Chip))]      - �������� ����

    //      [Import("NAND_Prog.Chip name", typeof(Chip))]                  - ��� ����


    //      <int>("NAND_Prog.Bad Block Mark")                              - ������� �� bad blok



    //      [Import("ChipDependency", typeof(List<string>))]               - ��������� ���� (������ dll-�� � ���� ���������� ���)

    //     �� ��������� ������� :

    //       [ImportMany("NAND_Prog.Chip", typeof(Operation))]             - ���� �������� ��� ����� ����            

    //       [ImportMany("NAND_Prog.Sub part", typeof(ChipPart))]          - ���� ���������� ������ , �� � � ����� ���� (��������� ID Register , Status Register � ���.)

    //       [Import("NAND_Prog.activeSR", typeof(SRregister), AllowDefault = true)]  -  ������-������ ��� ������ ������� �������� Programm i Erase

    //       [Import("Interpreted" ,typeof(SRregister), AllowDefault = true)]         - ������������� ����� ������-�������

    //       [Import("NAND_Prog.Algo", typeof(AlgoMapBB), AllowDefault = true)]       - �������� ������ ��� �����


    //       ---------------------------------------------------------------------------------------------------------------------------------

    //       ����� ����� private object objX ���������� ����������� ��� (����������� � �� �����������) ������� ��� NAND_Prog.exe .
    //       ̳����� �������� � ������ [Import] ��� ����� ����� objX ����� �������� ��������� �� �� ���� ��������� ���������� Importa � NAND_Prog.exe
    //       ������� ������� ��� ����� ���� ��������� ����� ��������� � ��� ��������� � ������ [Import] �������� � DLL � ��� ���� ���� �����������

    //    */
    //  #endregion

    //  // public class ChipDependency
    //  //{
    //  //    [Export("ChipDependency", typeof(List<string>))]
    //  //    private List<string> chip_dep;

    //  //    public ChipDependency()
    //  //    {
    //  //        chip_dep = new List<string>();

    //  //        chip_dep.Add("BadBlockImplement.dll");

    //  //        chip_dep.Add("ChipErase.dll");
    //  //        chip_dep.Add("ChipProgramm.dll");
    //  //        chip_dep.Add("ChipRead.dll");
    //  //        chip_dep.Add("ChipReset.dll");

    //  //        chip_dep.Add("ID_Interpreted.dll");
    //  //        chip_dep.Add("ID_Read.dll");
    //  //        chip_dep.Add("ID_Register.dll");

    //  //        chip_dep.Add("SR_Interpreted.dll");
    //  //        chip_dep.Add("SR_Read.dll");
    //  //        chip_dep.Add("StatusRegister.dll");

    //  //    }
    //  //}


    //  //#region Requared
    //  //#region Don't edit !!!

    //  //// DO NOT EDIT THIS CODE    !!!!
    //  //public class ChipStructure
    //  //{

    //  //    [Export("NAND_Prog.Device Manufacturing", typeof(NAND_Prog.Chip))]
    //  //    //     [Import("ChipDescriptor.Device Manufacturing", typeof(NAND_Prog.Chip))]
    //  //    private object devManuf;


    //  //    [Export("NAND_Prog.Chip name", typeof(NAND_Prog.Chip))]
    //  //    //    [Import("ChipDescriptor.Chip name", typeof(NAND_Prog.Chip))]
    //  //    private object name;




    //  //    //-----------------------------------------------------

    //  //    [Export("MemoryOrg.Organization", typeof(MemoryOrg))]
    //  //    public Organization width;

    //  //    [Export("MemoryOrg.Bytes per page", typeof(MemoryOrg))]
    //  //    public int bytesPP;

    //  //    [Export("MemoryOrg.Spare bytes per page", typeof(MemoryOrg))]
    //  //    public UInt16 spareBytesPP;

    //  //    [Export("MemoryOrg.Pages per block", typeof(MemoryOrg))]
    //  //    public UInt32 pagesPB;

    //  //    [Export("MemoryOrg.Bloks per LUN", typeof(MemoryOrg))]
    //  //    public UInt32 bloksPLUN;

    //  //    [Export("MemoryOrg.LUNs", typeof(MemoryOrg))]
    //  //    public byte LUNs;

    //  //    [Export("MemoryOrg.Column address cycles", typeof(MemoryOrg))]
    //  //    public byte colAdrCycles;

    //  //    [Export("MemoryOrg.Row address cycles", typeof(MemoryOrg))]
    //  //    public byte rowAdrCycles;

    //  //    #endregion


    //  //    public ChipStructure()
    //  //    {


    //  //        devManuf = "SAMSUNG";
    //  //        name = "K9F1G08U0D";

    //  //        width = Organization.x8;
    //  //        bytesPP = 0x0800;      // ����� ������� - 2048 ���� (2Kb)
    //  //        spareBytesPP = 0x40;   // ����� Spare Area - 64 ����
    //  //        pagesPB = 0x40;        // ������� ������� � ����� - 64 
    //  //        bloksPLUN = 0x0400;    // ������� ����� � CE - 1024
    //  //        LUNs = 0x01;           // ������� CE � ���
    //  //        colAdrCycles = 0x02;   // ��������� ������� 
    //  //        rowAdrCycles = 0x03;   // ��������� �����



    //  //    }
    //  //}

    //  //public class BadBolockImplement
    //  //{
    //  //    //����������� ������� GetExportedValue<int>("BadBlockProvider.BadBlockMark")  � NAND_Prog.exe     
    //  //    //---------------------------------------------------------------
    //  //    [Export("BadBlockProvider.BadBlockMark", typeof(int))]
    //  //    [Import("SomeDll.BadBlockMark", typeof(int))]
    //  //    private object obj1;
    //  //}


    //  //#endregion


    //  //#region Not requared
    //  ////public class ChipOperation
    //  ////{

    //  ////    [Export("JuliProg.Chip", typeof(List<Operation>))]
    //  ////    [Import("Reset_FFh", typeof(Operation))]
    //  ////    private object reset_command;

    //  ////    [Export("JuliProg.Chip", typeof(List<Operation>))]
    //  ////    [Import("Read_00h_30h", typeof(Operation))]
    //  ////    private object read_command;

    //  ////    [Export("JuliProg.Chip", typeof(List<Operation>))]
    //  ////    [Import("PageProgram_80h_10h", typeof(Operation))]
    //  ////    private object page_program_command;

    //  ////    [Export("JuliProg.Chip", typeof(List<Operation>))]
    //  ////    [Import("Erase_60h_D0h", typeof(Operation))]
    //  ////    private object erase_command;

    //  ////}



    //  //public class ChipSubParts
    //  //{

    //  //    //---------------------------------------------------------------

    //  //    [Export("Chip.Sub parts", typeof(ChipPart))]            // to JuliProg
    //  //    [Import("ID Register", typeof(ChipPart))]               // from [some].dll
    //  //    public object _id_register;

    //  //    //--------------------------------------------------

    //  //    [Export("Chip.Sub parts", typeof(ChipPart))]            // to JuliProg
    //  //    [Export("Chip.activeSR", typeof(SRregister))]           // to JuliProg            
    //  //    [Import("Status Register", typeof(ChipPart))]           // from [some].dll
    //  //    public object _sr_register;



    //  //}


    //  //public class Setup_ID_Register
    //  //{

    //  //    #region Required

    //  //    [Export("ID Register name", typeof(string))]         //  to ID Register
    //  //    string ID_Register_name = "ID register";

    //  //    [Export("ID Register size", typeof(int))]            //  to ID Register
    //  //    int ID_Register_size = 0x05;


    //  //    #endregion

    //  //    #region Not required (Option)

    //  //    [Export("ID Register Operation", typeof(Operation))]      //  to ID Register
    //  //    [Import("ID Read", typeof(Operation))]                    // from [some].dll
    //  //    private object obj1;



    //  //    [Export("ID Register Interpreted", typeof(Func<Register, string>))]    //  to ID Register
    //  //    [Import("K9F1G08U0D_IDInterpreted", typeof(Func<Register, string>))]                 // from here
    //  //    private object obj2;

    //  //    #endregion
    //  //}

    //  //public class Setup_Status_Register
    //  //{
    //  //    #region Required


    //  //    [Export("Status Register name", typeof(string))]           //  to Status Register
    //  //    string Status_Register_name = "Status Register";

    //  //    [Export("Status Register size", typeof(int))]              //  to Status Register
    //  //    int Status_Register_size = 0x01;

    //  //    #endregion

    //  //    #region Not required (Option)


    //  //    [Export("Status Register Operations", typeof(Operation))]          // to Status Register
    //  //    [Import("Status Read", typeof(Operation))]                         // from [some].dll
    //  //    private object obj1;

    //  //    [Export("Status Register Interpreted", typeof(Func<Operation, SRregister, bool?>))]  // to Status Register
    //  //    [Import("SRInterpreted", typeof(Func<Operation, SRregister, bool?>))]                // from [some].dll
    //  //    private object obj2;

    //  //    #endregion


    //  //}

    //  //public class IDInterpreted
    //  //{
    //  //    [Export("K9F1G08U0D_IDInterpreted", typeof(Func<Register, string>))]
    //  //    public string Interpreted(Register register)
    //  //    {
    //  //        string messsage = "1st Byte    Maker Code = " + BitConverter.ToString(register.GetContent(), 0, 1) + Environment.NewLine;
    //  //        messsage += "2st Byte    Device Code = " + BitConverter.ToString(register.GetContent(), 1, 1) + Environment.NewLine;
    //  //        messsage += "3rd ID Data = " + BitConverter.ToString(register.GetContent(), 2, 1) + Environment.NewLine;
    //  //        messsage += Encoding(register.GetContent()[2], 2) + Environment.NewLine;
    //  //        messsage += "4rd ID Data = " + BitConverter.ToString(register.GetContent(), 3, 1) + Environment.NewLine;
    //  //        messsage += Encoding(register.GetContent()[3], 3) + Environment.NewLine;
    //  //        messsage += "5rd ID Data = " + BitConverter.ToString(register.GetContent(), 4, 1) + Environment.NewLine;
    //  //        messsage += Encoding(register.GetContent()[4], 4) + Environment.NewLine;

    //  //        return messsage;
    //  //    }
    //  //    private string Encoding(byte bt, int pos)
    //  //    {
    //  //        string str_result = String.Empty;

    //  //        var IO = new System.Collections.BitArray(new[] { bt });

    //  //        switch (pos)
    //  //        {
    //  //            case 2:
    //  //                str_result += " Internal Chip Number = ";
    //  //                if (IO[1] == false && IO[0] == false)
    //  //                    str_result += "1";
    //  //                if (IO[1] == false && IO[0] == true)
    //  //                    str_result += "2";
    //  //                if (IO[1] == true && IO[0] == false)
    //  //                    str_result += "4";
    //  //                if (IO[1] == true && IO[0] == true)
    //  //                    str_result += "8";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Cell Type = ";
    //  //                if (IO[3] == false && IO[2] == false)
    //  //                    str_result += "2 Level Cell";
    //  //                if (IO[3] == false && IO[2] == true)
    //  //                    str_result += "4 Level Cell";
    //  //                if (IO[3] == true && IO[2] == false)
    //  //                    str_result += "8 Level Cell";
    //  //                if (IO[3] == true && IO[2] == true)
    //  //                    str_result += "16 Level Cell";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Number of Simultaneously Programmed Pages = ";
    //  //                if (IO[5] == false && IO[4] == false)
    //  //                    str_result += "1";
    //  //                if (IO[5] == false && IO[4] == true)
    //  //                    str_result += "2";
    //  //                if (IO[5] == true && IO[4] == false)
    //  //                    str_result += "4";
    //  //                if (IO[5] == true && IO[4] == true)
    //  //                    str_result += "8";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Interleave Program Between multiple chips = ";
    //  //                if (IO[6] == false)
    //  //                    str_result += "Not Support";
    //  //                if (IO[6] == true)
    //  //                    str_result += "Support";
    //  //                str_result += Environment.NewLine;

    //  //                str_result += " Cache Program = ";
    //  //                if (IO[7] == false)
    //  //                    str_result += "Not Support";
    //  //                if (IO[7] == true)
    //  //                    str_result += "Support";
    //  //                str_result += Environment.NewLine;
    //  //                break;

    //  //            case 3:

    //  //                str_result += " Page Size (w/o redundant area ) = ";
    //  //                if (IO[1] == false && IO[0] == false)
    //  //                    str_result += "1KB";
    //  //                if (IO[1] == false && IO[0] == true)
    //  //                    str_result += "2KB";
    //  //                if (IO[1] == true && IO[0] == false)
    //  //                    str_result += "4KB";
    //  //                if (IO[1] == true && IO[0] == true)
    //  //                    str_result += "8KB";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Block Size (w/o redundant area ) = ";
    //  //                if (IO[5] == false && IO[4] == false)
    //  //                    str_result += "64KB";
    //  //                if (IO[5] == false && IO[4] == true)
    //  //                    str_result += "128KB";
    //  //                if (IO[5] == true && IO[4] == false)
    //  //                    str_result += "256KB";
    //  //                if (IO[5] == true && IO[4] == true)
    //  //                    str_result += "512KB";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Redundant Area Size ( byte/512byte) = ";
    //  //                if (IO[2] == false)
    //  //                    str_result += "8";
    //  //                if (IO[2] == true)
    //  //                    str_result += "16";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Organization = ";
    //  //                if (IO[6] == false)
    //  //                    str_result += "x8";
    //  //                if (IO[6] == true)
    //  //                    str_result += "x16";
    //  //                str_result += Environment.NewLine;

    //  //                str_result += " Serial Access Minimum = ";
    //  //                if (IO[7] == false && IO[3] == false)
    //  //                    str_result += "50ns/30ns";
    //  //                if (IO[7] == true && IO[3] == false)
    //  //                    str_result += "25ns";
    //  //                if (IO[7] == false && IO[3] == true)
    //  //                    str_result += "Reserved";
    //  //                if (IO[7] == true && IO[3] == true)
    //  //                    str_result += "Reserved";
    //  //                str_result += Environment.NewLine;
    //  //                break;

    //  //            case 4:

    //  //                str_result += " Plane Number = ";
    //  //                if (IO[3] == false && IO[2] == false)
    //  //                    str_result += "1";
    //  //                if (IO[3] == false && IO[2] == true)
    //  //                    str_result += "2";
    //  //                if (IO[3] == true && IO[2] == false)
    //  //                    str_result += "4";
    //  //                if (IO[3] == true && IO[2] == true)
    //  //                    str_result += "8";
    //  //                str_result += Environment.NewLine;


    //  //                str_result += " Plane Size (w/o redundant area ) = ";
    //  //                if (IO[6] == false && IO[5] == false && IO[4] == false)
    //  //                    str_result += "64Mb";
    //  //                if (IO[6] == false && IO[5] == false && IO[4] == true)
    //  //                    str_result += "128Mb";
    //  //                if (IO[6] == false && IO[5] == true && IO[4] == false)
    //  //                    str_result += "256Mb";
    //  //                if (IO[6] == false && IO[5] == true && IO[4] == true)
    //  //                    str_result += "512Mb";
    //  //                if (IO[6] == true && IO[5] == false && IO[4] == false)
    //  //                    str_result += "1Gb";
    //  //                if (IO[6] == true && IO[5] == false && IO[4] == true)
    //  //                    str_result += "2Gb";
    //  //                if (IO[6] == true && IO[5] == true && IO[4] == false)
    //  //                    str_result += "4Gb";
    //  //                if (IO[6] == true && IO[5] == true && IO[4] == true)
    //  //                    str_result += "8Gb";
    //  //                str_result += Environment.NewLine;


    //  //                break;
    //  //        }
    //  //        return str_result;
    //  //    }
    //  //}



    //  //#endregion
}
