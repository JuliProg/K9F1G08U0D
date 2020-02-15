﻿using NAND_Prog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K9F1G08U0D
{
    // Ця dll-ка компаную всі складові чіпа K9F1G08U0D між собою (через DI інтерфейс ) і реалізує DI інтерфейс до NAND_Prog.exe



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

        //------------------------------------------------------



        //[Export("Memory organisation1", typeof(MemoryOrg))]
        //private MemoryOrg _memOrg;

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
        //задоволення імпорту [ImportMany("NAND_Prog.Chip", typeof(Operation))]   в NAND_Prog.exe     
        //---------------------------------------------------------------

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipReset", typeof(Operation))]
        private object obj1;

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipRead", typeof(Operation))]
        private object obj2;

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipProgramm", typeof(Operation))]
        private object obj3;

        [Export("NAND_Prog.Chip", typeof(List<Operation>))]
        [Import("ChipErase", typeof(Operation))]
        private object obj4;

        //-------------------------------------------------------------------
    }

    public class ChipSubPartImports
    {
        //задоволення імпорту [ImportMany("NAND_Prog.Sub part", typeof(ChipPart))]   в NAND_Prog.exe     
        //---------------------------------------------------------------

        [Export("NAND_Prog.Sub part", typeof(ChipPart))]
        [Import("ID Register", typeof(ChipPart))]
        private object obj1;

        [Export("NAND_Prog.Sub part", typeof(ChipPart))]
        [Export("NAND_Prog.activeSR", typeof(SRregister))]                        //користувач вказує що це є статус-регістр для перевірки результата операцій    
        [Import("Status Register", typeof(ChipPart))]
        private object obj2;

    }

    public class ID_RegisterImports
    {
        //задоволення імпорту [ImportMany("ID Operation", typeof(Operation))] для ID Register

        [Export("ID Operation", typeof(Operation))]
        [Import("ID Read", typeof(Operation))]
        private object obj1;


        //задоволення імпорту [Import("ID_Interpreted", typeof(Register), AllowDefault = true)]  для ID Register
        [Export("ID_Interpreted", typeof(Register))]
        [Import("K9F1G08U0D_IDInterpreted", typeof(Register))]
        private object obj2;
    }

    public class StatusRegisterImports
    {
        //задоволення імпорту [ImportMany("SR_operations", typeof(Operation))] для Status Register
        [Export("SR_operations", typeof(Operation))]
        [Import("SR_Read", typeof(Operation))]
        private object obj1;

        //задоволення імпорту [Import("SR_Interpreted", typeof(SRregister), AllowDefault = true)]  для Status Register
        [Export("SR_Interpreted", typeof(SRregister))]
        [Import("SRInterpreted", typeof(SRregister))]
        private object obj2;

    }

    #endregion
}
