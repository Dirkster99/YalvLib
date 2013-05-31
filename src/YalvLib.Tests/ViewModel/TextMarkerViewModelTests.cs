using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using YalvLib.Model;
using YalvLib.ViewModel;

namespace YalvLib.Tests.ViewModel
{

    [TestFixture, RequiresSTA]
    public class TextMarkerViewModelTests
    {

        [Test]
        public void SetAuthorInViewModel()
        {
            YalvRegistry.Instance.SetActualLogAnalysisWorkspace(new LogAnalysisWorkspace());
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), "Toto", "Hello World");
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);
            viewModel.Author = "Titi";
            viewModel.Message = "Hallo World";
            viewModel.CommandChangeTextMarker.Execute(null);
            Assert.AreEqual("Titi", textMarker.Author);
            Assert.AreEqual("Hallo World", textMarker.Message);
        }

        [Test]
        public void CanExecuteChangeTextMarker_1()
        {
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), "", "");
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);
            Assert.IsFalse(viewModel.CommandChangeTextMarker.CanExecute(null));
        }

        [Test]
        public void CanExecuteChangeTextMarker_2()
        {
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), null, null);
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);
            Assert.IsFalse(viewModel.CommandChangeTextMarker.CanExecute(null));
        }

        [Test]
        public void CanExecuteChangeTextMarker_3()
        {
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), "", "Hello World");
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);
            Assert.IsFalse(viewModel.CommandChangeTextMarker.CanExecute(null));
        }

        [Test]
        public void CanExecuteChangeTextMarker_4()
        {
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), "Toto", "");
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);
            Assert.IsFalse(viewModel.CommandChangeTextMarker.CanExecute(null));
        }

        [Test]
        public void TestNotificationsAuthor()
        {
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), "Toto", "Hello World");
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);

            PropertyChangedEventHandler delegateAuthor = (senderAuthor, e) => Assert.AreEqual("Author", e.PropertyName);
            viewModel.PropertyChanged += delegateAuthor;
            viewModel.Author = "plop";
        }

        [Test]
        public void TestNotificationsMessage()
        {
            TextMarker textMarker = new TextMarker(new List<LogEntry>(), "Toto", "Hello World");
            TextMarkerViewModel viewModel = new TextMarkerViewModel(textMarker);

            PropertyChangedEventHandler delegateMessage = (senderMess, a) => Assert.AreEqual("Message", a.PropertyName);
            viewModel.PropertyChanged += delegateMessage;
            viewModel.Message = "Et maintenant on va fourrer la dinde quoi !";
        }
    }

}
