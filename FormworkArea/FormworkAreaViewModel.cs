#region Namespaces
using AlphaBIM.Models;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using TINHKHOILUONGVANKHUON;

#endregion

namespace AlphaBIM
{
    public class FormworkAreaViewModel : ViewModelBase
    {
        public FormworkAreaViewModel(UIDocument uiDoc)
        {
            // Khởi tạo sự kiện(nếu có) | Initialize event (if have)

            // Lưu trữ data từ Revit | Store data from Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            // Khởi tạo data cho WPF | Initialize data for WPF
            Initialize();

            // Get setting(if have)

        }

        private void Initialize()
        {
            //Tạo share para cần thiết
            CreateShareParameterFormworkArea.CreateShareParameter(UiDoc);

            //Khởi tạo sự kiện

            IList<Reference> references = UiDoc.Selection.PickObjects(ObjectType.Element,   
                                          new BeamColumnSelectionFilter(), "Chọn cột và dầm");
            Beamcolumns = references.Select(x => Doc.GetElement(x)).ToList();
          
        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        #endregion public property

        #region private variable

        //private double _percent;
        //private IEnumerable<Element> intersectElements;

        #endregion private variable

         #region Binding properties
        public List<Element> Beamcolumns { get; set; }

       
        //List<Element> intersectElements;
        public bool IsCurrentViewScope { get; set; }
        public bool IsCurrentSelection { get; set; }
        public bool IsCalBeamBottom { get; set; }
        public bool IsEntireProject { get; set; }

        //public IEnumerable<Element> intersectElements 
        // List<Element> intersectElements = new List<Element>();
        #endregion Binding properties

        // Các method khác viết ở dưới đây | Other methods written below
        internal void ClickOk()
        {
           
            using (Transaction tran = new Transaction(Doc, "Formwork"))
            {
                tran.Start();

                foreach (Element element in Beamcolumns)
                {
                    Solid solid1 = element.GetSolid();// Lấy được đối tượng rồi 

                    #region // Lấy về tất cả đối tượng intersect

                    ElementCategoryFilter categoryFilterBeam
                        = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
                    ElementCategoryFilter categoryFilterColumns
                        = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);

                    BoundingBoxXYZ box = element.get_BoundingBox(Doc.ActiveView);
                    Outline outline = new Outline(box.Min, box.Max);
                    BoundingBoxIntersectsFilter bbFilter
                        = new BoundingBoxIntersectsFilter(outline);

                    LogicalOrFilter logicalOrFilter
                        = new LogicalOrFilter(categoryFilterBeam, categoryFilterColumns);
                    LogicalAndFilter logicalAndFilter
                        = new LogicalAndFilter(new List<ElementFilter>()
                        {
                            logicalOrFilter,bbFilter
                        });
                    List<Element> intersectElements;

                    if(IsEntireProject)
                    {
                        intersectElements
                          = new FilteredElementCollector(Doc)
                              .WherePasses(logicalAndFilter)
                              .ToList();
                    }    

                    else if (IsCurrentViewScope)
                    {
                        intersectElements
                            = new FilteredElementCollector(Doc,Doc.ActiveView.Id)
                                .WherePasses(logicalAndFilter)
                                .ToList();
                    }
                    else
                    {
                        intersectElements
                            = new FilteredElementCollector(Doc, Beamcolumns.Select(x => x.Id).ToList())
                                .WherePasses(logicalAndFilter)
                                .ToList();
                    }

                    intersectElements = intersectElements
                        .Where(x => x.Id.IntegerValue != element.Id.IntegerValue)
                        .ToList();
                    #endregion
                    //có đc các đối tượng dầm cột giao nhau 


                    List<Face> faces1 = solid1.GetFaces();// có diện tích của face
                    double solid1Area = 0;

                    double beamBottom = 0;

                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                    {
                        solid1Area = faces1.CalAreaNotTopNotBottom();
                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {
                        if (IsCalBeamBottom)
                        {
                            solid1Area = faces1.CalAreaNotTop();
                            beamBottom = faces1.CalAreaOnlyBottom();
                        }
                        else
                        {
                            solid1Area = faces1.CalAreaNotTopNotBottom();
                        }
                    }
                    double totalArea = solid1Area;

                    double beamSubBean = 0;
                    double beamSubCol = 0;

                    double colSubCol = 0;
                    double colSubBeam = 0;


                   
                    foreach (Element intersectElement in intersectElements)
                    {
                        Solid solid2 = intersectElement.GetSolid();

                        List<Face> faces2 = solid2.GetFaces();
                        double solid2Area = 0;
                        if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                        {
                            solid2Area = faces2.CalAreaNotTopNotBottom();
                        }
                        else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                        {
                            if (!IsCalBeamBottom)
                            {
                                solid2Area = faces2.CalAreaNotTopNotBottom();
                            }
                            else
                            {
                                solid2Area = faces2.CalAreaNotTop();
                            }
                        }

                        try
                        {
                            Solid union = BooleanOperationsUtils.ExecuteBooleanOperation(
                                solid1,
                                solid2,
                                BooleanOperationsType.Union);
                            List<Face> facesUnion = union.GetFaces();
                            double uionArea = 0;
                            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                            {
                                uionArea = facesUnion.CalAreaNotTopNotBottom();
                            }
                            else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                            {
                                if (!IsCalBeamBottom)
                                {
                                    uionArea = facesUnion.CalAreaNotTopNotBottom();
                                }
                                else
                                {
                                    uionArea = facesUnion.CalAreaNotTop();
                                }
                            }

                            double areaIntersect = (solid1Area + solid2Area - uionArea) / 2;

                            if (areaIntersect > 0)
                            {
                                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        colSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        colSubBeam += areaIntersect;
                                    }
                                }
                                else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        beamSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        beamSubBean += areaIntersect;
                                    }

                                    if (IsCalBeamBottom)
                                    {
                                        beamBottom -= faces1.CalAreaOnlyBottom()
                                                    + faces2.CalAreaOnlyBottom()
                                                    - facesUnion.CalAreaNotTop();
                                    }
                                }
                            }
                        }
                        catch (Exception e )
                        {
                            //break;
                        }
                    }

                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                    {
                        Parameter nameAlpFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlpFormworkArea);
                        Parameter nameFwColumnTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnTotal);
                        Parameter nameFwColumnSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubBeam);
                        Parameter nameFwColumnSubColumn = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubColumn);
                        nameAlpFormworkArea.Set(0);
                        nameFwColumnTotal.Set(0);
                        nameFwColumnSubBeam.Set(0);
                        nameFwColumnSubColumn.Set(0);

                        nameAlpFormworkArea.Set(totalArea - colSubBeam - colSubCol);
                        nameFwColumnTotal.Set(totalArea);
                        nameFwColumnSubBeam.Set(colSubBeam);
                        nameFwColumnSubColumn.Set(colSubCol);

                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {
                        Parameter nameAlpFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlpFormworkArea);
                        Parameter nameFwBeamTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamTotal);
                        Parameter nameFwBeamBottom = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamBottom);
                        Parameter nameFwBeamSubCol =element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubCol);
                        Parameter nameFwBeamSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubBeam);

                        nameAlpFormworkArea.Set(0);
                        nameFwBeamTotal.Set(0);
                        nameFwBeamBottom.Set(0);
                        nameFwBeamSubCol.Set(0);
                        nameFwBeamSubBeam.Set(0);

                        nameAlpFormworkArea.Set(totalArea - beamSubBean - beamSubCol);
                        nameFwBeamTotal.Set(totalArea);
                        if (IsCalBeamBottom)
                        {
                            nameFwBeamBottom.Set(beamBottom);
                        }

                        nameFwBeamSubCol.Set(beamSubCol);
                        nameFwBeamSubBeam.Set(beamSubBean);

                    }
                }
                tran.Commit();
            }

        }
    }
}

