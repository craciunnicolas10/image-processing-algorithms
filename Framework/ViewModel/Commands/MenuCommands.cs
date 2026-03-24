using Emgu.CV;
using Emgu.CV.Structure;

using System.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using Framework.View;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.DrawingHelper;
using static Framework.Utilities.FileHelper;
using static Framework.Converters.ImageConverter;

using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;

        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayscaleImageCommand;
        public RelayCommand LoadGrayscaleImageCommand
        {
            get
            {
                if (_loadGrayscaleImageCommand == null)
                    _loadGrayscaleImageCommand = new RelayCommand(LoadGrayscaleImage);
                return _loadGrayscaleImageCommand;
            }
        }

        private void LoadGrayscaleImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a grayscale picture");
            if (fileName != null)
            {
                GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = Convert(GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = Convert(ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                GrayProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                ColorProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                Process.Start(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            RemoveUiElements(canvases[0] as Canvas);
            RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != Application.Current.MainWindow)
                {
                    window.Close();
                }
            }

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools
        #region Mirror image
        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Mirror(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Mirror(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate Clockwise
        private ICommand _rotateClockwiseCommand;
        public ICommand RotateClockwiseCommand
        {
            get
            {
                if (_rotateClockwiseCommand == null)
                    _rotateClockwiseCommand = new RelayCommand(RotateClockwise);
                return _rotateClockwiseCommand;
            }
        }

        private void RotateClockwise(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate Anti-Clockwise
        private ICommand _rotateAntiClockwiseCommand;
        public ICommand RotateAntiClockwiseCommand
        {
            get
            {
                if (_rotateAntiClockwiseCommand == null)
                    _rotateAntiClockwiseCommand = new RelayCommand(RotateAntiClockwise);
                return _rotateAntiClockwiseCommand;
            }
        }

        private void RotateAntiClockwise(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateAntiClockwise(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateAntiClockwise(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #region Crop image
        private ICommand _cropImageCommand;
        public ICommand CropImageCommand
        {
            get
            {
                if (_cropImageCommand == null)
                    _cropImageCommand = new RelayCommand(CropImage);
                return _cropImageCommand;
            }
        }

        private void CropImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            if (MouseClickCollection.Count < 2)
            {
                MessageBox.Show("Please select an area first (2 clicks: top-left and bottom-right)!");
                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;
            foreach (var point in MouseClickCollection)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }

            int x1 = (int)minX, y1 = (int)minY;
            int x2 = (int)maxX, y2 = (int)maxY;

            if (GrayInitialImage != null)
            {
                x1 = System.Math.Max(0, x1);
                y1 = System.Math.Max(0, y1);
                x2 = System.Math.Min(GrayInitialImage.Width, x2);
                y2 = System.Math.Min(GrayInitialImage.Height, y2);
            }
            else if (ColorInitialImage != null)
            {
                x1 = System.Math.Max(0, x1);
                y1 = System.Math.Max(0, y1);
                x2 = System.Math.Min(ColorInitialImage.Width, x2);
                y2 = System.Math.Min(ColorInitialImage.Height, y2);
            }

            if (x2 <= x1 || y2 <= y1)
            {
                MessageBox.Show("Invalid selection area!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Crop(GrayInitialImage, x1, y1, x2, y2);
                ProcessedImage = Convert(GrayProcessedImage);

                var (mean, stdDev) = Tools.ComputeMeanStdDev(GrayProcessedImage);
                MessageBox.Show($"Mean: {mean:F2}\nStd Dev: {stdDev:F2}",
                                "Crop Statistics");
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Crop(ColorInitialImage, x1, y1, x2, y2);
                ProcessedImage = Convert(ColorProcessedImage);

                var grayCrop = Tools.Convert(ColorProcessedImage);
                var (mean, stdDev) = Tools.ComputeMeanStdDev(grayCrop);
                MessageBox.Show($"Mean: {mean:F2}\nStd Dev: {stdDev:F2}",
                                "Crop Statistics");
            }
        }
        #endregion
        #endregion

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }
        #endregion

        #region Visualize color levels

        #region Row color levels
        private ICommand _rowColorLevelsCommand;
        public ICommand RowColorLevelsCommand
        {
            get
            {
                if (_rowColorLevelsCommand == null)
                    _rowColorLevelsCommand = new RelayCommand(RowColorLevels);
                return _rowColorLevelsCommand;
            }
        }

        private void RowColorLevels(object parameter)
        {
            if (RowColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region Column color levels
        private ICommand _columnColorLevelsCommand;
        public ICommand ColumnColorLevelsCommand
        {
            get
            {
                if (_columnColorLevelsCommand == null)
                    _columnColorLevelsCommand = new RelayCommand(ColumnColorLevels);
                return _columnColorLevelsCommand;
            }
        }

        private void ColumnColorLevels(object parameter)
        {
            if (ColumnColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            HistogramWindow window = null;

            if (ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            HistogramWindow window = null;

            if (ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images!");
            }
        }
        #endregion
        #region Binary image
        private ICommand _binaryImageCommand;
        public ICommand BinaryImageCommand
        {
            get
            {
                if (_binaryImageCommand == null)
                    _binaryImageCommand = new RelayCommand(BinaryImage);
                return _binaryImageCommand;
            }
        }

        private void BinaryImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            //List<string> labels = new List<string>();
            //labels.Add("Threshold:");

            // DialogWindow diag = new DialogWindow(_mainVM, labels);
            // diag.ShowDialog();

            //List<double> values = diag.GetValues();
            //byte threshold = (byte)values[0];

            List<string> labels = new List<string> { "Threshold" };

            DialogWindow diag = new DialogWindow(_mainVM, labels);
            diag.ShowDialog();

            List<double> values = diag.GetValues();
            byte threshold = (byte)values[0];

            if(threshold <10 || threshold >145)
            {
                MessageBox.Show("Threshold values must be between 10 and 145!");
                return;
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Binary(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add an image!");
            }
        }
        #endregion
        #endregion

        #region Pointwise operations
        #region BrightnessContrast image
        private ICommand _bcImageCommand;
        public ICommand BCImageCommand
        {
            get
            {
                if (_bcImageCommand == null)
                    _bcImageCommand = new RelayCommand(BrightnessContrastImage);
                return _bcImageCommand;
            }
        }

        private void BrightnessContrastImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);
            List<string> labels = new List<string>()
            {
                "a:",
                "b"
            };
            DialogWindow window=new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            double a = values[0];
            double b = values[1];
            if (a < 0) a = 1;
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.BrightnessContrast(GrayInitialImage, a,b);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.BrightnessContrast(ColorProcessedImage, a,b);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion
        #endregion

        #region Thresholding

        #region Otsu Thresholding
        private ICommand _otsuThresholdingCommand;
        public ICommand OtsuThresholdingCommand
        {
            get
            {
                if (_otsuThresholdingCommand == null)
                    _otsuThresholdingCommand = new RelayCommand(OtsuThresholdingImage);
                return _otsuThresholdingCommand;
            }
        }

        private void OtsuThresholdingImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = GrayProcessedImage = Thresholding.OtsuThresholding(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                GrayProcessedImage = Thresholding.OtsuThresholding(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion
        #region Gamma operator
        private ICommand _gammaImageCommand;
        public ICommand GammaImageCommand
        {
            get
            {
                if (_gammaImageCommand == null)
                    _gammaImageCommand = new RelayCommand(GammaImage);
                return _gammaImageCommand;
            }
        }

        private void GammaImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string>() { "gamma:" };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            double gamma = values[0];

            if (gamma <= 0)
            {
                MessageBox.Show("Gamma must be strictly positive!");
                return;
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.GammaOperator(GrayInitialImage, gamma);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.GammaOperator(ColorInitialImage, gamma);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion
        #endregion

        #region Filters

        #region Filters

        #region Custom filter 1 - Low pass
        private ICommand _customFilterCommand;
        public ICommand CustomFilterCommand
        {
            get
            {
                if (_customFilterCommand == null)
                    _customFilterCommand = new RelayCommand(CustomFilterImage);
                return _customFilterCommand;
            }
        }

        private void CustomFilterImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            double[,] kernel = new double[3, 3]
            {
        { 0.1, 0.1, 0.1 },
        { 0.1, 0.2, 0.1 },
        { 0.1, 0.1, 0.1 }
            };

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.ApplyFilter(GrayInitialImage, kernel);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.ApplyFilter(ColorInitialImage, kernel);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Custom filter 2 - High pass
        private ICommand _customFilter2Command;
        public ICommand CustomFilter2Command
        {
            get
            {
                if (_customFilter2Command == null)
                    _customFilter2Command = new RelayCommand(CustomFilter2Image);
                return _customFilter2Command;
            }
        }

        private void CustomFilter2Image(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            double[,] kernel = new double[3, 3]
            {
        {  0, -1,  0 },
        { -1,  5, -1 },
        {  0, -1,  0 }
            };

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Filters.ApplyFilter(GrayInitialImage, kernel);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Filters.ApplyFilter(ColorInitialImage, kernel);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Separable filter
        private ICommand _separableFilterCommand;
        public ICommand SeparableFilterCommand
        {
            get
            {
                if (_separableFilterCommand == null)
                    _separableFilterCommand = new RelayCommand(SeparableFilterImage);
                return _separableFilterCommand;
            }
        }

        private void SeparableFilterImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            double[,] horizontal = new double[1, 3] { { 0.25, 0.5, 0.25 } };
            double[,] vertical = new double[3, 1] { { 0.25 }, { 0.5 }, { 0.25 } };

            if (GrayInitialImage != null)
            {
                var temp = Filters.ApplyFilter(GrayInitialImage, horizontal);
                GrayProcessedImage = Filters.ApplyFilter(temp, vertical);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                var temp = Filters.ApplyFilter(ColorInitialImage, horizontal);
                ColorProcessedImage = Filters.ApplyFilter(temp, vertical);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #endregion

        #endregion

        #region Morphological operations
        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation
        #endregion

        #region Use processed image as initial image
        private ICommand _useProcessedImageAsInitialImageCommand;
        public ICommand UseProcessedImageAsInitialImageCommand
        {
            get
            {
                if (_useProcessedImageAsInitialImageCommand == null)
                    _useProcessedImageAsInitialImageCommand = new RelayCommand(UseProcessedImageAsInitialImage);
                return _useProcessedImageAsInitialImageCommand;
            }
        }

        private void UseProcessedImageAsInitialImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = Convert(GrayInitialImage);
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = Convert(ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion
    }
}