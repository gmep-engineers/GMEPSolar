using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using GMEPUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        Editor ed,
        int slaveOrMasterNumber,
        int inverterCount
    )
    {
      string path = "block data/Inverter2P.json";
      var isMaster = Convert.ToBoolean(inverterData["isMaster"]);
      var is2P = Convert.ToBoolean(inverterData["is2P"]);

      if (!is2P)
      {
        path = "block data/Inverter3P.json";
      }

      var data = BlockDataMethods.GetData(path);

      if (isMaster)
      {
        CreateEMButton(point, ed);
        data = UpdateInverterTextToMaster(data, slaveOrMasterNumber);
      }
      else
      {
        data = UpdateInverterTextToSlave(data, slaveOrMasterNumber);
      }

      data = UpdateInverterTextWithInverterNumber(data, inverterCount);

      CreateFilledCircles(point);

      BlockDataMethods.CreateObjectGivenData(data, ed, point);
    }

    private List<
        Dictionary<string, Dictionary<string, object>>
    > UpdateInverterTextWithInverterNumber(
        List<Dictionary<string, Dictionary<string, object>>> data,
        int inverterCount
    )
    {
      var inverterText = "INVERTER #" + inverterCount;
      foreach (var dict in data)
      {
        if (dict.ContainsKey("mtext"))
        {
          var mText = dict["mtext"];
          if (mText["text"].ToString().Contains("INVERTER #1"))
          {
            mText["text"] = mText["text"]
                .ToString()
                .Replace("INVERTER #1", inverterText);
          }
        }
      }
      return data;
    }

    private static void CreateFilledCircles(Point3d point)
    {
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
      BlockDataMethods.CreateFilledCircleInPaperSpace(center1, radius1);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center2, radius2);
    }

    private List<Dictionary<string, Dictionary<string, object>>> UpdateInverterTextToSlave(
        List<Dictionary<string, Dictionary<string, object>>> data,
        int inverterNumber
    )
    {
      var slaveText = "SLAVE-" + inverterNumber.ToString("D2");
      foreach (var dict in data)
      {
        if (dict.ContainsKey("mtext"))
        {
          var sText = dict["mtext"];
          if (sText["text"].ToString() == "\"SLAVE-01\"")
          {
            sText["text"] = "\"" + slaveText + "\"";
          }
        }
      }
      return data;
    }

    private List<Dictionary<string, Dictionary<string, object>>> UpdateInverterTextToMaster(
        List<Dictionary<string, Dictionary<string, object>>> data,
        int inverterNumber
    )
    {
      var masterText = "MASTER-" + inverterNumber.ToString("D2");
      foreach (var dict in data)
      {
        if (dict.ContainsKey("mtext"))
        {
          var mText = dict["mtext"];
          if (mText["text"].ToString() == "\"SLAVE-01\"")
          {
            mText["text"] = "\"" + masterText + "\"";
          }
        }
      }
      return data;
    }

    private void CreateEMButton(Point3d point, Editor ed)
    {
      string path = "block data/EMButton.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
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
        Dictionary<string, Dictionary<string, object>> formData,
        bool smallStrings = false
    )
    {
      var TEXT_HEIGHT = 0.8334;
      var STRING_HEIGHT = smallStrings ? 0.7648 : 1.0695;
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
            stringData["height"] = (double)stringData["height"] + TEXT_HEIGHT;
            ((List<string>)stringData["items"]).Add("text");
            previousModulesCount = modulesCount;
          }
          else if ((isRegular || isParallel) && modulesCount != previousModulesCount)
          {
            stringData["height"] = (double)stringData["height"] + TEXT_HEIGHT;
            ((List<string>)stringData["items"]).Add("text");
            previousModulesCount = modulesCount;
          }

          if (isRegular)
          {
            stringData["height"] = (double)stringData["height"] + STRING_HEIGHT;
            ((List<string>)stringData["items"]).Add("string");
          }
          else if (isParallel)
          {
            stringData["height"] = (double)stringData["height"] + (STRING_HEIGHT * 2);
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

      var INVERTER_MAXIMUM_HEIGHT = 5.75;
      var INVERTER_MINIMUM_HEIGHT = 4.25;
      var DISTANCE_TO_FIRST_CONDUIT = 3.4556;
      var BUS_BAR_TO_CONDUIT_SPACING = 0.0894;
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

            var numberOfMasters = 0;
            var numberOfSlaves = 0;
            var inverterCount = 0;
            var oldPoint = point;
            var numberOfConduit = 0;
            var oldNumberOfConduits = 0;
            var smallStrings = false;
            var totalHeightOfInverterModule = GetTotalHeightOfStringModules(
                inverterData
            );

            if (totalHeightOfInverterModule > 22.0)
            {
              smallStrings = true;
            }

            foreach (var currentInverterData in inverterData)
            {
              inverterCount += 1;

              if (Convert.ToBoolean(currentInverterData["isMaster"]))
              {
                numberOfMasters++;
                CreateInverter(
                    point,
                    currentInverterData,
                    ed,
                    numberOfMasters,
                    inverterCount
                );
              }
              else
              {
                numberOfSlaves++;
                CreateInverter(
                    point,
                    currentInverterData,
                    ed,
                    numberOfSlaves,
                    inverterCount
                );
              }

              if (inverterCount == 1)
              {
                CreateTopOfBusBar(point, ed);
              }
              else if (inverterCount != inverterData.Count)
              {
                CreateMiddleBusBar(
                    point,
                    ed,
                    oldPoint,
                    oldNumberOfConduits,
                    DISTANCE_TO_FIRST_CONDUIT,
                    BUS_BAR_TO_CONDUIT_SPACING
                );
              }
              else
              {
                CreateEndOfBusBar(point, ed);
              }

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

                var stringDataContainer = GetStringData(dcSolarData, smallStrings);

                oldNumberOfConduits = numberOfConduit;
                numberOfConduit = GetNumberOfConduit(dcSolarData);

                var stringTotalHeight = GetTotalHeightOfStringModule(
                    stringDataContainer
                );

                var lineStartPointX =
                    point.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1)));
                var lineStartPointY = point.Y - 0.6098;

                var stringDefaultMidPointX = lineStartPointX - 3.3606;
                var stringDefaultMidPointY = lineStartPointY - 1.3;

                var topOfStringY = lineStartPointY + 2.9784 + 0.0539;

                var shiftYDown = (
                    topOfStringY - stringDefaultMidPointY - stringTotalHeight / 2
                );

                DC_SOLAR_INPUT.CreateDCSolarObject(
                    dcSolarData,
                    shiftYDown.ToString(),
                    "0",
                    placement,
                    smallStrings
                );

                var adjustedInverterHeight =
                    INVERTER_MAXIMUM_HEIGHT
                    - ((MAXIMUM_CONDUIT - numberOfConduit) * REDUCING_FACTOR);

                oldPoint = point;

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
                point = new Point3d(
                    point.X,
                    point.Y - INVERTER_MINIMUM_HEIGHT,
                    point.Z
                );
              }
            }
          }
        }
      }
    }

    private void CreateEndOfBusBar(Point3d point, Editor ed)
    {
      return;
    }

    private void CreateMiddleBusBar(
        Point3d point,
        Editor ed,
        Point3d oldPoint,
        int oldNumberOfConduits,
        double DISTANCE_TO_FIRST_CONDUIT,
        double BUS_BAR_TO_CONDUIT_SPACING
    )
    {
      var CONDUIT_SPACING = 0.0447;
      string path = "block data/MiddleBusBar.json";
      var data = BlockDataMethods.GetData(path);

      var oldPointY = oldPoint.Y;
      var topOfMiddleBusBarY =
          oldPointY
          - DISTANCE_TO_FIRST_CONDUIT
          - (oldNumberOfConduits - 1) * CONDUIT_SPACING
          - BUS_BAR_TO_CONDUIT_SPACING;

      var topOfArcY = oldPointY - DISTANCE_TO_FIRST_CONDUIT + BUS_BAR_TO_CONDUIT_SPACING;
      var botOfArcY = topOfMiddleBusBarY;

      data = UpdateMiddleBusBarLineHeight(data, topOfMiddleBusBarY);

      BlockDataMethods.CreateObjectGivenData(data, ed, point);

      // Create first filled circle
      Point3d center1 = new Point3d(
          -7.4960687024852319 + point.X,
          -2.8603527581839572 + point.Y,
          0.0 + point.Z
      );
      double radius1 = 0.039236382599967569;
      BlockDataMethods.CreateFilledCircleInPaperSpace(center1, radius1);

      // Create second filled circle
      Point3d center2 = new Point3d(
          -7.6530142328851056 + point.X,
          -2.7971967905063195 + point.Y,
          0.0 + point.Z
      );
      double radius2 = 0.039236382599967569;
      BlockDataMethods.CreateFilledCircleInPaperSpace(center2, radius2);
    }

    private List<Dictionary<string, Dictionary<string, object>>> UpdateMiddleBusBarLineHeight(
        List<Dictionary<string, Dictionary<string, object>>> data,
        double topOfMiddleBusBarY
    )
    {
      var lengths = new List<double> { 3.1049, 6.0505 };
      GetLinesFromDataByLength(data, lengths);
      return data;
    }

    private void GetLinesFromDataByLength(List<Dictionary<string, Dictionary<string, object>>> data, List<double> lengths)
    {
      HelperMethods.SaveDataToJsonFile(data, "middleBusBarData.json");
    }

    private void CreateTopOfBusBar(Point3d point, Editor ed)
    {
      string path = "block data/TopOfBusBar.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);

      // Create first filled circle
      Point3d center1 = new Point3d(
          -7.4960687024852319 + point.X,
          -2.8603527581839572 + point.Y,
          0.0 + point.Z
      );
      double radius1 = 0.039236382599967569;
      BlockDataMethods.CreateFilledCircleInPaperSpace(center1, radius1);

      // Create second filled circle
      Point3d center2 = new Point3d(
          -7.6530142328851056 + point.X,
          -2.7971967905063195 + point.Y,
          0.0 + point.Z
      );
      double radius2 = 0.039236382599967569;
      BlockDataMethods.CreateFilledCircleInPaperSpace(center2, radius2);
    }

    private double GetTotalHeightOfStringModules(List<Dictionary<string, object>> inverterData)
    {
      double totalHeight = 0.0;

      foreach (var currentInverterData in inverterData)
      {
        if (currentInverterData.ContainsKey("DCSolarData"))
        {
          var dcSolarData =
              currentInverterData["DCSolarData"]
              as Dictionary<string, Dictionary<string, object>>;

          var stringDataContainer = GetStringData(dcSolarData);

          var stringTotalHeight = GetTotalHeightOfStringModule(stringDataContainer);

          totalHeight += stringTotalHeight;
        }
      }

      return totalHeight;
    }
  }
}
