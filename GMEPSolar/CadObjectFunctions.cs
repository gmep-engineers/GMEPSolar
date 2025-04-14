using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace GMEPSolar
{
  public class CadObjectFunctions
  {
    public CadObjectFunctions() { }

    public static string GetProjectNameFromFileName()
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Core
        .Application
        .DocumentManager
        .MdiActiveDocument;
      return Path.GetFileName(doc.Name).Replace(".dwg", "").Replace(".DWG", "");
    }
  }
}
