using System.Text.RegularExpressions;

namespace HFM.Core.Client;

public abstract class SlotDescription
{
    public abstract SlotType SlotType { get; }

    public string Value { get; set; }

    public static SlotDescription Parse(string value) =>
        GetSlotType(value) switch
        {
            SlotType.GPU => GPUSlotDescription.Parse(value),
            SlotType.CPU => CPUSlotDescription.Parse(value),
            SlotType.Unknown => null,
            _ => null
        };

    private static SlotType GetSlotType(string value) =>
        String.IsNullOrWhiteSpace(value)
            ? SlotType.Unknown
            : value.StartsWith("gpu", StringComparison.OrdinalIgnoreCase)
                ? SlotType.GPU
                : SlotType.CPU;
}

public class GPUSlotDescription : SlotDescription
{
    public override SlotType SlotType => SlotType.GPU;

    public int? GPUBus { get; set; }
    public int? GPUSlot { get; set; }
    public string GPUPrefix { get; set; }
    public string GPU { get; set; }

    public static new GPUSlotDescription Parse(string value)
    {
        var d = new GPUSlotDescription
        {
            Value = value
        };
        d.SetGPUBusAndSlot(value);
        d.SetGPUPrefix(value);
        d.SetGPU(value);
        return d;
    }

    private void SetGPUBusAndSlot(string description)
    {
        var match = Regex.Match(description, @"gpu\:(?<GPUBus>\d+)\:(?<GPUSlot>\d+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success &&
            Int32.TryParse(match.Groups["GPUBus"].Value, out var bus) &&
            Int32.TryParse(match.Groups["GPUSlot"].Value, out var slot))
        {
            GPUBus = bus;
            GPUSlot = slot;
        }
    }

    private void SetGPUPrefix(string description)
    {
        var match = Regex.Match(description, @"gpu\:\d+\:\d+ (?<GPUPrefix>.+) \[", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            GPUPrefix = match.Groups["GPUPrefix"].Value;
        }
    }

    private void SetGPU(string description)
    {
        var match = Regex.Match(description, "\\[(?<GPU>.+)\\]", RegexOptions.Singleline);
        if (match.Success)
        {
            GPU = match.Groups["GPU"].Value;
        }
    }
}

public class CPUSlotDescription : SlotDescription
{
    public override SlotType SlotType => SlotType.CPU;

    public int? CPUThreads { get; set; }

    public static new CPUSlotDescription Parse(string value)
    {
        var d = new CPUSlotDescription
        {
            Value = value
        };
        d.SetCPUThreads(value);
        return d;
    }

    private void SetCPUThreads(string description)
    {
        var match = Regex.Match(description, @"[cpu|smp]\:(?<CPUThreads>\d+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success && Int32.TryParse(match.Groups["CPUThreads"].Value, out var threads))
        {
            CPUThreads = threads;
        }
    }
}
