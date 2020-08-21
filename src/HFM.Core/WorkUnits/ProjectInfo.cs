namespace HFM.Core.WorkUnits
{
    public interface IProjectInfo
    {
        /// <summary>
        /// Project ID Number
        /// </summary>
        int ProjectID { get; }

        /// <summary>
        /// Project ID (Run)
        /// </summary>
        int ProjectRun { get; }

        /// <summary>
        /// Project ID (Clone)
        /// </summary>
        int ProjectClone { get; }

        /// <summary>
        /// Project ID (Gen)
        /// </summary>
        int ProjectGen { get; }
    }

    public class ProjectInfo : IProjectInfo
    {
        /// <summary>
        /// Project ID Number
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// Project ID (Run)
        /// </summary>
        public int ProjectRun { get; set; }

        /// <summary>
        /// Project ID (Clone)
        /// </summary>
        public int ProjectClone { get; set; }

        /// <summary>
        /// Project ID (Gen)
        /// </summary>
        public int ProjectGen { get; set; }

        public override string ToString()
        {
            return this.ToProjectString();
        }
    }
}
