# K9F1G08U0D
Implementation of the K9F1G08U0D chip for the JuliProg programmer

Dependency injection, DI based on MEF framework is used to connect the chip to the programmer.


# Chip parameters

```c#
 public ChipStructure()
        {   

            devManuf = "SAMSUNG";
            name = "K9F1G08U0D";

            width = Organization.x8;  //chip width - 8 bit
            bytesPP = 0x0800;         // page size - 2048 byte (2Kb)
            spareBytesPP = 0x40;      // size Spare Area - 64 byte
            pagesPB = 0x40;           // the number of pages per block - 64 
            bloksPLUN = 0x0400;       // number of blocks in CE - 1024
            LUNs = 0x01;              // the amount of CE in the chip
            colAdrCycles = 0x02;      // cycles for column addressing
            rowAdrCycles = 0x03;      // cycles for row addressing 

        }
  ```   

# Chip registers


