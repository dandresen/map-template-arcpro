using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MapLayoutFirst
{
    internal class BuildNewLayout : Button
    {
        async protected override void OnClick()
        {
            // create a layout with will be returned from the QueuedTask
            Layout newLayout = await QueuedTask.Run<Layout>(() =>
            {
                // create a new CIM page
                CIMPage newPage = new CIMPage();

                // add properties
                newPage.Width = 17;
                newPage.Height = 11;
                newPage.Units = LinearUnit.Inches;

                // apply rulers 
                newPage.ShowRulers = true;
                newPage.SmallestRulerDivision = 0.5;

                // apply CIM page toa new laout and set name
                newLayout = LayoutFactory.Instance.CreateLayout(newPage);
                newLayout.SetName("llx17_landscape_template");

                // create a new map with an arcgis online basemap
                //Map newMap = MapFactory.Instance.CreateMap("Census Map", MapType.Map, MapViewingMode.Map, Basemap.NationalGeographic);
                //string url = @"http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer";
                //Uri uri = new Uri(url);
                //LayerFactory.Instance.CreateLayer(uri, newMap);


                MapProjectItem mapPrjItem = Project.Current.GetItems<MapProjectItem>().FirstOrDefault(item => item.Name.Equals("Map Layers1"));
                Map newMap = mapPrjItem.GetMap();
                
                
                // build a map fram geometry / envelope
                // lower leftk, upper right
                Coordinate2D ll = new Coordinate2D(0.1, 0.6);
                Coordinate2D ur = new Coordinate2D(16.9, 10.4);
                Envelope mapEnv = EnvelopeBuilder.CreateEnvelope(ll, ur);

                // create a map frame and add it to the layout
                MapFrame newMapFrame = LayoutElementFactory.Instance.CreateMapFrame(newLayout, mapEnv, newMap);
                newMapFrame.SetName("Map Frame");

                // create and set the camera
                Camera camera = newMapFrame.Camera;
                camera.X = 988687;
                camera.Y = 1012363;
                camera.Scale = 6000000;
                newMapFrame.SetCamera(camera);


                // blue box
                Coordinate2D bottomBox_ll = new Coordinate2D(0.1, 0.1);
                Coordinate2D bottomBox_ur = new Coordinate2D(16.9, 0.6);
                Envelope bottomBoxEnv = EnvelopeBuilder.CreateEnvelope(bottomBox_ll, bottomBox_ur);
                var boxBottomElmSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.CreateRGBColor(15,56,90), SimpleFillStyle.Solid);
                GraphicElement bottomBoxElm = LayoutElementFactory.Instance.CreatePolygonGraphicElement(newLayout, bottomBoxEnv, boxBottomElmSymbol);
                bottomBoxElm.SetName("Bottom Banner Box");

                Coordinate2D topBox_ll = new Coordinate2D(0.1, 10.4);
                Coordinate2D topBox_ur = new Coordinate2D(16.9, 10.9);
                Envelope topBoxEnv = EnvelopeBuilder.CreateEnvelope(topBox_ll, topBox_ur);
                //var boxTopElmSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.CreateRGBColor(15, 56, 90), SimpleFillStyle.Solid);
                GraphicElement topBoxElm = LayoutElementFactory.Instance.CreatePolygonGraphicElement(newLayout, topBoxEnv, boxBottomElmSymbol);
                topBoxElm.SetName("Top Banner Box");

                // add text map creator
                Coordinate2D titleTxt_ll = new Coordinate2D(8.5, 0.172);
                CIMTextSymbol arial36bold = SymbolFactory.Instance.ConstructTextSymbol(ColorFactory.Instance.WhiteRGB, 7.5, "Arial", "Bold");
                string mapCredits = "<ALIGN horizontal = 'center'>Texas Department of Transportation \n Department Here \n <dyn type= 'date' format= 'MMMMMMMMMMMM dd, yyyy' /></ALIGN> ";
                GraphicElement titleTxtElm = LayoutElementFactory.Instance.CreatePointTextGraphicElement(newLayout, titleTxt_ll, mapCredits, arial36bold);
                titleTxtElm.SetName("Bottom Banner - Author");

                // add text district
                Coordinate2D districtTxt_ll = new Coordinate2D(0.89, 10.4703);
                CIMTextSymbol impact20Regular = SymbolFactory.Instance.ConstructTextSymbol(ColorFactory.Instance.WhiteRGB, 20, "Impact", "Regular");
                string districtText = "<ALIGN horizontal = 'center'>DISTRICT</ALIGN> ";
                GraphicElement districtTxtElm = LayoutElementFactory.Instance.CreatePointTextGraphicElement(newLayout, districtTxt_ll, districtText, impact20Regular);
                districtTxtElm.SetName("Top Banner - District");

                // add text date year
                Coordinate2D dateTxt_ll = new Coordinate2D(16.01, 10.47);
                //CIMTextSymbol impact20Regular = SymbolFactory.Instance.ConstructTextSymbol(ColorFactory.Instance.WhiteRGB, 20, "Impact", "Regular");
                string dateText = "<ALIGN horizontal = 'center'><dyn type= 'date' format=  'yyyy' /></ALIGN> ";
                GraphicElement dateTxtElm = LayoutElementFactory.Instance.CreatePointTextGraphicElement(newLayout, dateTxt_ll, dateText, impact20Regular);
                dateTxtElm.SetName("Top Banner - Date");

                // add north arrow
                // reference north arrow in a style
                StyleProjectItem stylePrjItm = Project.Current.GetItems<StyleProjectItem>().FirstOrDefault(item => item.Name == "ArcGIS 2D");
                NorthArrowStyleItem naStyleItm = stylePrjItm.SearchNorthArrows("ArcGIS North 1")[0];

                // set the center coordinate and create the arrow on the layout
                Coordinate2D center = new Coordinate2D(0.458,0.458);
                NorthArrow arrowElem = LayoutElementFactory.Instance.CreateNorthArrow(newLayout, center, newMapFrame, naStyleItm);
                arrowElem.SetName("North Arrow");
                arrowElem.SetHeight(0.3198);

                //// add legend- first build 2d envelope geometry
                //Coordinate2D leg_ll = new Coordinate2D(0.45, 0.40);
                //Coordinate2D leg_ur = new Coordinate2D(0.403, 0.401);
                //Envelope leg_env = EnvelopeBuilder.CreateEnvelope(leg_ll, leg_ur);

                //// create legend
                //Legend legendElm = LayoutElementFactory.Instance.CreateLegend(newLayout, leg_env, newMapFrame);
                //legendElm.SetVisible(true);
                //legendElm.SetName("New Legend");

                return newLayout;

            });

            // open layout pane and clear selection in the layout

            var layoutPane = await ProApp.Panes.CreateLayoutPaneAsync(newLayout);
            var sel = layoutPane.LayoutView.GetSelectedElements();
            if (sel.Count > 0)
            {
                layoutPane.LayoutView.ClearElementSelection();
            }

        }
    }
}
