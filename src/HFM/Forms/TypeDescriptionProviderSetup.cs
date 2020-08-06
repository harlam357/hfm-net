
using System.Diagnostics.CodeAnalysis;

using HFM.Core.Client;
using HFM.Core.Data;

namespace HFM.Forms
{
    public static class TypeDescriptionProviderSetup
    {
        [ExcludeFromCodeCoverage]
        public static void Execute()
        {
            Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(typeof(SlotModel));
            Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(typeof(WorkUnitRow));
        }
    }
}
