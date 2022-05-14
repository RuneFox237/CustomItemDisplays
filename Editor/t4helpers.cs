using System.Globalization;
using UnityEngine;

namespace RuneFoxMods.TemplateHelpers
{
  public class Helpers
  {
    public static string PrintVector3(Vector3 vec)
    {
      return "new Vector3(" + PrintFloat(vec.x) + ", " + PrintFloat(vec.y) + ", " + PrintFloat(vec.z) + ")";
    }

    public static string PrintFloat(float f)
    {
      return f.ToString("G", CultureInfo.InvariantCulture) + "f";
    }
  }

}