using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class GPUDeviceDescriptionTests
{
    [Test]
    public void GPUDeviceDescription_Parse_ReturnsNullWhenInputIsNull() =>
        Assert.IsNull(GPUDeviceDescription.Parse(null));

    [Test]
    public void GPUDeviceDescription_Parse_ReturnsDescriptionForCUDA()
    {
        const string value = "Platform:0 Device:0 Bus:0 Slot:4 Compute:7.5 Driver:11.4";
        var description = GPUDeviceDescription.Parse(value);
        Assert.AreEqual(0, description.Platform);
        Assert.AreEqual(0, description.Device);
        Assert.AreEqual(0, description.Bus);
        Assert.AreEqual(4, description.Slot);
        Assert.AreEqual("7.5", description.Compute);
        Assert.AreEqual("11.4", description.Driver);
    }

    [Test]
    public void GPUDeviceDescription_Parse_ReturnsDescriptionForOpenCL()
    {
        const string value = "Platform:0 Device:1 Bus:5 Slot:0 Compute:1.2 Driver:3004.8";
        var description = GPUDeviceDescription.Parse(value);
        Assert.AreEqual(0, description.Platform);
        Assert.AreEqual(1, description.Device);
        Assert.AreEqual(5, description.Bus);
        Assert.AreEqual(0, description.Slot);
        Assert.AreEqual("1.2", description.Compute);
        Assert.AreEqual("3004.8", description.Driver);
    }
}
