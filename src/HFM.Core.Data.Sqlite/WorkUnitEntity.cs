namespace HFM.Core.Data;

public record WorkUnitEntity
{
    public long ID { get; set; }

    public string DonorName { get; set; }

    public int DonorTeam { get; set; }

    public string CoreVersion { get; set; }

    public string Result { get; set; }

    public DateTime Assigned { get; set; }

    public DateTime? Finished { get; set; }

    public int ProjectRun { get; set; }

    public int ProjectClone { get; set; }

    public int ProjectGen { get; set; }

    public string HexID { get; set; }

    public int? FramesCompleted { get; set; }

    public int? FrameTimeInSeconds { get; set; }

    public long ProteinID { get; set; }

    public long ClientID { get; set; }

    public long? PlatformID { get; set; }

    public int? ClientSlot { get; set; }

    public virtual ProteinEntity Protein { get; set; }

    public virtual ClientEntity Client { get; set; }

    public virtual PlatformEntity Platform { get; set; }

    public virtual ICollection<WorkUnitFrameEntity> Frames { get; set; }

    public string SlotName { get; set; }

    public string SlotType { get; set; }

    public double PPD { get; set; }

    public double Credit { get; set; }
}

public record ProteinEntity
{
    public long ID { get; set; }

    public int ProjectID { get; set; }

    public double Credit { get; set; }

    public double KFactor { get; set; }

    public int Frames { get; set; }

    public string Core { get; set; }

    public int Atoms { get; set; }

    public double TimeoutDays { get; set; }

    public double ExpirationDays { get; set; }
}

public record ClientEntity
{
    public long ID { get; set; }

    public string Name { get; set; }

    public string ConnectionString { get; set; }

    public string Guid { get; set; }
}

public record PlatformEntity
{
    public long ID { get; set; }

    public string ClientVersion { get; set; }

    public string OperatingSystem { get; set; }

    // Reference, CPU, OpenCL, CUDA
    public string Implementation { get; set; }

    public string Processor { get; set; }

    // CPU specific
    public int? Threads { get; set; }

    // GPU specific
    public string DriverVersion { get; set; }

    public string ComputeVersion { get; set; }

    public string CUDAVersion { get; set; }
}

public record WorkUnitFrameEntity
{
    public long WorkUnitID { get; set; }

    public int FrameID { get; set; }

    public int RawFramesComplete { get; set; }

    public int RawFramesTotal { get; set; }

    public TimeSpan TimeStamp { get; set; }

    public TimeSpan Duration { get; set; }
}

public record VersionEntity
{
    public long ID { get; set; }

    public string Version { get; set; }
}
