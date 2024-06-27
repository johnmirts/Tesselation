using Grasshopper.Kernel;
using System.Drawing;

namespace Tesselation {
    public abstract class BaseComponent : GH_Component {

        private readonly GH_Exposure exposure = GH_Exposure.hidden;
        private readonly Bitmap icon = null;

        public BaseComponent(string _componentName, string _nickname, string _description, ComponentCategory _subcategory, GH_Exposure _exposure, Bitmap _icon)
            : base(_componentName, _nickname, _description, "Tesselation", _subcategory.ToString()) {
            exposure = _exposure;
            icon = _icon;
            IconDisplayMode = GH_IconDisplayMode.icon;
        }
        protected override void BeforeSolveInstance() {
            base.BeforeSolveInstance();
        }
        public override GH_Exposure Exposure {
            get { return exposure; }
        }
        protected override Bitmap Icon {
            get {
                return icon;
            }
        }
    }

    public enum ComponentCategory : byte {
        Tesselation
    }
}
