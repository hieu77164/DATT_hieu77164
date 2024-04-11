using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBIM;
using Autodesk.Revit.ApplicationServices;
using  Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TINHKHOILUONGVANKHUON
{
   internal class CreateShareParameterFormworkArea
    {
        internal static void CreateShareParameter(UIDocument uiDoc)
        {
            Document doc = uiDoc.Document;
            Application app = uiDoc.Application.Application;

            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path = System.IO.Path.Combine(pathDesktop, "ALB_ShareParameter.txt");
            string group = "STR";

            List<Category> categories = new List<Category>();
            categories.Add(Category.GetCategory(doc,BuiltInCategory.OST_StructuralFraming));
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));

            List<Category> categoriesBeam = new List<Category>();
            categoriesBeam.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming));

            List<Category> categoriesColum = new List<Category>();
            categoriesColum.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));

            Transaction t = new Transaction(doc, " Tạo ShareParameter");
            t.Start();

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameAlpFormworkArea,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionAlFormworkArea,
                categories,
                true);
            
            
            #region Para của dầm 
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamTotal,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamBottom,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamBottom,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubCol,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubCol,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubBeam,
                categories,
                true);
            #endregion

            #region Para của cột 
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnTotal,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnBottom,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnBottom,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubBeam,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubColumn,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubColumn,
                categories,
                true);
            #endregion

            t.Commit();
        }



        internal static String NameAlpFormworkArea { get; set; } = "FW_FormworkArea";
        internal static String DescriptionAlFormworkArea { get; set; } = "Diện tích ván khuôn";


        internal static string NameFwBeamTotal { get; set; } = "FW.Beam.Total";
        internal static string DescriptionFwBeamTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwBeamBottom { get; set; } = "FW.Beam.Bottom";
        internal static string DescriptionFwBeamBottom { get; set; } = "Diện tích ván khuôn đáy";
        internal static string NameFwBeamSubCol { get; set; } = "FW.Beam.SubCol";
        internal static string DescriptionFwBeamSubCol { get; set; } = "Diện tích tiếp xúc với cột";
        internal static string NameFwBeamSubBeam { get; set; } = "FW.Beam.SubBeam";
        internal static string DescriptionFwBeamSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";

        internal static string NameFwColumnTotal { get; set; } = "FW.Colum.Total";
        internal static string DescriptionFwColumnTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwColumnBottom { get; set; } = "FW.Colum.Bottom";
        internal static string DescriptionFwColumnBottom { get; set; } = "Diện tích ván khuôn đáy";
        internal static string NameFwColumnSubBeam { get; set; } = "FW.Colum.SubBeam";
        internal static string DescriptionFwColumnSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
        internal static string NameFwColumnSubColumn { get; set; } = "FW.Colum.SubColum";
        internal static string DescriptionFwColumnSubColumn { get; set; } = "Diện tích tiếp xúc với cột";


    }
}
