namespace YALV.Samples
{
  using System;
  using System.Windows;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Random r = new Random();

      for (int i = 0; i < 10000; i++)
      {
        int value = r.Next(13);

        switch (value)
        {
          case 0:
          case 1:
          case 2:
          case 3:
            this.method1();
            break;

          case 4:
          case 5:
          case 6:
          case 7:
          case 8:
            this.method2();
            break;

          case 9:
            this.method3();
            break;

          case 10:
          case 11:
            this.method4();
            break;

          case 12:
            this.method5();
            break;
        }
      }

      MessageBox.Show("Generation Complete!", "YALV! Samples", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void method1()
    {
      LogService.Trace.Debug("This is a debug message");
    }

    private void method2()
    {
        LogService.Trace.Info("Lorem ipsum dolor sit amet, consectetur adipiscing elit. In quis nisi non ipsum vulputate viverra non a felis. Suspendisse potenti. Maecenas lorem urna, imperdiet in ultricies vulputate, pulvinar ac erat. Phasellus magna urna, dictum vel neque sed, mattis condimentum erat. Suspendisse tristique tincidunt justo, id sagittis orci faucibus at. Vestibulum vitae pharetra ipsum. Phasellus at orci eu urna consectetur consectetur. Suspendisse diam velit, tristique vitae mi molestie, cursus fringilla arcu. Donec porttitor erat mi, sit amet fringilla libero sollicitudin eget. Curabitur varius elit vitae hendrerit lobortis. Suspendisse potenti.");
    }

    private void method3()
    {
      LogService.Trace.Warn("This is a warning message!", new Exception("Warning Exception!"));
    }

    private void method4()
    {
      LogService.Trace.Error("This is an error message!", new Exception("Warning Exception!"));
    }

    private void method5()
    {
      LogService.Trace.Fatal("Ut tincidunt, dolor ut egestas porttitor, mauris metus volutpat ante, tristique ornare metus tortor non velit. Ut egestas ultricies ipsum, quis semper sem blandit eu. " +
                             "Pellentesque at elementum nisi. Nulla dignissim nisi enim, et porta dolor faucibus id. Praesent et justo nec felis accumsan gravida et eu mauris. " +
                             "Sed turpis urna, sagittis vel est et, molestie viverra sapien. Ut sed ultricies quam. Cras ultricies mi sem, et pharetra orci sodales in. Vivamus vulputate " +
                             "aliquam lacus, sit amet commodo ipsum accumsan eget. Etiam dictum accumsan turpis, ac pellentesque velit aliquet id. Praesent velit sapien, lacinia nec magna et, commodo semper sem. Nunc lobortis diam non semper porta" +
                             "Maecenas lectus purus, molestie quis pellentesque sed, sollicitudin at ligula. Curabitur quis nisl dui. In feugiat luctus tincidunt. Nam porta commodo lectus vel commodo. Aenean lobortis leo libero, vitae facilisis " +
                             "nisi facilisis sit amet. Ut condimentum viverra augue, eu accumsan libero rutrum eget. Nunc aliquam sem vitae enim dapibus pretium. Duis gravida tortor elit, quis ornare tortor sodales quis. Proin sit amet lacinia nisl, " +
                             "at ullamcorper erat. Cras id risus in nisi ornare auctor. Proin condimentum libero vitae vulputate sollicitudin. Fusce justo odio, dapibus nec elementum quis, rutrum eleifend turpis.", new Exception("Warning Exception!"));
    }
  }
}
