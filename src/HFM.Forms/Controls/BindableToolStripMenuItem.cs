using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace HFM.Forms.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public class BindableToolStripMenuItem : ToolStripMenuItem, IBindableComponent
    {
        private BindingContext _context;

        public BindingContext BindingContext
        {
            get => _context ?? (_context = new BindingContext());
            set => _context = value;
        }

        private ControlBindingsCollection _bindings;

        public ControlBindingsCollection DataBindings
        {
            get => _bindings ?? (_bindings = new ControlBindingsCollection(this));
            set => _bindings = value;
        }
    }
}
