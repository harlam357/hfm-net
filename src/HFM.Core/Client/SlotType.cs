namespace HFM.Core.Client;

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
    public static IEnumerable<string> CPUCoreNames { get; } = new[]
    {
        "GROMACS",
        "DGROMACS",
        "GBGROMACS",
        "AMBER",
        "GROMACS33",
        "GROST",
        "GROSIMT",
        "DGROMACSB",
        "DGROMACSC",
        "GRO-A4",
        "GRO_A4",
        "PROTOMOL",
        "GRO-SMP",
        "GROCVS",
        "GRO-A3",
        "GRO_A3",
        "GRO-A5",
        "GRO_A5",
        "GRO-A6",
        "GRO_A7",
        "GRO_A8"
    };

    public static IEnumerable<string> GPUCoreNames { get; } = new[]
    {
        "GROGPU2",
        "GROGPU2-MT",
        "OPENMM_21",
        "OPENMM_22",
        "OPENMMGPU",
        "OPENMM_OPENCL",
        "ATI-DEV",
        "NVIDIA-DEV",
        "ZETA",
        "ZETA_DEV"
    };

    public static SlotType FromCoreName(string coreName)
    {
        if (String.IsNullOrEmpty(coreName))
        {
            return SlotType.Unknown;
        }

        if (GPUCoreNames.Contains(coreName, StringComparer.OrdinalIgnoreCase))
        {
            return SlotType.GPU;
        }

        if (CPUCoreNames.Contains(coreName, StringComparer.OrdinalIgnoreCase))
        {
            return SlotType.CPU;
        }

        return SlotType.Unknown;
    }
}
