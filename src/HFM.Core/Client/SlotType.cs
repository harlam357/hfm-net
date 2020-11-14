using System;

namespace HFM.Core.Client
{
    /// <summary>
    /// Represents the FAHClient slot type.
    /// </summary>
    public enum SlotType
    {
        Unknown = 0,
        CPU = 1,
        GPU = 2
    }

    public static class ConvertToSlotType
    {
        public static SlotType FromCoreName(string coreName)
        {
            if (String.IsNullOrEmpty(coreName)) return SlotType.Unknown;

            switch (coreName.ToUpperInvariant())
            {
                case "GROMACS":
                case "DGROMACS":
                case "GBGROMACS":
                case "AMBER":
                case "GROMACS33":
                case "GROST":
                case "GROSIMT":
                case "DGROMACSB":
                case "DGROMACSC":
                case "GRO-A4":
                case "GRO_A4":
                case "PROTOMOL":
                case "GRO-SMP":
                case "GROCVS":
                case "GRO-A3":
                case "GRO_A3":
                case "GRO-A5":
                case "GRO_A5":
                case "GRO-A6":
                case "GRO_A7":
                case "GRO_A8":
                    return SlotType.CPU;
                case "GROGPU2":
                case "GROGPU2-MT":
                case "OPENMM_21":
                case "OPENMM_22":
                case "OPENMMGPU":
                case "OPENMM_OPENCL":
                case "ATI-DEV":
                case "NVIDIA-DEV":
                case "ZETA":
                case "ZETA_DEV":
                    return SlotType.GPU;
                default:
                    return SlotType.Unknown;
            }
        }

        public static SlotType FromCoreID(string coreID)
        {
            if (String.IsNullOrEmpty(coreID)) return SlotType.Unknown;

            switch (coreID.ToUpperInvariant())
            {
                case "78": // Gromacs
                case "79": // Double Gromacs
                case "7A": // GB Gromacs
                case "7B": // Double Gromacs B
                case "7C": // Double Gromacs C
                case "80": // Gromacs SREM
                case "81": // Gromacs SIMT
                case "82": // Amber
                case "A0": // Gromacs 33
                case "B4": // ProtoMol
                case "A1": // Gromacs SMP
                case "A2": // Gromacs SMP
                case "A3": // Gromacs SMP2
                case "A5": // Gromacs SMP2
                case "A6": // Gromacs SMP2
                case "A7": // Gromacs A7
                case "A8": // Gromacs A8
                    return SlotType.CPU;
                case "11": // GPU2 - GROGPU2
                case "12": // GPU2 - ATI-DEV
                case "13": // GPU2 - NVIDIA-DEV
                case "14": // GPU2 - GROGPU2-MT
                case "15": // GPU3 - OPENMMGPU - NVIDIA
                case "16": // GPU3 - OPENMMGPU - ATI
                case "17": // GPU3 - ZETA
                case "18": // GPU3 - ZETA_DEV
                case "21": // OPENMM_21
                case "22": // OPENMM_22
                    return SlotType.GPU;
                default:
                    return SlotType.Unknown;
            }
        }
    }
}
