using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Common;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    public class DisplayTextMarkersViewModel : BindableObject
    {

        private readonly List<TextMarkerViewModel> _textMarkerVmList;

        public DisplayTextMarkersViewModel()
        {
            _textMarkerVmList = new List<TextMarkerViewModel>();
        }

        public void GenerateViewModels(List<TextMarker> tm)
        {
            foreach (TextMarker textMarker in tm)
            {
                _textMarkerVmList.Add(new TextMarkerViewModel(textMarker));
            }
            RaisePropertyChanged("TextMarkerViewModels");
        }

        public List<TextMarkerViewModel> TextMarkerViewModels
        {
            get { return _textMarkerVmList; }
        }


    }
}
