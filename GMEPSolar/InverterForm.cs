using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using GMEPUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GMEPSolar
{
    public partial class InverterForm : Form
    {
        Dictionary<int, Dictionary<string, Dictionary<string, object>>> DCSolarData =
            new Dictionary<int, Dictionary<string, Dictionary<string, object>>>();
        DC_SOLAR_INPUT form;
        int initialID;

        public InverterForm()
        {
            initialID = 1;
            InitializeComponent();
            CreateNewPanelTab("Inverter");
            this.form = new DC_SOLAR_INPUT();
        }

        private void CreateInverter(
            Point3d point,
            Dictionary<string, object> inverterData,
            Editor ed
        )
        {
            string path = "block data/Inverter2P.json";

            if (!Convert.ToBoolean(inverterData["is2P"]))
            {
                path = "block data/Inverter3P.json";
            }

            var data = BlockDataMethods.GetData(path);
            Point3d center1 = new Point3d(
                -0.33597823956413464 + point.X,
                -3.0767796081386578 + point.Y,
                0.0 + point.Z
            );
            double radius1 = 0.039236382599967569;
            Point3d center2 = new Point3d(
                -0.20286513573002907 + point.X,
                -3.0767796081386578 + point.Y,
                0.0 + point.Z
            );
            double radius2 = 0.039236382599967569;

            BlockDataMethods.CreateObjectGivenData(data, ed, point);
            BlockDataMethods.CreateFilledCircleInPaperSpace(center1, radius1);
            BlockDataMethods.CreateFilledCircleInPaperSpace(center2, radius2);
        }

        public InverterUserControl CreateNewPanelTab(string tabName)
        {
            InverterUserControl inverterUserControl = new InverterUserControl(this);
            this.Controls.Add(inverterUserControl);

            var numberOfTabs = INVERTER_TABS.TabPages.Count + 1;
            var tabNameWithNumber = tabName + " " + numberOfTabs;

            TabPage newTabPage = new TabPage(tabNameWithNumber);
            newTabPage.Tag = initialID;
            initialID++;

            INVERTER_TABS.TabPages.Add(newTabPage);
            INVERTER_TABS.SelectedTab = newTabPage;

            AddUserControlToTab(inverterUserControl, newTabPage);

            return inverterUserControl;
        }

        public void AddUserControlToTab(UserControl control, TabPage tabPage)
        {
            control.Location = new Point(0, 0);
            control.Dock = DockStyle.Fill;

            tabPage.Controls.Add(control);
        }

        internal TabControl GetInverterTabs()
        {
            return INVERTER_TABS;
        }

        internal void SaveDCData(
            Dictionary<int, Dictionary<string, Dictionary<string, object>>> newDataObject
        )
        {
            DCSolarData = newDataObject;
        }

        internal Dictionary<int, Dictionary<string, Dictionary<string, object>>> GetCurrentDCData()
        {
            return DCSolarData;
        }

        internal void RemoveFromDCData(int tabID)
        {
            DCSolarData.Remove(tabID);
        }

        internal bool DoesDCSolarDataHaveThisID(int tabID)
        {
            return DCSolarData.ContainsKey(tabID);
        }

        internal void FillDCSolarFormWithExistingData(int tabID, DC_SOLAR_INPUT form)
        {
            var data = DCSolarData[tabID];
            foreach (var mpptData in data)
            {
                var mpptName = mpptData.Key.ToString();
                if (mpptName == "MPPT1")
                {
                    var mppt1Data = mpptData.Value;
                    var mppt1Enabled = Convert.ToBoolean(mppt1Data["Enabled"]);
                    var mppt1Regular = Convert.ToBoolean(mppt1Data["Regular"]);
                    var mppt1Parallel = Convert.ToBoolean(mppt1Data["Parallel"]);
                    var mppt1Input = mppt1Data["Input"];
                    form.UpdateValues(
                        "MPPT1",
                        mppt1Enabled,
                        mppt1Regular,
                        mppt1Parallel,
                        mppt1Input
                    );
                }
                else if (mpptName == "MPPT2")
                {
                    var mppt2Data = mpptData.Value;
                    var mppt2Enabled = Convert.ToBoolean(mppt2Data["Enabled"]);
                    var mppt2Regular = Convert.ToBoolean(mppt2Data["Regular"]);
                    var mppt2Parallel = Convert.ToBoolean(mppt2Data["Parallel"]);
                    var mppt2Input = mppt2Data["Input"];
                    form.UpdateValues(
                        "MPPT2",
                        mppt2Enabled,
                        mppt2Regular,
                        mppt2Parallel,
                        mppt2Input
                    );
                }
                else if (mpptName == "MPPT3")
                {
                    var mppt3Data = mpptData.Value;
                    var mppt3Enabled = Convert.ToBoolean(mppt3Data["Enabled"]);
                    var mppt3Regular = Convert.ToBoolean(mppt3Data["Regular"]);
                    var mppt3Parallel = Convert.ToBoolean(mppt3Data["Parallel"]);
                    var mppt3Input = mppt3Data["Input"];
                    form.UpdateValues(
                        "MPPT3",
                        mppt3Enabled,
                        mppt3Regular,
                        mppt3Parallel,
                        mppt3Input
                    );
                }
                else if (mpptName == "MPPT4")
                {
                    var mppt4Data = mpptData.Value;
                    var mppt4Enabled = Convert.ToBoolean(mppt4Data["Enabled"]);
                    var mppt4Regular = Convert.ToBoolean(mppt4Data["Regular"]);
                    var mppt4Parallel = Convert.ToBoolean(mppt4Data["Parallel"]);
                    var mppt4Input = mppt4Data["Input"];
                    form.UpdateValues(
                        "MPPT4",
                        mppt4Enabled,
                        mppt4Regular,
                        mppt4Parallel,
                        mppt4Input
                    );
                }
            }
        }

        private Dictionary<string, object> GetInverterFormData()
        {
            var inverterFormData = new Dictionary<string, object>();
            var inverterEncapsulation = new List<Dictionary<string, object>>();
            var inverterEncapsulationExtra = new Dictionary<string, object>();
            var inverterTabs = INVERTER_TABS.TabPages;

            foreach (TabPage tab in inverterTabs)
            {
                var tabID = (int)tab.Tag;
                if (DCSolarData.ContainsKey(tabID))
                {
                    inverterFormData.Add("DCSolarData", DCSolarData[tabID]);
                }
                var control = tab.Controls[0] as InverterUserControl;
                var controlData = control.GetInverterFormData();
                inverterFormData.Add("is2P", controlData["is2P"]);
                inverterFormData.Add("isMaster", controlData["isMaster"]);
                inverterEncapsulation.Add(inverterFormData);
                inverterFormData = new Dictionary<string, object>();
            }

            var configuration = GetConfigurationOfInverters();

            inverterEncapsulationExtra.Add("InverterData", inverterEncapsulation);
            inverterEncapsulationExtra.Add("Configuration", configuration);
            return inverterEncapsulationExtra;
        }

        private object GetConfigurationOfInverters()
        {
            if (EMPTY_RADIO.Checked)
            {
                return "EMPTY";
            }
            else if (GRID_RADIO.Checked)
            {
                return "GRID";
            }
            else if (LOAD_RADIO.Checked)
            {
                return "LOAD";
            }
            else if (GRID_LOAD_RADIO.Checked)
            {
                return "GRID_LOAD";
            }
            else
            {
                return "EMPTY";
            }
        }

        private int GetNumberOfConduit(Dictionary<string, Dictionary<string, object>> dcSolarData)
        {
            int numberOfConduit = 0;
            foreach (var stringData in dcSolarData.Values)
            {
                if (stringData.ContainsKey("items"))
                {
                    var items = stringData["items"] as List<string>;
                    numberOfConduit += items.Count(item => item == "string");
                }
            }
            return numberOfConduit * 2;
        }

        private static double GetTotalHeightOfStringModule(
            List<Dictionary<string, object>> stringDataContainer
        )
        {
            double totalHeight = 0;
            foreach (var stringData in stringDataContainer)
            {
                totalHeight += Convert.ToDouble(stringData["height"]);
            }
            return totalHeight;
        }

        private static List<Dictionary<string, object>> GetStringData(
            Dictionary<string, Dictionary<string, object>> formData
        )
        {
            var stringDataContainer = new List<Dictionary<string, object>>();
            var firstFlag = true;
            var previousModulesCount = 0;

            foreach (var mppt in formData)
            {
                var stringData = new Dictionary<string, object>();
                stringData["height"] = 0.0;
                stringData["items"] = new List<string>();

                var mpptData = mppt.Value;
                stringData["module"] = mpptData["Input"];

                if (Convert.ToBoolean(mpptData["Enabled"]))
                {
                    var isRegular = Convert.ToBoolean(mpptData["Regular"]);
                    var isParallel = Convert.ToBoolean(mpptData["Parallel"]);
                    var modulesCount = 0;
                    if (isRegular || isParallel)
                    {
                        int.TryParse((string)mpptData["Input"], out modulesCount);
                    }

                    if (firstFlag && (isRegular || isParallel))
                    {
                        firstFlag = false;
                        stringData["height"] = (double)stringData["height"] + 0.8334;
                        ((List<string>)stringData["items"]).Add("text");
                        previousModulesCount = modulesCount;
                    }
                    else if ((isRegular || isParallel) && modulesCount != previousModulesCount)
                    {
                        stringData["height"] = (double)stringData["height"] + 0.8334;
                        ((List<string>)stringData["items"]).Add("text");
                        previousModulesCount = modulesCount;
                    }

                    if (isRegular)
                    {
                        stringData["height"] = (double)stringData["height"] + 1.0695;
                        ((List<string>)stringData["items"]).Add("string");
                    }
                    else if (isParallel)
                    {
                        stringData["height"] = (double)stringData["height"] + (1.0695 * 2);
                        ((List<string>)stringData["items"]).Add("string");
                        ((List<string>)stringData["items"]).Add("string");
                    }
                }
                stringDataContainer.Add(stringData);
            }
            return stringDataContainer;
        }

        private static int GetNumberOfMPPTs(Dictionary<string, Dictionary<string, object>> formData)
        {
            int numberOfMPPTs = 0;
            foreach (var mppt in formData)
            {
                var mpptData = mppt.Value as Dictionary<string, object>;
                if (Convert.ToBoolean(mpptData["Enabled"]))
                {
                    numberOfMPPTs++;
                }
            }

            return numberOfMPPTs;
        }

        private void CREATE_BUTTON_Click(object sender, EventArgs e)
        {
            var inverterFormData = GetInverterFormData();
            var INVERTER_HEIGHT = 5.75;
            var REDUCING_FACTOR = 0.081;
            var MAXIMUM_CONDUIT = 16;

            Close();

            Editor ed;
            PromptPointResult pointResult;
            HelperMethods.GetUserToClick(out ed, out pointResult);

            if (pointResult.Status == PromptStatus.OK)
            {
                var point = pointResult.Value;

                using (
                    DocumentLock docLock =
                        Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument()
                )
                {
                    if (inverterFormData.ContainsKey("InverterData"))
                    {
                        var inverterData =
                            inverterFormData["InverterData"] as List<Dictionary<string, object>>;

                        foreach (var currentInverterData in inverterData)
                        {
                            CreateInverter(point, currentInverterData, ed);

                            if (currentInverterData.ContainsKey("DCSolarData"))
                            {
                                var dcSolarData =
                                    currentInverterData["DCSolarData"]
                                    as Dictionary<string, Dictionary<string, object>>;

                                var numberOfMPPTs = GetNumberOfMPPTs(dcSolarData);
                                string json;

                                if (numberOfMPPTs != 0)
                                {
                                    json = BlockDataMethods.GetUnparsedJSONData(
                                        $"dc solar placement data/DCSolar{numberOfMPPTs}PlacementPoint.json"
                                    );
                                }
                                else
                                {
                                    return;
                                }

                                var parsedData = JsonConvert.DeserializeObject<
                                    Dictionary<string, double>
                                >(json);

                                var placement = new Point3d(
                                    parsedData["X"] + point.X,
                                    parsedData["Y"] + point.Y,
                                    parsedData["Z"] + point.Z
                                );

                                var stringDataContainer = GetStringData(dcSolarData);

                                var numberOfConduit = GetNumberOfConduit(dcSolarData);

                                var stringTotalHeight = GetTotalHeightOfStringModule(
                                    stringDataContainer
                                );

                                var lineStartPointX =
                                    point.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1)));
                                var lineStartPointY = point.Y - 0.6098;

                                var stringDefaultMidPointX = lineStartPointX - 3.3606;
                                var stringDefaultMidPointY = lineStartPointY - 1.3;

                                var topOfStringY = lineStartPointY + 2.9784;

                                var shiftYDown = (
                                    topOfStringY - stringDefaultMidPointY - stringTotalHeight / 2
                                );

                                DC_SOLAR_INPUT.CreateDCSolarObject(
                                    dcSolarData,
                                    shiftYDown.ToString(),
                                    "0",
                                    placement
                                );

                                var adjustedInverterHeight =
                                    INVERTER_HEIGHT
                                    - ((MAXIMUM_CONDUIT - numberOfConduit) * REDUCING_FACTOR);

                                if (stringTotalHeight > adjustedInverterHeight)
                                {
                                    point = new Point3d(
                                        point.X,
                                        point.Y - (stringTotalHeight),
                                        point.Z
                                    );
                                }
                                else
                                {
                                    point = new Point3d(
                                        point.X,
                                        point.Y - (adjustedInverterHeight),
                                        point.Z
                                    );
                                }
                            }
                            else
                            {
                                point = new Point3d(point.X, point.Y - 4.25, point.Z);
                            }
                        }
                    }
                }
            }
        }
    }
}
