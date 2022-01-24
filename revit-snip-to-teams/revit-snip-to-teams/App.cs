using Autodesk.Revit.UI;
using AW = Autodesk.Windows;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;

namespace revit-snip-to-teams
{
    public class App : IExternalApplication
    {
        public AW.RibbonControl RibbonControl;

        public string TabName;

        public static UIControlledApplication UIapp;
        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            try
            {
                UIapp = application;
                TabName = AddRibbonTab();
                RibbonPanel panel = AddRibbonPanel();
                AssembleTools(panel);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }

            return Result.Succeeded;
        }

        private RibbonPanel AddRibbonPanel()
        {
            List<RibbonPanel> panels = UIapp.GetRibbonPanels(TabName);

            foreach (RibbonPanel _panel in panels)
            {
                if (_panel.Name.Contains("revit-snip-to-teams"))
                {
                    return _panel;
                }
            }
            return UIapp.CreateRibbonPanel(TabName, "revit-snip-to-teams");
        }

        private string AddRibbonTab()
        {
            RibbonControl = AW.ComponentManager.Ribbon;
            AW.RibbonTab ribbonTab = RibbonControl.FindTab("Digital Tools");
            string res = null;

            if (ribbonTab == null)
            {
                UIapp.CreateRibbonTab("Digital Tools");
                ribbonTab = RibbonControl.FindTab("Digital Tools");
                if (ribbonTab != null)
                {
                    res = "Digital Tools";
                }
            }
            else
            {
                res = "Digital Tools";
            }
            if (ribbonTab == null)
            {
                ribbonTab = RibbonControl.FindTab("Add-ins");
                if (ribbonTab != null)
                {
                    res = "Add-ins";
                }
            }
            return res;
        }
        private void AssembleTools(RibbonPanel panel)
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string dir = Path.GetDirectoryName(thisAssemblyPath);

            #region DLL File Paths
            string feedbackLocation = Path.Combine(dir, "revit-snip-to-teams.dll");
            #endregion
            #region Feedback Form
            PushButtonData pbdFeedback = new PushButtonData("revit-snip-to-teams", "Post\nTeams", feedbackLocation, typeof(revit-snip-to-teams.Command).FullName);
            pbdFeedback.LargeImage = new BitmapImage(uriSource: new Uri("pack://application:,,,/revit-snip-to-teams;component/Resources/feedback.png"));
            panel.AddItem(pbdFeedback);
            #endregion
        }
        
    }
}
