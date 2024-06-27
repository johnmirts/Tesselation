using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tesselation {
    public class TesselationComponent : BaseComponent {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public TesselationComponent()
          : base("Curve Tesselation",
                 "CrvTesselation",
                 "Controlled tesselation of curved segments of a curve",
                 ComponentCategory.Tesselation,
                 GH_Exposure.tertiary,
                 Properties.Resources.icon_question) {
        }

        public override void AddedToDocument(GH_Document document) {
            //Be sure that the value in Params.Input[value] is the index of the input you want to place the number slider
            if (Params.Input[1].SourceCount == 0) {
                // Perform Layout to get actual positionning of the component on the canvas
                Attributes.ExpireLayout();
                Attributes.PerformLayout();

                //instantiate new number slider
                var tesselations = new GH_NumberSlider();
                tesselations.CreateAttributes();

                // place the objects
                document.AddObject(tesselations, false);

                // plug the number slider to the GH component
                Params.Input[1].AddSource(tesselations);
                tesselations.Slider.Minimum = 1;
                tesselations.Slider.Maximum = 10;
                tesselations.Slider.DecimalPlaces = 0;
                tesselations.SetSliderValue(5);

                //get the pivot of the number slider
                PointF currPivot = Params.Input[1].Attributes.Pivot;

                //set the pivot of the new object
                tesselations.Attributes.Pivot = new PointF(currPivot.X - 228, currPivot.Y - 10);
            }

            base.AddedToDocument(document);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddCurveParameter("Curve", "Curve", "The curve to tesselater (if necessary)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Divisions", "Divisions", "The number of divisions", GH_ParamAccess.item, 5);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddCurveParameter("Curve", "Curve", "The updated (tessealted) curve", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            Curve crv = new LineCurve();
            DA.GetData(0, ref crv);

            int div_count = 0;
            DA.GetData(1, ref div_count);

            List<int> curvedIds = _curvedSegIDs(crv);
            Message = curvedIds.Count + "x curved segment(s)";
            DA.SetDataList(0, _updatedCurve(crv.DuplicateSegments(), curvedIds, div_count));
        }

        private Curve[] _updatedCurve(Curve[] segments, List<int> ids, int div_count) {
            Curve[] final_segs = new Curve[segments.Length];
            foreach (var kvp in segments.Select((value, index) => new { value, index })) {
                if (ids.Contains(kvp.index)) {
                    final_segs[kvp.index] = _tesselateCurve(kvp.value, div_count);
                }
                else {
                    final_segs[kvp.index] = segments[kvp.index];
                }
            }
            return Curve.JoinCurves(final_segs, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
        }

        public List<int> _curvedSegIDs(Curve crv) {

            Curve[] segments = crv.DuplicateSegments();
            List<int> curvedSegIDs = new List<int>();
            foreach (var kvp in segments.Select((value, index) => new { value, index })) {
                Curve seg = segments[kvp.index];
                Vector3d guide_vec = new Vector3d(seg.PointAtEnd) - new Vector3d(seg.PointAtStart);

                double[] parameters = seg.DivideByCount(3, false);
                foreach (double param in parameters) {
                    Vector3d tangent = seg.TangentAt(param);
                    if (!_isParallel(guide_vec, tangent)) {
                        curvedSegIDs.Add(kvp.index);
                        break;
                    }
                }
            }
            return curvedSegIDs;
        }

        private Curve _tesselateCurve(Curve seg, int div_count) {
            Point3d[] pts = new Point3d[div_count + 1];
            foreach (var kvp in (seg.DivideByCount(div_count, true)).Select((value, index) => new { value, index })) {
                pts[kvp.index] = seg.PointAt(kvp.value);
            }
            Polyline p = new Polyline(pts);
            return p.ToNurbsCurve();
        }

        public bool _isParallel(Vector3d vecA, Vector3d vecB) {
            int parallelCheck = vecA.IsParallelTo(vecB, RhinoMath.DefaultAngleTolerance);
            if (parallelCheck == -1 || parallelCheck == 1) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("2e862183-ff6b-43ed-b70f-a1324d7a0ef4"); }
        }        
    }
}