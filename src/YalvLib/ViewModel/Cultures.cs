﻿namespace YalvLib.ViewModel
{
  public static class ResolveCultures
  {
    public static System.Globalization.CultureInfo ResolvedCulture
    {
      get { return System.Globalization.CultureInfo.GetCultureInfo(YalvLib.Strings.Resources.CultureName); }
    }
  }
}