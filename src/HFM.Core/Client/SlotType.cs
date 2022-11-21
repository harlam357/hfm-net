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
    public static HashSet<string> CPUCoreNames { get; } = new(new[]
    {
        "0xa7",
        "0xa8",
        "0xa9",
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
    }, StringComparer.OrdinalIgnoreCase);

    public static HashSet<string> GPUCoreNames { get; } = new(new[]
    {
        "0x22",
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
    }, StringComparer.OrdinalIgnoreCase);

    public static SlotType FromCoreName(string coreName)
    {
        if (String.IsNullOrEmpty(coreName))
        {
            return SlotType.Unknown;
        }

        if (GPUCoreNames.Contains(coreName))
        {
            return SlotType.GPU;
        }

        if (CPUCoreNames.Contains(coreName))
        {
            return SlotType.CPU;
        }

        return SlotType.Unknown;
    }
}
