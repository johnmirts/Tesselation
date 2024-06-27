using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Tesselation {
    public class TesselationInfo : GH_AssemblyInfo {
        public override string Name => "Tesselation";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Identify and tesselate curved segments in the provided curves";

        public override Guid Id => new Guid("2a8639e7-0975-49a1-9c19-9722043782ec");

        //Return a string identifying you or your company.
        public override string AuthorName => "Ioannis Mirtsopoulos";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "ioannis@mirtsopoulos.xyz";
    }
}