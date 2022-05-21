using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class SlotDescriptionTests
{
    [Test]
    public void SlotDescription_Parse_NullDescription()
    {
        // Arrange
        const string cpu = null;
        // Act
        var description = SlotDescription.Parse(cpu);
        // Act
        Assert.IsNull(description);
    }

    [Test]
    public void SlotDescription_Parse_EmptyStringDescription()
    {
        // Arrange
        const string cpu = "";
        // Act
        var description = SlotDescription.Parse(cpu);
        // Act
        Assert.IsNull(description);
    }

    [Test]
    public void SlotDescription_Parse_WhiteSpaceStringDescription()
    {
        // Arrange
        const string cpu = "  ";
        // Act
        var description = SlotDescription.Parse(cpu);
        // Act
        Assert.IsNull(description);
    }

    [Test]
    public void SlotDescription_Parse_CPUSlotDescription()
    {
        // Arrange
        const string cpu = "cpu:16";
        // Act
        var description = (CPUSlotDescription)SlotDescription.Parse(cpu);
        // Act
        Assert.AreEqual(SlotType.CPU, description.SlotType);
        Assert.AreEqual(cpu, description.Value);
        Assert.AreEqual(16, description.CPUThreads);
    }

    [Test]
    public void SlotDescription_Parse_GPUSlotDescriptionWithBusAndSlot()
    {
        // Arrange
        const string gpu = "gpu:8:0 TU116 [GeForce GTX 1660 Ti]";
        // Act
        var description = (GPUSlotDescription)SlotDescription.Parse(gpu);
        // Act
        Assert.AreEqual(SlotType.GPU, description.SlotType);
        Assert.AreEqual(gpu, description.Value);
        Assert.AreEqual(8, description.GPUBus);
        Assert.AreEqual(0, description.GPUSlot);
        Assert.AreEqual("TU116", description.GPUPrefix);
        Assert.AreEqual("GeForce GTX 1660 Ti", description.Processor);
    }

    [Test]
    public void SlotDescription_Parse_GPUSlotDescriptionWithDevice()
    {
        // Arrange
        const string gpu = "gpu:0:TU104GL [Tesla T4]";
        // Act
        var description = (GPUSlotDescription)SlotDescription.Parse(gpu);
        // Act
        Assert.AreEqual(SlotType.GPU, description.SlotType);
        Assert.AreEqual(gpu, description.Value);
        Assert.AreEqual(null, description.GPUBus);
        Assert.AreEqual(null, description.GPUSlot);
        Assert.AreEqual("TU104GL", description.GPUPrefix);
        Assert.AreEqual("Tesla T4", description.Processor);
    }
}
