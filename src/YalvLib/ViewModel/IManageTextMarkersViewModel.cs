using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YalvLib.ViewModel
{
    public interface IManageTextMarkersViewModel
    {

        event EventHandler MarkerDeleted;
        event EventHandler MarkerAdded;
    }
}
