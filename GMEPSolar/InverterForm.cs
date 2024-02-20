using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using GMEPUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GMEPSolar
{
  public partial class InverterForm : Form
  {
    private Dictionary<int, Dictionary<string, Dictionary<string, object>>> DCSolarData = new Dictionary<int, Dictionary<string, Dictionary<string, object>>>();
    private DC_SOLAR_INPUT form;
    private int initialID;

    public InverterForm()
    {
      initialID = 1;
      InitializeComponent();
      CreateNewPanelTab("Inverter");
      this.form = new DC_SOLAR_INPUT();
    }

    public void AddUserControlToTab(UserControl control, TabPage tabPage)
    {
      control.Location = new Point(0, 0);
      control.Dock = DockStyle.Fill;

      tabPage.Controls.Add(control);
    }

    public Point3d CreateIntermediatePoint(Point3d startPoint, Point3d endPoint, bool right)
    {
      // Calculate the midpoint
      Point3d midPoint = new Point3d((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2, (startPoint.Z + endPoint.Z) / 2);

      // Create vectors
      Vector3d startToEnd = endPoint - startPoint;
      Vector3d up = new Vector3d(0, 0, 1);

      // Calculate the cross product
      Vector3d crossProduct = startToEnd.CrossProduct(up);

      // If left is true, we want the cross product, otherwise we want the negative of the cross product
      Vector3d direction = right ? crossProduct : -crossProduct;

      // Normalize the direction vector and scale it to desired length (e.g., a fourth the distance between start and end points)
      direction = direction.GetNormal() * startToEnd.Length / 4;

      // Calculate the intermediate point
      Point3d intermediatePoint = midPoint + direction;

      return intermediatePoint;
    }

    public InverterUserControl CreateNewPanelTab(string tabName, bool is2P = true)
    {
      InverterUserControl inverterUserControl = new InverterUserControl(this, is2P);
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

    internal Dictionary<int, Dictionary<string, Dictionary<string, object>>> GetCurrentDCData()
    {
      return DCSolarData;
    }

    internal TabControl GetInverterTabs()
    {
      return INVERTER_TABS;
    }

    internal void RemoveFromDCData(int tabID)
    {
      DCSolarData.Remove(tabID);
    }

    internal void SaveDCData(Dictionary<int, Dictionary<string, Dictionary<string, object>>> newDataObject)
    {
      DCSolarData = newDataObject;
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

    private static void CreateMiddleBusBarFilledCircles(Point3d point)
    {
      Point3d center1 = new Point3d(
                -7.4960687024852319 + point.X,
                -2.8603527581839572 + point.Y,
                0.0 + point.Z
            );
      double radius1 = 0.039236382599967569;
      BlockDataMethods.CreateFilledCircleInPaperSpace(center1, radius1);

      Point3d center2 = new Point3d(
          -7.6530142328851056 + point.X,
          -2.7971967905063195 + point.Y,
          0.0 + point.Z
      );
      double radius2 = 0.039236382599967569;
      BlockDataMethods.CreateFilledCircleInPaperSpace(center2, radius2);
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

    private static List<Dictionary<string, object>> GetStringData(Dictionary<string, Dictionary<string, object>> formData, bool smallStrings = false)
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

    private static double GetTotalHeightOfStringModule(List<Dictionary<string, object>> stringDataContainer)
    {
      double totalHeight = 0;
      foreach (var stringData in stringDataContainer)
      {
        totalHeight += Convert.ToDouble(stringData["height"]);
      }
      return totalHeight;
    }

    private void CREATE_BUTTON_Click(object sender, EventArgs e)
    {
      var inverterFormData = GetInverterFormData();

      var INVERTER_MAXIMUM_HEIGHT = 5.25;
      var INVERTER_MINIMUM_HEIGHT = 4.25;
      var DISTANCE_TO_FIRST_CONDUIT = 3.4482;
      var BUS_BAR_TO_CONDUIT_SPACING = 0.082;
      var REDUCING_FACTOR = 0.081;
      var CONDUIT_SPACING = 0.0405;
      var MAXIMUM_CONDUIT = 16;
      var X_DISTANCE_AWAY = -0.5020;
      var NOTE1PT1_X = 8.3175;
      var COM_CONDUIT_X = 6.7542;

      Close();

      Editor ed;
      PromptPointResult pointResult;
      HelperMethods.GetUserToClick(out ed, out pointResult);

      if (pointResult.Status == PromptStatus.OK)
      {
        var point = pointResult.Value;

        using (DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
        {
          if (inverterFormData.ContainsKey("InverterData"))
          {
            var inverterData = inverterFormData["InverterData"] as List<Dictionary<string, object>>;
            var numberOfInverters = inverterData.Count;
            var numberOfMasters = 0;
            var numberOfSlaves = 0;
            var inverterCount = 0;
            var oldPoint = point;
            var numberOfConduit = 0;
            var smallStrings = false;
            var firstStringContainerWithItems = false;
            var originalPoint = point;
            var absoluteStringPoint = new Point3d(0, 0, 0);
            var absoluteBottomOfString = new Point3d(0, 0, 0);
            var totalHeightOfInverterModule = GetTotalHeightOfStringModules(inverterData);
            var groundWire1stNodeInitialEndpoint = GetGroundWire1stNodeInitialEndpoint(point, ed);
            var groundWire2ndNodeInitialEndpoint = GetGroundWire2ndNodeInitialEndpoint(point, ed);
            var configuration = inverterFormData["Configuration"] as string;

            CreateCombinationPanel(point, inverterData, ed);

            if (configuration == "GRID_LOAD")
            {
              var fakePoint = new Point3d(point.X, point.Y - 7.0, point.Z);
              CreateCombinationPanel(fakePoint, inverterData, ed);
            }

            if (totalHeightOfInverterModule > 22.0)
            {
              smallStrings = true;
            }

            foreach (var currentInverterData in inverterData)
            {
              inverterCount += 1;

              var is2P = Convert.ToBoolean(currentInverterData["is2P"]);

              CreateInitialWiringToCombinationPanel(point, ed, is2P, configuration);

              if (Convert.ToBoolean(currentInverterData["isMaster"]))
              {
                numberOfMasters++;
                CreateInverter(point, currentInverterData, ed, numberOfMasters, inverterCount, originalPoint);
              }
              else
              {
                numberOfSlaves++;
                CreateInverter(point, currentInverterData, ed, numberOfSlaves, inverterCount, originalPoint);
              }

              if (inverterCount == 1)
              {
                CreateTopOfBusBar(point, ed);

                if (inverterCount != inverterData.Count)
                {
                  CreateTopOfBusBarCOMWire(point, ed);
                }

                CreateFirstGroundingWire(point, ed);

                if (inverterCount == inverterData.Count)
                {
                  if (currentInverterData.ContainsKey("DCSolarData"))
                  {
                    var dcSolarData = currentInverterData["DCSolarData"] as Dictionary<string, Dictionary<string, object>>;

                    numberOfConduit = GetNumberOfConduit(dcSolarData);
                  }

                  var topOfArcY = point.Y - DISTANCE_TO_FIRST_CONDUIT + BUS_BAR_TO_CONDUIT_SPACING;
                  var botOfArcY = topOfArcY - (numberOfConduit - 1) * CONDUIT_SPACING - 2 * BUS_BAR_TO_CONDUIT_SPACING;

                  if (numberOfConduit == 0)
                  {
                    botOfArcY = topOfArcY;
                  }
                  else
                  {
                    var pt1 = new Point3d(point.X - NOTE1PT1_X, topOfArcY, 0.0);
                    var pt2 = new Point3d(point.X - NOTE1PT1_X, botOfArcY, 0.0);
                    CreateEllipseBy2Points(pt1, pt2);
                  }

                  var botOfArcYRelativeToNewPoint = botOfArcY - point.Y;

                  CreateEndOfBusBar2(point, ed, botOfArcYRelativeToNewPoint);
                }
              }
              else if (inverterCount != inverterData.Count)
              {
                var oldPointY = oldPoint.Y;
                var topOfArcY = oldPointY - DISTANCE_TO_FIRST_CONDUIT + BUS_BAR_TO_CONDUIT_SPACING;
                var botOfArcY = topOfArcY - (numberOfConduit - 1) * CONDUIT_SPACING - 2 * BUS_BAR_TO_CONDUIT_SPACING;

                groundWire2ndNodeInitialEndpoint = GetGroundWire2ndNodeInitialEndpoint(point, ed);

                CreateGround2ndNodeInitialWire(point, ed);

                CreateGround1stNodeInitialWire(point, ed);

                CreateGroundConnectingWire(groundWire1stNodeInitialEndpoint, groundWire2ndNodeInitialEndpoint, ed);

                groundWire1stNodeInitialEndpoint = GetGroundWire1stNodeInitialEndpoint(point, ed);

                if (numberOfConduit == 0)
                {
                  botOfArcY = topOfArcY;
                }
                else
                {
                  var pt1 = new Point3d(point.X - COM_CONDUIT_X, topOfArcY, 0.0);
                  var pt2 = new Point3d(point.X - COM_CONDUIT_X, botOfArcY, 0.0);
                  CreateArcBy2Points(pt1, pt2, true);

                  pt1 = new Point3d(point.X - NOTE1PT1_X, topOfArcY, 0.0);
                  pt2 = new Point3d(point.X - NOTE1PT1_X, botOfArcY, 0.0);
                  CreateEllipseBy2Points(pt1, pt2);
                }

                var botOfArcYRelativeToNewPoint = botOfArcY - point.Y;

                CreateMiddleBusBar(point, ed, botOfArcYRelativeToNewPoint);
              }
              else
              {
                var oldPointY = oldPoint.Y;
                var topOfArcY = oldPointY - DISTANCE_TO_FIRST_CONDUIT + BUS_BAR_TO_CONDUIT_SPACING;
                var botOfArcY = topOfArcY - (numberOfConduit - 1) * CONDUIT_SPACING - 2 * BUS_BAR_TO_CONDUIT_SPACING;

                groundWire2ndNodeInitialEndpoint = GetGroundWire2ndNodeInitialEndpoint(point, ed);

                CreateGround2ndNodeInitialWire(point, ed);

                CreateGroundConnectingWire(groundWire1stNodeInitialEndpoint, groundWire2ndNodeInitialEndpoint, ed);

                groundWire1stNodeInitialEndpoint = GetGroundWire1stNodeInitialEndpoint(point, ed);

                if (numberOfConduit == 0)
                {
                  botOfArcY = topOfArcY;
                }
                else
                {
                  var pt1 = new Point3d(point.X - COM_CONDUIT_X, topOfArcY, 0.0);
                  var pt2 = new Point3d(point.X - COM_CONDUIT_X, botOfArcY, 0.0);
                  CreateArcBy2Points(pt1, pt2, true);

                  pt1 = new Point3d(point.X - NOTE1PT1_X, topOfArcY, 0.0);
                  pt2 = new Point3d(point.X - NOTE1PT1_X, botOfArcY, 0.0);
                  CreateEllipseBy2Points(pt1, pt2);
                }

                var botOfArcYRelativeToNewPoint = botOfArcY - point.Y;

                CreateEndOfBusBar(point, ed, botOfArcYRelativeToNewPoint);

                if (currentInverterData.ContainsKey("DCSolarData"))
                {
                  var dcSolarData = currentInverterData["DCSolarData"] as Dictionary<string, Dictionary<string, object>>;

                  numberOfConduit = GetNumberOfConduit(dcSolarData);
                }
                else
                {
                  numberOfConduit = 0;
                }

                topOfArcY = point.Y - DISTANCE_TO_FIRST_CONDUIT + BUS_BAR_TO_CONDUIT_SPACING;
                botOfArcY = topOfArcY - (numberOfConduit - 1) * CONDUIT_SPACING - 2 * BUS_BAR_TO_CONDUIT_SPACING;

                if (numberOfConduit == 0)
                {
                  botOfArcY = topOfArcY;
                }
                else
                {
                  var pt1 = new Point3d(point.X - NOTE1PT1_X, topOfArcY, 0.0);
                  var pt2 = new Point3d(point.X - NOTE1PT1_X, botOfArcY, 0.0);
                  CreateEllipseBy2Points(pt1, pt2);
                }

                botOfArcYRelativeToNewPoint = botOfArcY - point.Y;

                CreateEndOfBusBar2(point, ed, botOfArcYRelativeToNewPoint);
              }

              if (currentInverterData.ContainsKey("DCSolarData"))
              {
                var dcSolarData = currentInverterData["DCSolarData"] as Dictionary<string, Dictionary<string, object>>;

                numberOfConduit = GetNumberOfConduit(dcSolarData);

                if (numberOfConduit != 0)
                {
                  CreateNote1pt1(point, ed);
                }

                var numberOfMPPTs = GetNumberOfMPPTs(dcSolarData);
                string json;

                if (numberOfMPPTs != 0)
                {
                  json = BlockDataMethods.GetUnparsedJSONData($"point data/DCSolar{numberOfMPPTs}PlacementPoint.json");
                }
                else
                {
                  return;
                }

                var parsedData = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);

                var placement = new Point3d(
                    parsedData["X"] + point.X,
                    parsedData["Y"] + point.Y,
                    parsedData["Z"] + point.Z
                );

                var stringDataContainer = GetStringData(dcSolarData, smallStrings);

                var stringDataContainerHasItems = StringDataContainerHasItems(stringDataContainer);

                var stringTotalHeight = GetTotalHeightOfStringModule(stringDataContainer);

                if (!firstStringContainerWithItems && stringDataContainerHasItems)
                {
                  firstStringContainerWithItems = true;

                  absoluteStringPoint = GetAbsoluteStringPoint(point);
                }

                if (stringDataContainerHasItems)
                {
                  var adjustedStringTotalHeight = stringTotalHeight - 0.8134999;
                  absoluteBottomOfString = new Point3d(point.X, point.Y - adjustedStringTotalHeight, point.Z);
                }

                var lineStartPointX = point.X - (0.5673 + (0.6484 * (numberOfMPPTs - 1)));
                var lineStartPointY = point.Y - 0.6098;

                var stringDefaultMidPointX = lineStartPointX - 3.3606;
                var stringDefaultMidPointY = lineStartPointY - 1.3;

                var topOfStringY = lineStartPointY + 2.9784 + 0.0539;

                var shiftYDown = (topOfStringY - stringDefaultMidPointY - stringTotalHeight / 2);

                DC_SOLAR_INPUT.CreateDCSolarObject(dcSolarData, shiftYDown.ToString(), X_DISTANCE_AWAY.ToString(), placement, smallStrings);

                var adjustedInverterHeight = INVERTER_MAXIMUM_HEIGHT - ((MAXIMUM_CONDUIT - numberOfConduit) * REDUCING_FACTOR);

                oldPoint = point;

                if (stringTotalHeight > adjustedInverterHeight)
                {
                  if (inverterCount != inverterData.Count)
                  {
                    point = new Point3d(
                      point.X,
                      point.Y - (stringTotalHeight),
                      point.Z
                  );
                  }
                }
                else
                {
                  if (inverterCount != inverterData.Count)
                  {
                    point = new Point3d(
                      point.X,
                      point.Y - (adjustedInverterHeight),
                      point.Z
                  );
                  }
                }
              }
              else
              {
                numberOfConduit = 0;
                oldPoint = point;

                if (inverterCount != inverterData.Count)
                {
                  point = new Point3d(
                    point.X,
                    point.Y - INVERTER_MINIMUM_HEIGHT,
                    point.Z
                );
                }
              }

              if (inverterCount == inverterData.Count)
              {
                if (absoluteStringPoint != new Point3d(0, 0, 0))
                {
                  CreateGround1stNodeInitialWire(point, ed);
                  CreateGroundWireBottomToTopOfString(point, absoluteStringPoint, absoluteBottomOfString, ed);
                }
              }
            }
          }
        }
      }
    }

    private void CreateTopOfBusBarCOMWire(Point3d point, Editor ed)
    {
      string path = "block data/TopOfBusBarCOM.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
    }

    private void CreateInitialWiringToCombinationPanel(Point3d point, Editor ed, bool is2P, string configuration)
    {
      if (configuration == "GRID")
      {
        if (is2P)
        {
          string path = "block data/OnlyGridInitial2P.json";
          var data = BlockDataMethods.GetData(path);
          BlockDataMethods.CreateObjectGivenData(data, ed, point);
        }
        else
        {
          string path = "block data/OnlyGridInitial3P.json";
          var data = BlockDataMethods.GetData(path);
          BlockDataMethods.CreateObjectGivenData(data, ed, point);
        }
      }
      else if (configuration == "LOAD")
      {
        if (is2P)
        {
          string path = "block data/OnlyLoadInitial2P.json";
          var data = BlockDataMethods.GetData(path);
          BlockDataMethods.CreateObjectGivenData(data, ed, point);
        }
        else
        {
          string path = "block data/OnlyLoadInitial3P.json";
          var data = BlockDataMethods.GetData(path);
          BlockDataMethods.CreateObjectGivenData(data, ed, point);
        }
      }
      else if (configuration == "GRID_LOAD")
      {
        if (is2P)
        {
          string path = "block data/GridLoadInitial2P.json";
          var data = BlockDataMethods.GetData(path);
          BlockDataMethods.CreateObjectGivenData(data, ed, point);
        }
        else
        {
          string path = "block data/GridLoadInitial3P.json";
          var data = BlockDataMethods.GetData(path);
          BlockDataMethods.CreateObjectGivenData(data, ed, point);
        }
      }
    }

    private void CreateCombinationPanel(Point3d point, List<Dictionary<string, object>> inverterData, Editor ed)
    {
      var BREAKER_HEIGHT = 0.3964;

      var path = "block data/CombinationPanelUpperWire.json";
      var upperWireData = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(upperWireData, ed, point);

      for (var i = 0; i < inverterData.Count; i++)
      {
        var currentInverterData = inverterData[i];
        var is2P = Convert.ToBoolean(currentInverterData["is2P"]);
        var adjustedPoint = new Point3d(point.X, point.Y - (BREAKER_HEIGHT * i), point.Z);
        CreateBreaker(adjustedPoint, is2P, ed);
      }

      path = "block data/CombinationPanelRectangle.json";
      var rectangleData = BlockDataMethods.GetData(path);
      rectangleData = IncreaseHeightOfCombinationPanel(rectangleData, inverterData.Count, BREAKER_HEIGHT);
      BlockDataMethods.CreateObjectGivenData(rectangleData, ed, point);

      path = "block data/CombinationPanelBottomText.json";
      var bottomTextData = BlockDataMethods.GetData(path);
      bottomTextData = AdjustPlacementOfBottomTextOnCombinationPanel(bottomTextData, inverterData.Count, BREAKER_HEIGHT);
      BlockDataMethods.CreateObjectGivenData(bottomTextData, ed, point);
    }

    private List<Dictionary<string, Dictionary<string, object>>> AdjustPlacementOfBottomTextOnCombinationPanel(List<Dictionary<string, Dictionary<string, object>>> bottomTextData, int numberOfInverters, double BREAKER_HEIGHT)
    {
      foreach (var text in bottomTextData)
      {
        var textData = JObject.FromObject(text["mtext"]).ToObject<MText>();
        textData.location = new Vertex(textData.location.x, textData.location.y - (BREAKER_HEIGHT * (numberOfInverters - 1)), textData.location.z);
        text["mtext"] = JObject.FromObject(textData).ToObject<Dictionary<string, object>>();
      }
      return bottomTextData;
    }

    private List<Dictionary<string, Dictionary<string, object>>> IncreaseHeightOfCombinationPanel(List<Dictionary<string, Dictionary<string, object>>> rectangleData, int numberOfInverters, double BREAKER_HEIGHT)
    {
      foreach (var rectangle in rectangleData)
      {
        if (rectangle.ContainsKey("polyline"))
        {
          var polyline = JObject.FromObject(rectangle["polyline"]).ToObject<Polyline>();
          var newVertices = new List<Vertex>();

          foreach (var vertex in polyline.vertices)
          {
            if (vertex.y < -4.4)
            {
              newVertices.Add(new Vertex(vertex.x, vertex.y - (BREAKER_HEIGHT * (numberOfInverters - 1)), vertex.z));
            }
            else
            {
              newVertices.Add(new Vertex(vertex.x, vertex.y, vertex.z));
            }
          }
          polyline.vertices = newVertices;
          rectangle["polyline"] = JObject.FromObject(polyline).ToObject<Dictionary<string, object>>();
        }
      }
      return rectangleData;
    }

    private static void CreateBreaker(Point3d point, bool is2P, Editor ed)
    {
      string path = "block data/CombinationPanelBreaker.json";
      var breakerData = BlockDataMethods.GetData(path);

      if (is2P)
      {
        breakerData[0]["mtext"]["text"] = "200A/2P";
      }

      BlockDataMethods.CreateObjectGivenData(breakerData, ed, point);
    }

    private void CreateGroundWireBottomToTopOfString(Point3d point, Point3d absoluteStringPoint, Point3d absoluteBottomOfString, Editor ed)
    {
      var DISTANCE_BELOW_BOTTOM = 0.2;

      var path = "point data/BottomOfBatteryPoint.json";
      var json = BlockDataMethods.GetUnparsedJSONData(path);
      var bottomBatteryPoint = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
      var absoluteBottomBatteryPoint = new Point3d(bottomBatteryPoint["X"] + point.X, bottomBatteryPoint["Y"] + point.Y, bottomBatteryPoint["Z"] + point.Z);
      var smallestAbsoluteY = Math.Min(absoluteBottomBatteryPoint.Y, absoluteBottomOfString.Y);

      path = "point data/GroundWire1stNodeInitialEndpoint.json";
      json = BlockDataMethods.GetUnparsedJSONData(path);

      var groundWirePoint1Relative = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
      var groundWirePoint2Absolute = new Point3d(groundWirePoint1Relative["X"] + point.X, smallestAbsoluteY - DISTANCE_BELOW_BOTTOM, groundWirePoint1Relative["Z"] + point.Z);
      var groundWirePoint1Absolute = new Point3d(groundWirePoint1Relative["X"] + point.X, groundWirePoint1Relative["Y"] + point.Y, groundWirePoint1Relative["Z"] + point.Z);

      BlockDataMethods.CreateHiddenLineInPaperspace(groundWirePoint1Absolute, groundWirePoint2Absolute, ed);

      var groundWirePoint3Absolute = new Point3d(absoluteStringPoint.X, groundWirePoint2Absolute.Y, groundWirePoint2Absolute.Z);

      BlockDataMethods.CreateHiddenLineInPaperspace(groundWirePoint2Absolute, groundWirePoint3Absolute, ed);

      var groundWirePoint4Absolute = new Point3d(absoluteStringPoint.X, absoluteStringPoint.Y, absoluteStringPoint.Z);

      BlockDataMethods.CreateHiddenLineInPaperspace(groundWirePoint3Absolute, groundWirePoint4Absolute, ed);
    }

    private Point3d GetAbsoluteStringPoint(Point3d point)
    {
      string path = "point data/FirstStringGroundPoint.json";
      var json = BlockDataMethods.GetUnparsedJSONData(path);
      var parsedData = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
      var absoluteStringPoint = new Point3d(parsedData["X"] + point.X, parsedData["Y"] + point.Y, parsedData["Z"] + point.Z);
      return absoluteStringPoint;
    }

    private bool StringDataContainerHasItems(List<Dictionary<string, object>> stringDataContainer)
    {
      foreach (var stringData in stringDataContainer)
      {
        if (stringData["items"] != null)
        {
          return true;
        }
      }

      return false;
    }

    private void CreateGroundConnectingWire(Dictionary<string, double> groundWire1stNodeInitialEndpoint, Dictionary<string, double> groundWire2ndNodeInitialEndpoint, Editor ed)
    {
      var startPoint = new Point3d(groundWire1stNodeInitialEndpoint["X"], groundWire1stNodeInitialEndpoint["Y"], groundWire1stNodeInitialEndpoint["Z"]);
      var endPoint = new Point3d(groundWire2ndNodeInitialEndpoint["X"], groundWire2ndNodeInitialEndpoint["Y"], groundWire2ndNodeInitialEndpoint["Z"]);

      BlockDataMethods.CreateHiddenLineInPaperspace(startPoint, endPoint, ed);
    }

    private Dictionary<string, double> GetGroundWire2ndNodeInitialEndpoint(Point3d point, Editor ed)
    {
      string path = "point data/GroundWire2ndNodeInitialEndpoint.json";
      var json = BlockDataMethods.GetUnparsedJSONData(path);
      var parsedData = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
      var absoluteData = new Dictionary<string, double>
      {
          { "X", parsedData["X"] + point.X },
          { "Y", parsedData["Y"] + point.Y },
          { "Z", parsedData["Z"] + point.Z }
      };
      return absoluteData;
    }

    private Dictionary<string, double> GetGroundWire1stNodeInitialEndpoint(Point3d point, Editor ed)
    {
      string path = "point data/GroundWire1stNodeInitialEndpoint.json";
      var json = BlockDataMethods.GetUnparsedJSONData(path);
      var parsedData = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
      var absoluteData = new Dictionary<string, double>
      {
          { "X", parsedData["X"] + point.X },
          { "Y", parsedData["Y"] + point.Y },
          { "Z", parsedData["Z"] + point.Z }
      };
      return absoluteData;
    }

    private void CreateGround2ndNodeInitialWire(Point3d point, Editor ed)
    {
      string path = "block data/GroundWire2ndNodeInitial.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
    }

    private void CreateGround1stNodeInitialWire(Point3d point, Editor ed)
    {
      string path = "block data/GroundWire1stNodeInitial.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
    }

    private void CreateFirstGroundingWire(Point3d point, Editor ed)
    {
      string path = "block data/GroundWire1stNode1stInverter.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
    }

    private void CreateArcBy2Points(Point3d startPoint, Point3d endPoint, bool right)
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Database db = doc.Database;

      using (Transaction trans = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);

        var intermediatePoint = CreateIntermediatePoint(startPoint, endPoint, right);
        var pts = new Point3dCollection { startPoint, intermediatePoint, endPoint };
        CircularArc3d arc3d = new CircularArc3d(startPoint, intermediatePoint, endPoint);

        Point3d centerPoint = arc3d.Center;

        double startAngle = Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
        double endAngle = Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);
        if (!right)
        {
          startAngle = Math.Atan2(endPoint.Y - centerPoint.Y, endPoint.X - centerPoint.X);
          endAngle = Math.Atan2(startPoint.Y - centerPoint.Y, startPoint.X - centerPoint.X);
        }

        Arc arc = new Arc(arc3d.Center, arc3d.Radius, startAngle, endAngle);

        btr.AppendEntity(arc);
        trans.AddNewlyCreatedDBObject(arc, true);

        trans.Commit();
      }
    }

    private void CreateEllipseBy2Points(Point3d startPoint, Point3d endPoint)
    {
      Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      Database db = doc.Database;

      using (Transaction trans = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);

        Vector3d majorAxis = (endPoint - startPoint) / 2;
        double majorRadius = majorAxis.Length;
        double minorRadius = majorRadius / 5;

        var centerPt = new Point3d((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2, (startPoint.Z + endPoint.Z) / 2);
        Ellipse ellipse = new Ellipse(centerPt, Vector3d.ZAxis, majorAxis, minorRadius / majorRadius, 0, 2 * Math.PI);

        if (!BlockDataMethods.LayerExists("E-TEXT"))
        {
          BlockDataMethods.CreateLayer("E-TEXT", 2);
        }

        ellipse.Layer = "E-TEXT";

        btr.AppendEntity(ellipse);
        trans.AddNewlyCreatedDBObject(ellipse, true);

        trans.Commit();
      }
    }

    private void CreateEMButton(Point3d point, Editor ed, int inverterCount)
    {
      string path = "block data/EMButton.json";

      if (inverterCount != 1)
      {
        path = "block data/EMButton2.json";
      }

      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
    }

    private void CreateEndBusBar2FilledCircles(Point3d point)
    {
      double radius = 0.039236382599967569;

      Point3d center1 = new Point3d(
          -7.4960687024852319 + point.X,
          -4.8608048270942366 + point.Y,
          0.0 + point.Z
      );
      Point3d center2 = new Point3d(
          -7.6530142328851056 + point.X,
          -5.2579733814152236 + point.Y,
          0.0 + point.Z
      );
      Point3d center3 = new Point3d(
          -5.7042862323706061 + point.X,
          -4.8608048270942348 + point.Y,
          0.0 + point.Z
      );
      Point3d center4 = new Point3d(
          -5.8778454256876387 + point.X,
          -5.2579733814152227 + point.Y,
          0.0 + point.Z
      );
      Point3d center5 = new Point3d(
          -5.8778454256876387 + point.X,
          -5.6202406847764479 + point.Y,
          0.0 + point.Z
      );
      Point3d center6 = new Point3d(
          -5.7042862323706061 + point.X,
          -5.6202406847764479 + point.Y,
          0.0 + point.Z
      );
      Point3d center7 = new Point3d(
          -5.8778454256876387 + point.X,
          -5.6202406847764479 + point.Y,
          0.0 + point.Z
      );

      BlockDataMethods.CreateFilledCircleInPaperSpace(center1, radius);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center2, radius);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center3, radius);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center4, radius);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center5, radius);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center6, radius);
      BlockDataMethods.CreateFilledCircleInPaperSpace(center7, radius);
    }

    private void CreateEndOfBusBar(Point3d point, Editor ed, double botOfArcYRelativeToNewPoint)
    {
      string path = "block data/EndBusBar1.json";
      var data = BlockDataMethods.GetData(path);

      data = UpdateEndBusBarLineHeight(data, botOfArcYRelativeToNewPoint);

      BlockDataMethods.CreateObjectGivenData(data, ed, point);
      CreateMiddleBusBarFilledCircles(point);
    }

    private void CreateEndOfBusBar2(Point3d point, Editor ed, double botOfArcYRelativeToNewPoint)
    {
      string path = "block data/EndBusBar2.json";
      var data = BlockDataMethods.GetData(path);

      data = UpdateEndBusBar2LineHeight(data, botOfArcYRelativeToNewPoint);

      BlockDataMethods.CreateObjectGivenData(data, ed, point);
      CreateEndBusBar2FilledCircles(point);
    }

    private void CreateInverter(Point3d point, Dictionary<string, object> inverterData, Editor ed, int slaveOrMasterNumber, int inverterCount, Point3d originalPoint)
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
        CreateEMButton(originalPoint, ed, inverterCount);
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

    private void CreateMiddleBusBar(Point3d point, Editor ed, double botOfArcYRelativeToNewPoint)
    {
      string path = "block data/MiddleBusBar.json";
      var data = BlockDataMethods.GetData(path);

      data = UpdateMiddleBusBarLineHeight(data, botOfArcYRelativeToNewPoint);

      BlockDataMethods.CreateObjectGivenData(data, ed, point);
      CreateMiddleBusBarFilledCircles(point);
    }

    private void CreateNote1pt1(Point3d point, Editor ed)
    {
      string path = "block data/Note1.1.json";
      var data = BlockDataMethods.GetData(path);
      BlockDataMethods.CreateObjectGivenData(data, ed, point);
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

    private List<Dictionary<string, Dictionary<string, object>>> GetLinesFromDataByLengthAndUpdateUpperPointYCoord(List<Dictionary<string, Dictionary<string, object>>> data, List<double> lengths, double newUpperYCoord)
    {
      foreach (var dict in data)
      {
        if (dict.ContainsKey("line"))
        {
          var line = dict["line"];

          var startPoint = ((JObject)line["startPoint"]).ToObject<Dictionary<string, object>>();
          var endPoint = ((JObject)line["endPoint"]).ToObject<Dictionary<string, object>>();

          var startPointX = Convert.ToDouble(startPoint["x"]);
          var startPointY = Convert.ToDouble(startPoint["y"]);
          var endPointX = Convert.ToDouble(endPoint["x"]);
          var endPointY = Convert.ToDouble(endPoint["y"]);

          var length = Math.Sqrt(Math.Pow(endPointX - startPointX, 2) + Math.Pow(endPointY - startPointY, 2));

          length = Math.Round(length, 4);

          if (lengths.Contains(length))
          {
            var adjustmentPoint = (startPointY > endPointY) ? startPoint : endPoint;
            var startOrEnd = (startPointY > endPointY) ? "start" : "end";
            adjustmentPoint["y"] = newUpperYCoord;
            JObject adjustmentPointJObject = JObject.FromObject(adjustmentPoint);

            if (startOrEnd == "start")
            {
              line["startPoint"] = adjustmentPointJObject;
            }
            else
            {
              line["endPoint"] = adjustmentPointJObject;
            }
          }
        }
      }
      return data;
    }

    private int GetNumberOfConduit(Dictionary<string, Dictionary<string, object>> dcSolarData)
    {
      int numberOfConduit = 0;
      foreach (var mpptData in dcSolarData.Values)
      {
        if (Convert.ToBoolean(mpptData["Enabled"]))
        {
          if (Convert.ToBoolean(mpptData["Regular"]))
          {
            numberOfConduit += 1;
          }
          else if (Convert.ToBoolean(mpptData["Parallel"]))
          {
            numberOfConduit += 2;
          }
        }
      }
      return numberOfConduit * 2;
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

    private List<Dictionary<string, Dictionary<string, object>>> UpdateEndBusBar2LineHeight(List<Dictionary<string, Dictionary<string, object>>> data, double topOfMiddleBusBarY)
    {
      var lengths = new List<double> { 0.8440, 1.2412 };
      data = GetLinesFromDataByLengthAndUpdateUpperPointYCoord(data, lengths, topOfMiddleBusBarY); return data;
    }

    private List<Dictionary<string, Dictionary<string, object>>> UpdateEndBusBarLineHeight(List<Dictionary<string, Dictionary<string, object>>> data, double topOfMiddleBusBarY)
    {
      var lengths = new List<double> { 3.1123, 6.0578 };
      data = GetLinesFromDataByLengthAndUpdateUpperPointYCoord(data, lengths, topOfMiddleBusBarY); return data;
    }

    private List<Dictionary<string, Dictionary<string, object>>> UpdateInverterTextToMaster(List<Dictionary<string, Dictionary<string, object>>> data, int inverterNumber)
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

    private List<Dictionary<string, Dictionary<string, object>>> UpdateInverterTextToSlave(List<Dictionary<string, Dictionary<string, object>>> data, int inverterNumber)
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

    private List<Dictionary<string, Dictionary<string, object>>> UpdateInverterTextWithInverterNumber(List<Dictionary<string, Dictionary<string, object>>> data, int inverterCount)
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

    private List<Dictionary<string, Dictionary<string, object>>> UpdateMiddleBusBarLineHeight(List<Dictionary<string, Dictionary<string, object>>> data, double topOfMiddleBusBarY)
    {
      var lengths = new List<double> { 3.1049, 6.0505 };
      data = GetLinesFromDataByLengthAndUpdateUpperPointYCoord(data, lengths, topOfMiddleBusBarY); return data;
    }
  }

  public class Vertex
  {
    public Vertex(double x, double y, double z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
  }

  public class Polyline
  {
    public string layer { get; set; }
    public List<Vertex> vertices { get; set; }
    public string linetype { get; set; }
    public bool isClosed { get; set; }
  }

  public class MText
  {
    public string layer { get; set; }
    public string style { get; set; }
    public string justification { get; set; }
    public string text { get; set; }
    public double height { get; set; }
    public double lineSpaceDistance { get; set; }
    public Vertex location { get; set; }
  }
}