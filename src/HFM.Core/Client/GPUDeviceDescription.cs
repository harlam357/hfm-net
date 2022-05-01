using System.Text.RegularExpressions;

namespace HFM.Core.Client;

public class GPUDeviceDescription
{
    public int Platform { get; set; }
    public int Device { get; set; }
    public int Bus { get; set; }
    public int Slot { get; set; }
    public string Compute { get; set; }
    public string Driver { get; set; }

    public static GPUDeviceDescription Parse(string value)
    {
        var match = Regex.Match(value, @"Platform:(?<Platform>\d+) Device:(?<Device>\d+) Bus:(?<Bus>\d+) Slot:(?<Slot>\d+) Compute:(?<Compute>[\d\.]+) Driver:(?<Driver>[\d\.]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (match.Success &&
            Int32.TryParse(match.Groups["Platform"].Value, out var platform) &&
            Int32.TryParse(match.Groups["Device"].Value, out var device) &&
            Int32.TryParse(match.Groups["Bus"].Value, out var bus) &&
            Int32.TryParse(match.Groups["Slot"].Value, out var slot))
        {
            return new GPUDeviceDescription
            {
                Platform = platform,
                Device = device,
                Bus = bus,
                Slot = slot,
                Compute = match.Groups["Compute"].Value,
                Driver = match.Groups["Driver"].Value
            };
        }

        return null;
    }
}
